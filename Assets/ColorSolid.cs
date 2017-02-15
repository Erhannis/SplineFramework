public enum ColorSolid
{
	RGB_CUBE, HSV_CUBE, HSV_CYLINDER, HSL_CUBE, HSL_CYLINDER, CIE_LAB
}

static class ColorSolidMethods
{

	public static float[] GetFARGB(this ColorSolid colorSolid, float[] position)
	{
		switch (colorSolid)
		{
		case ColorSolid.RGB_CUBE:
			float rf = BezierSpline.Clamp(position[0], 0, 1);
			float gf = BezierSpline.Clamp(position[1], 0, 1);
			float bf = BezierSpline.Clamp(position[2], 0, 1);
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

	//TODO Maybe should skip alpha, unless we MEAN it
	public static int GetARGB(this ColorSolid colorSolid, float[] position) {
		float[] color = colorSolid.GetFARGB(position);
		//TODO Are we shortchanging 0xFF here?
		int r = (int)(0xFF * color[0]);
		int g = (int)(0xFF * color[1]);
		int b = (int)(0xFF * color[2]);
		return (int)((0xFF000000) + (0x00010000 * r) + (0x00000100 * g) + (0x00000001 * b));
	}
}