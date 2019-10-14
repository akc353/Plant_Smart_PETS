using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreParentPosition : MonoBehaviour
{
    public bool isXAxis, isYAxis, isZAxis;

    Transform t;
    Transform pt;
    Vector3 OriginPos;
    Vector3 fixedPos = new Vector3();

    public Transform ProxyTarget;
    Vector3 StoreTargetPos;

    void Start ()
    {
        t = this.transform;
        pt = t.parent;

        OriginPos = t.transform.position;
    }

    public void Update()
    {
        BeforeProxyAnimationData();
    }

    void LateUpdate ()
    {
        AfterProxyAnimationData();
        IgnorePos();
    }

    private void IgnorePos()
    {
        fixedPos = t.transform.position;

        if (isXAxis)
        {
            fixedPos.x = OriginPos.x;
        }
        if (isYAxis)
        {
            fixedPos.y = OriginPos.y;
        }
        if (isZAxis)
        {
            fixedPos.z = OriginPos.z;
        }

        t.transform.position = fixedPos;
    }

    private void BeforeProxyAnimationData()
    {
        if (ProxyTarget == null) return;

        StoreTargetPos = ProxyTarget.position;
    }

    private void AfterProxyAnimationData()
    {
        if (ProxyTarget == null) return;

        var deltaPos = ProxyTarget.position - StoreTargetPos;

        if (deltaPos.y * -1 < 0)
        {
            OriginPos.y += deltaPos.y;
        }
    }
}
