using UnityEngine;
using System.Collections;

public class ColorSpline {
    public long id;
    public ColorSolid space;
    public BezierSpline spline;

	//TODO Maybe should skip alpha, unless we MEAN it
	public int InterpolateToARGB(float t) {
		float[] color = spline.Interpolate(t);
		switch (space) {
			case ColorSolid.RGB_CUBE:
				float rf = Clamp(color[0], 0, 1);
				float gf = Clamp(color[1], 0, 1);
				float bf = Clamp(color[2], 0, 1);
				//TODO Are we shortchanging 0xFF here?
				int r = (int)(0xFF * rf);
				int g = (int)(0xFF * gf);
				int b = (int)(0xFF * bf);
				return (int)((0xFF000000) + (0x00010000 * r) + (0x00000100 * g) + (0x00000001 * b));
			case ColorSolid.HSV_CUBE:
			case ColorSolid.HSV_CYLINDER:
			case ColorSolid.HSL_CUBE:
			case ColorSolid.HSL_CYLINDER:
			case ColorSolid.CIE_LAB:
			default:
				return unchecked((int)(0xFFFF00FF));
		}
	}
	
	private static float Clamp(float x, float min, float max) {
		return Mathf.Max(Mathf.Min(x, max), min);
	}
}
