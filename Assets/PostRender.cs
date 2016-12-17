using UnityEngine;
using System.Collections;

public class PostRender : MonoBehaviour {
    //TODO This is dumb
    public MonoBehaviour[] renderers;

	// Use this for initialization
	void Start () {

    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnPostRender() {
        //TODO I don't like that I need to put a script on the camera.
        foreach (IPostRenderer renderer in renderers) {
            renderer.DoRender();
        }
    }
}
