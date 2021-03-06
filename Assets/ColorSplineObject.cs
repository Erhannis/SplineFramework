using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;

public class ColorSplineObject : MonoBehaviour, IPostRenderer {
	public delegate void UpdateSplineEventHandler(ColorSpline spline);
	public event UpdateSplineEventHandler UpdateSplineEvent;

    private ColorSpline spline;
    public GameObject firstControlPrefab;
    public GameObject secondControlPrefab;
    public List<GameObject> controlObjects; //TODO Hide, or pre-pre-generate?

    private GameObject targetMarker;

    //TODO Improve material efficiency?
    public Material lineMat;

    private const float CONTROL_SIZE = 0.1f;

    void Start() {
        targetMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        targetMarker.GetComponent<Renderer>().material.color = Color.green;
        //TODO Could make transparent
        targetMarker.transform.parent = transform;
        targetMarker.transform.localPosition = new Vector3();
        targetMarker.transform.localScale = new Vector3(CONTROL_SIZE, CONTROL_SIZE, CONTROL_SIZE);
        targetMarker.SetActive(false);

        spline = new ColorSpline();
        spline.spline.SetOrder(3);
        controlObjects = new List<GameObject>();
        for (int i = 0; i < 4; i++) {
            GameObject controlObject;
            if (i % 3 == 0) {
                controlObject = Instantiate<GameObject>(firstControlPrefab, transform);
            } else {
                controlObject = Instantiate<GameObject>(secondControlPrefab, transform);
            }
            controlObject.transform.localPosition = new Vector3(i / 3f, i / 3f, i / 3f);
            controlObject.transform.localScale = new Vector3(CONTROL_SIZE, CONTROL_SIZE, CONTROL_SIZE);
            controlObjects.Add(controlObject);
        }
        // Make 2nd controls children of the 1sts
        for (int i = 1; i < (4-1); i++) {
            GameObject controlObject;
            if (i % 3 == 0) {
                // 1st control
            } else {
                // 2nd control
                //TODO Could add support for greater order
                controlObject = controlObjects[i];
                GameObject firstControl;
                if (i % 3 == 1) {
                    // back one
                    firstControl = controlObjects[i - 1];
                } else {
                    // i % 3 == 2
                    firstControl = controlObjects[i + 1];
                }
                controlObject.transform.SetParent(firstControl.transform, true);
            }
        }
        UpdatePoints();
    }

    void Update() {
		foreach (GameObject controlPoint in controlObjects) {
			if (controlPoint.GetComponent<PositionChanged>().PullChanged()) {
				UpdatePoints();
				break;
			}
		}
    }

    private void UpdatePoints() {
        Vector3[] newPts = new Vector3[controlObjects.Count];
        for (int i = 0; i < controlObjects.Count; i++) {
            GameObject controlObject = controlObjects[i];
            newPts[i] = controlObject.transform.position;
        }
        spline.SetPoints(newPts);
		UpdateSplineEvent(spline);
    }

    public void Stuff() {
        //TODO Do
    }

    public void ShowTargetMarker(Vector3 near) {
        //TODO Make this update as controller moves?
        float t = FindClosestT(near);
        float[] pos = spline.spline.Interpolate(t);
        targetMarker.transform.position = new Vector3(pos[0], pos[1], pos[2]);
        targetMarker.SetActive(true);
    }

    public void HideTargetMarker() {
        targetMarker.SetActive(false);
    }

    public float FindClosestT(Vector3 pos) {
        float minDistance = float.MaxValue;
        float minT = 0;
        for (float t = 0; t <= 1; t += 0.001f) {
            float[] p = spline.spline.Interpolate(t);
            float distSqr = new Vector3(pos.x - p[0], pos.y - p[1], pos.z - p[2]).sqrMagnitude;
            if (distSqr < minDistance) {
                minDistance = distSqr;
                minT = t;
            }
        }
        return minT;
    }

    public void AddPoint(Vector3 pos) {
        AddPoint(FindClosestT(pos));
    }

    public void AddPoint(float t) {
        // This won't work well if you try to put a point on the end
        if (t <= 0 || t >= 1) {
            return;
        }
        //TODO This assumes order 3, 1 reused
        int firstControlPoints = ((controlObjects.Count - 1) / 3) + 1;
        int gp = (int)(t * (firstControlPoints - 1));
        int target = 2 + (3 * gp);

        //TODO Make 2nd control points nice
        GameObject before = controlObjects[target - 2];
        GameObject after = controlObjects[target + 1];

        float[] pos = spline.spline.Interpolate(t);
        GameObject b = Instantiate<GameObject>(firstControlPrefab, transform);
        b.transform.position = new Vector3(pos[0], pos[1], pos[2]);
        b.transform.localScale = new Vector3(CONTROL_SIZE, CONTROL_SIZE, CONTROL_SIZE);

        //TODO Something's wrong with the sizes
        GameObject a = Instantiate<GameObject>(secondControlPrefab);
        a.transform.SetParent(transform, false);
        a.transform.SetParent(b.transform, true);
        a.transform.localPosition = new Vector3(2, 0, 0);

        GameObject c = Instantiate<GameObject>(secondControlPrefab);
        c.transform.SetParent(transform, false);
        c.transform.SetParent(b.transform, true);
        c.transform.localPosition = new Vector3(-2, 0, 0);

        controlObjects.Insert(target, c);
        controlObjects.Insert(target, b);
        controlObjects.Insert(target, a);

        UpdatePoints();
    }

    public void DeletePointClosestToRightController() {
        //TODO Store controller manager?
        Vector3 pos = FindObjectOfType<SteamVR_ControllerManager>().right.transform.position;
        DeletePoint(pos);
    }

    public void DeletePoint(Vector3 pos) {
        //TODO Update for different order, reuse
        int closestI = FindClosestFirstControlIndex(pos);
        if (closestI > -1) {
            if (controlObjects.Count > 4) {
                if (closestI == 0) {
                    // Remove first three
                    Destroy(controlObjects[0]);
                    Destroy(controlObjects[1]);
                    Destroy(controlObjects[2]);
                    controlObjects.RemoveRange(0, 3);
                } else if (closestI == (controlObjects.Count - 1)) {
                    // Remove last three
                    Destroy(controlObjects[controlObjects.Count - 3]);
                    Destroy(controlObjects[controlObjects.Count - 2]);
                    Destroy(controlObjects[controlObjects.Count - 1]);
                    controlObjects.RemoveRange(controlObjects.Count - 3, 3);
                } else {
                    // Remove three from middle
                    Destroy(controlObjects[closestI - 1]);
                    Destroy(controlObjects[closestI]);
                    Destroy(controlObjects[closestI + 1]);
                    controlObjects.RemoveRange(closestI - 1, 3);
                }
                UpdatePoints();
            }
        }
    }

    public int FindClosestFirstControlIndex(Vector3 pos) {
        //TODO Update for different order, reuse
        int closestI = -1;
        float distSqr = float.PositiveInfinity;
        for (int i = 0; i < controlObjects.Count; i += 3) {
            float sqrMag = (controlObjects[i].transform.position - pos).sqrMagnitude;
            if (sqrMag < distSqr) {
                distSqr = sqrMag;
                closestI = i;
            }
        }
        return closestI;
    }

    public void DoRender() {
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        //GL.MultMatrix(transform.localToWorldMatrix);

        lineMat.SetPass(0);
        GL.Color(new Color(1f, 1f, 1f, 1f));

        /**/
        float h = 1f / (1 << 8);
        for (float t = 0; (t + h) <= 1; t += h) {
            float[] pt1 = spline.spline.Interpolate(t);
            float[] pt2 = spline.spline.Interpolate(t + h);
            //Debug.Log(pt1[0] + " " + pt1[1] + " " + pt1[2] + " -> " + pt2[0] + " " + pt2[1] + " " + pt2[2]);
            float[] color = spline.InterpolateToFARGB(t);
            GL.Color(new Color(color[0], color[1], color[2], color[3]));
            GL.Vertex3(pt1[0], pt1[1], pt1[2]);
            GL.Vertex3(pt2[0], pt2[1], pt2[2]);
        }
        /**/

        /*
		GL.Vertex3(0, 0, 0);
		GL.Vertex3(1, 0, 0);

		GL.Vertex3(1, 0, 0);
		GL.Vertex3(1, 1, 0);

		GL.Vertex3(1, 1, 0);
		GL.Vertex3(0, 1, 0);
		*/

        //float[] pt = spline.spline.Interpolate(1);
        //Debug.Log(pt[0] + " " + pt[1] + " " + pt[2]);

        //Debug.Log("blah");

        GL.End();
        GL.PopMatrix();
    }
}