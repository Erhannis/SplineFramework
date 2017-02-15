using UnityEngine;
using System.Collections;
using System;
using System.Text;

[Serializable]
public class ColorSpline {
    public long id;
    public ColorSolid space;
    public BezierSpline spline;

    public ColorSpline() {
        spline = new BezierSpline();
    }

	public int InterpolateToARGB(float t) {
		float[] pos = spline.Interpolate(t);
		return space.GetARGB(pos);
    }

    public float[] InterpolateToFARGB(float t)
    {
        float[] pos = spline.Interpolate(t);
		return space.GetFARGB(pos);
    }

    public void SetPoints(Vector3[] points) {
      float[,] newPts = new float[3, points.Length];
      for (int i = 0; i < points.Length; i++) {
        newPts[0, i] = points[i].x;
        newPts[1, i] = points[i].y;
        newPts[2, i] = points[i].z;
      }
      spline.SetPoints(newPts);
    }

	public string ToJSON() {
		StringBuilder sb = new StringBuilder();
		sb.Append("{");
		sb.Append("id:" + id);
		sb.Append(", ");
		sb.Append("space:" + space); //TODO ToJSON?
		sb.Append(", ");
		sb.Append("spline:" + spline.ToJSON());
		sb.Append("}");
		return sb.ToString();
	}

	public static ColorSpline FromJSON(string json) {
		throw new Exception("Not yet implemented");
	}
}