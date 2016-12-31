using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorSplineObject : MonoBehaviour, IPostRenderer {
    private ColorSpline spline;
    public List<GameObject> controlObjects;

    //TODO Improve material efficiency?
    public Material lineMat;

    void Start() {
      spline = new ColorSpline();
      spline.spline.SetOrder(3);
      controlObjects = new List<GameObject>();
      for (int i = 0; i < 4; i++) {
        //TODO Maybe a prefab
        //TODO Maybe different for 1st vs 2nd control points
		GameObject controlObject;
		if (i % 3 == 0) {
			controlObject = GameObject.CreatePrimitive (PrimitiveType.Cube);
		} else {
			controlObject = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		}
        //controlObject.AddComponent<RigidBody>();
        controlObject.transform.parent = transform;
        controlObject.transform.localPosition = new Vector3(i / 3f, i/3f, i/3f);
		float size = 0.1f;
			controlObject.transform.localScale = new Vector3(size, size, size);
        controlObjects.Add(controlObject);
      }
      UpdatePoints();
    }
  
    void Update() {
        //TODO Remove, probably
        UpdatePoints();
    }
  
    private void UpdatePoints() {
      Vector3[] newPts = new Vector3[controlObjects.Count];
      for (int i = 0; i < controlObjects.Count; i++) {
            GameObject controlObject = controlObjects[i];
        newPts[i] = controlObject.transform.position;
      }
      spline.SetPoints(newPts);
    }
  
    public void Stuff() {
      //TODO Do
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
      
      int i = 0;
      //TODO Make 2nd control points nice
      GameObject before = controlObjects[i - 2];
      GameObject after = controlObjects[i + 1];
      
      float[] pos = spline.spline.Interpolate(t);
      GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
      b.transform.position = new Vector3(pos[0], pos[1], pos[2]);
      
      GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
      a.transform.position = new Vector3(1, 0, 0);
      a.transform.parent = b.transform;
      
      GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
      c.transform.position = new Vector3(-1, 0, 0);
      c.transform.parent = b.transform;
      
      UpdatePoints();
    }

    public void DoRender() {
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        //GL.MultMatrix(transform.localToWorldMatrix);

        lineMat.SetPass(0);
        GL.Color(new Color(1f, 1f, 1f, 1f));

		/**/
        float h = 1f / 16;
        for (float t = 0; (t + h) <= 1; t += h)
        {
            float[] pt1 = spline.spline.Interpolate(t);
            float[] pt2 = spline.spline.Interpolate(t + h);
			Debug.Log(pt1[0] + " " + pt1[1] + " " + pt1[2] + " -> " + pt2[0] + " " + pt2[1] + " " + pt2[2]);
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

        float[] pt = spline.spline.Interpolate(1);
        Debug.Log(pt[0] + " " + pt[1] + " " + pt[2]);

		Debug.Log("blah");

        GL.End();
        GL.PopMatrix();
    }
}