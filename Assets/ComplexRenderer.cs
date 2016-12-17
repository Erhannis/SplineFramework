using UnityEngine;
using System.Collections;
using System.Numerics; // Illusion; copied in Complex.cs

public class ComplexRenderer : MonoBehaviour, IPostRenderer {
    private int count;
    private Complex[] values;
    private Vector3[] positions;

    //TODO Improve material efficiency?
    public Material lineMat;

    // Use this for initialization
    void Start () {
        count = -1;
        //TODO Use shader?
    }

    private void UpdatePositions() {
        for (int i = 0; i < count; i++) {
            Complex val = values[i];
            positions[i] = new Vector3((float)val.Real, (float)val.Imaginary, (((float)i) / count));
        }
    }

    /**
     * If you change the numbers in values after calling this, the lines will not automatically
     * update, fyi.
     */
    public void SetValues(Complex[] values) {
        this.values = values;
        if (values.Length != count) {
            count = values.Length;
            positions = new Vector3[count];
        }
        UpdatePositions();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void DoRender() {
        BezierSpline spline = new BezierSpline();
        spline.SetOrder(3);
        float[,] pts = new float[3, 4];
        pts[0, 0] = 0;
        pts[1, 0] = 0;
        pts[2, 0] = 0;
        pts[0, 1] = 0.5f;
        pts[1, 1] = 0;
        pts[2, 1] = 0;
        pts[0, 2] = 1;
        pts[1, 2] = 0.5f;
        pts[2, 2] = 1;
        pts[0, 3] = 1;
        pts[1, 3] = 1;
        pts[2, 3] = 1;
        spline.SetPoints(pts);

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        GL.MultMatrix(transform.localToWorldMatrix);

        lineMat.SetPass(0);
        GL.Color(new Color(0f, 1f, 0f, 1f));

        for (float t = 0; (t+0.01f) <= 1; t += 0.01f) {
            float[] pt1 = spline.Interpolate(t);
            float[] pt2 = spline.Interpolate(t + 0.01f);
            GL.Vertex3(pt1[0], pt1[1], pt1[2]);
            GL.Vertex3(pt2[0], pt2[1], pt2[2]);
        }

        GL.End();
        GL.PopMatrix();

        /*
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        GL.MultMatrix(transform.localToWorldMatrix);

        lineMat.SetPass(0);
        GL.Color(new Color(0f, 1f, 0f, 1f));
        
        for (int i = 0; i < positions.Length; i++) {
            Vector3 pos = positions[i];
            lineMat.SetPass(0);
            GL.Color(new Color(0f, 1f, 0f, 1f));
            GL.Vertex3(pos.x, pos.y, pos.z);
            if (i < positions.Length - 1) {
                pos = positions[i+1];
                GL.Vertex3(pos.x, pos.y, pos.z);
            }
        }

        GL.End();
        GL.PopMatrix();
        */

        /*
        Vector3 last = new Vector3(0, 0, 0);
        bool already = false;
        foreach (Vector3 pos in positions) {
            if (!already) {
                last = pos;
                already = true;
            } else {
                GL.Begin(GL.LINES);
                lineMat.SetPass(0);
                //GL.Color(new Color(0f, 1f, 0f, 1f));
                GL.Vertex3(last.x, last.y, last.z);
                GL.Vertex3(pos.x, pos.y, pos.z);
                GL.End();
                last = pos;
            }
        }
        */
    }
}
