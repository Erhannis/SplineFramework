using UnityEngine;
using System.Collections;
using System;

public class ColorCube : MonoBehaviour {
    public GameObject xRayPlane;
    public GameObject heatWand;

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private SteamVR_Controller.Device controller { get { return ((int)controllerTrackedObj.index) >= 0 ? SteamVR_Controller.Input((int)controllerTrackedObj.index) : null; } }
    private SteamVR_TrackedObject controllerTrackedObj;
    public GameObject controllerObject;

    private VoxelEngine engine;
    private System.Random random = new System.Random();

    private Color[,,] color;
	private ColorSolid colorSolid = ColorSolid.RGB_CUBE;

    private const int W = 25;
    private const int H = 25;
    private const int D = 25;

    // Use this for initialization
    void Start () {
        color = new Color[W, H, D];

		float dim = 1f;

        for (int x = 0; x < W; x++) {
            for (int y = 0; y < H; y++) {
                for (int z = 0; z < D; z++) {
					float[] fargb = colorSolid.GetFARGB(new float[]{x / (W - 1f), y / (H - 1f), z / (D - 1f)});
					color[x, y, z] = new Color(dim * fargb[1], dim * fargb[2], dim * fargb[3], 0.01f);
                }
            }
        }

        controllerTrackedObj = controllerObject.GetComponent<SteamVR_TrackedObject>();

        engine = GetComponent<VoxelEngine>();
        engine.Init(W, H, D);
    }

    // Update is called once per frame
    void Update () {
        engine.DoUpdate((x, y, z, t) => {
			return color[x, y, z];
        });
    }
}
