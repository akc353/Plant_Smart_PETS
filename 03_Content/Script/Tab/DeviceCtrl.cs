using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;

public class DeviceCtrl : MonoBehaviour {
    public VivePoseTracker tracker;
    public int index;

    public SteamVR_Controller.Device device;
    Valve.VR.EVRButtonId triggerId = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    public rayshoot ray;

    private void Awake()
    {
        tracker = GetComponent<VivePoseTracker>();
    }

    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        while (!tracker.isPoseValid)
            yield return null;
        index = (int)tracker.viveRole.GetDeviceIndex();

        device = SteamVR_Controller.Input(index);
    }
	
	// Update is called once per frame
	void Update () {
        if (device == null)
            return;
        if (device.GetPressDown(triggerId))
        {
            ray.trigger = true;
        }
        else if (device.GetPressUp(triggerId))
        {
            ray.triggerUp = true;
            ray.trigger = false;
        }
	}
}
