using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreParentRotation : MonoBehaviour
{
    public bool isXAxis, isYAxis, isZAxis;

    Transform t;
    Quaternion originRot;
    Vector3 rotStore = new Vector3();

	void Start ()
    {
        t = this.transform;
        originRot = t.transform.rotation;
    }
	
	void LateUpdate()
    {
        IgnoreRot();
    }

    private void IgnoreRot()
    {
        rotStore = t.transform.rotation.eulerAngles;

        if (isXAxis)
        {
            rotStore.x = originRot.eulerAngles.x;
        }
        if (isYAxis)
        {
            rotStore.y = originRot.eulerAngles.y;
        }
        if (isZAxis)
        {
            rotStore.z = originRot.eulerAngles.z;
        }

        t.transform.rotation = Quaternion.Euler(rotStore);
    }
}
