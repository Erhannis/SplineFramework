using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionChanged : MonoBehaviour {
	private Vector3 lastPosition;
	private bool changed;

	// Use this for initialization
	void Start () {
		lastPosition = transform.position;
		changed = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!transform.position.Equals(lastPosition)) {
            Vector3 curPos = transform.position;
			lastPosition = new Vector3(curPos.x, curPos.y, curPos.z);
			changed = true;
		}
	}

	public bool PullChanged() {
		bool result = changed;
		changed = false;
		return result;
	}

	public bool PeekChanged() {
		return changed;
	}
}
