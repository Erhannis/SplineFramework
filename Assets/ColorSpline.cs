using UnityEngine;
using System.Collections;

public class ColorSpline {
    public long id;
    public ColorSolid space;
    public BezierSpline spline;

    public ColorSpline() {
        spline = new BezierSpline();
    }

	//TODO Maybe should skip alpha, unless we MEAN it
	public int InterpolateToARGB(float t) {
		float[] color = InterpolateToFARGB(t);
        //TODO Are we shortchanging 0xFF here?
        int r = (int)(0xFF * color[0]);
        int g = (int)(0xFF * color[1]);
        int b = (int)(0xFF * color[2]);
        return (int)((0xFF000000) + (0x00010000 * r) + (0x00000100 * g) + (0x00000001 * b));
    }

    public float[] InterpolateToFARGB(float t)
    {
        float[] color = spline.Interpolate(t);
        switch (space)
        {
            case ColorSolid.RGB_CUBE:
                float rf = BezierSpline.Clamp(color[0], 0, 1);
			    float gf = BezierSpline.Clamp(color[1], 0, 1);
			    float bf = BezierSpline.Clamp(color[2], 0, 1);
                return new float[]{1f, rf, gf, bf};
            case ColorSolid.HSV_CUBE:
            case ColorSolid.HSV_CYLINDER:
            case ColorSolid.HSL_CUBE:
            case ColorSolid.HSL_CYLINDER:
            case ColorSolid.CIE_LAB:
            default:
                return new float[] {1f, 1f, 0f, 1f};
        }
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
}