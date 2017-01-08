using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/**
 * Adapted from VRTK_ControllerEvents_ListenerExample
 */
public class SplineControllerListener : MonoBehaviour {
    //TODO This is kinda hacky
    public ColorSplineObject colorSplineObject;

    private bool triggerDown = false;

    private void Start() {
        if (GetComponent<VRTK_ControllerEvents>() == null) {
            Debug.LogError("SplineControllerListener is required to be attached to a SteamVR Controller that has the VRTK_ControllerEvents script attached to it");
            return;
        }

        //Setup controller event listeners
        GetComponent<VRTK_ControllerEvents>().TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
        GetComponent<VRTK_ControllerEvents>().TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);

        GetComponent<VRTK_ControllerEvents>().TriggerAxisChanged += new ControllerInteractionEventHandler(DoTriggerAxisChanged);

        GetComponent<VRTK_ControllerEvents>().GripPressed += new ControllerInteractionEventHandler(DoGripPressed);
        GetComponent<VRTK_ControllerEvents>().GripReleased += new ControllerInteractionEventHandler(DoGripReleased);

        GetComponent<VRTK_ControllerEvents>().TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
        GetComponent<VRTK_ControllerEvents>().TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);

        GetComponent<VRTK_ControllerEvents>().TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouchStart);
        GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);

        GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
    }

    private void DebugLogger(uint index, string button, string action, ControllerInteractionEventArgs e) {
        Debug.Log("Controller on index '" + index + "' " + button + " has been " + action + " with a pressure of " + e.buttonPressure + " / trackpad axis at: " + e.touchpadAxis + " (" + e.touchpadAngle + " degrees)");
    }

    private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e) {
        DebugLogger(e.controllerIndex, "TRIGGER", "pressed down", e);
        triggerDown = true;
        GameObject controller = VRTK.VRTK_DeviceFinder.GetControllerByIndex(e.controllerIndex, true);
        //SteamVR_TrackedObject controller = null;
        colorSplineObject.ShowTargetMarker(controller.transform.position);
    }

    private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e) {
        DebugLogger(e.controllerIndex, "TRIGGER", "released", e);
        triggerDown = false;
        colorSplineObject.HideTargetMarker();
    }

    private void DoTriggerAxisChanged(object sender, ControllerInteractionEventArgs e) {
        DebugLogger(e.controllerIndex, "TRIGGER", "axis changed", e);
        if (e.buttonPressure == 1) {
            GameObject controller = VRTK.VRTK_DeviceFinder.GetControllerByIndex(e.controllerIndex, true);
            //SteamVR_TrackedObject controller = null;
            colorSplineObject.AddPoint(controller.transform.position);
        }
    }

    private void DoGripPressed(object sender, ControllerInteractionEventArgs e) {
        DebugLogger(e.controllerIndex, "GRIP", "pressed down", e);
    }

    private void DoGripReleased(object sender, ControllerInteractionEventArgs e) {
        DebugLogger(e.controllerIndex, "GRIP", "released", e);
    }

    private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e) {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "pressed down", e);
    }

    private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e) {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "released", e);
    }

    private void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e) {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "touched", e);
    }

    private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e) {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "untouched", e);
    }

    private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e) {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "axis changed", e);
    }
}
