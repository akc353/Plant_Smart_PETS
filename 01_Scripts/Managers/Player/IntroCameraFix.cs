using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCameraFix : MonoBehaviour
{
	void Update ()
    {
        Intro_Camera_Inverse_RotAndPos_ByUpdate();
    }

    #region 인트로 무브/로테이션 관련

    [SerializeField]
    private GameObject originPoint;
    [SerializeField]
    private GameObject fixedRotation;
    [SerializeField]
    private GameObject fixedPosition;
    [SerializeField]
    private GameObject targetCamera;

    private bool isInverserPos = false;
    private bool isInverseRot = false;

    public GameObject OriginPoint
    {
        get
        {
            return originPoint;
        }

        set
        {
            originPoint = value;
        }
    }

    public GameObject FixedRotation
    {
        get
        {
            return fixedRotation;
        }

        set
        {
            fixedRotation = value;
        }
    }

    public GameObject FixedPosition
    {
        get
        {
            return fixedPosition;
        }

        set
        {
            fixedPosition = value;
        }
    }

    public GameObject TargetCamera
    {
        get
        {
            return targetCamera;
        }

        set
        {
            targetCamera = value;
        }
    }

    private void Intro_Camera_Inverse_RotAndPos_ByUpdate()
    {
        if (isInverserPos)
        {
            FixedPosition.transform.position = FixedPosition.transform.position + GetPosOffset();
        }

        if (isInverseRot)
        {
            FixedRotation.transform.localRotation = Quaternion.Inverse(TargetCamera.transform.localRotation);
        }
    }

    private void FixVrPositionAndRotation(bool value)
    {
        isInverserPos = value;
        isInverseRot = value;
    }

    private Vector3 GetPosOffset()
    {
        Vector3 posOffset = OriginPoint.transform.position - TargetCamera.transform.position;

        return posOffset;
    }

    #endregion
}
