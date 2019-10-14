using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTargetForHeadCollision : MonoBehaviour
{
    public GameObject MovingTarget;

    Transform Height;
    Transform VRCamera;
    Transform Field;

    bool IsFollowingVRCamera = true;

    float DeltaY;

    private void Awake()
    {
        DeltaY = transform.position.y - MovingTarget.transform.position.y;

        MovingTarget.SetActive(false);

        Height = PlayerMgr.Inst.Get("DeviceHeight").transform;
        VRCamera = PlayerMgr.Inst.Get("VRCamera").transform;
        Field = PlayerMgr.Inst.Get("VROrigin").transform;
    }
    

    IEnumerator Co_LookAtTarget(Transform targetTrans)
    {
        Quaternion preAngles = MovingTarget.transform.rotation;
        MovingTarget.transform.LookAt(targetTrans);
        Quaternion postAngles = MovingTarget.transform.rotation;

        var line1 = MovingTarget.transform.position - MovingTarget.transform.position;
        line1 = line1.normalized;

        MovingTarget.transform.rotation = preAngles;
        var line2 = MovingTarget.transform.forward;
        float time = 0;
        while (true)
        {
            var angle = Vector3.Angle(line2, line1);
            time += Time.deltaTime * 0.1f;

            while (angle < 15f)
            {
                MovingTarget.transform.rotation = Quaternion.Slerp(preAngles, postAngles, time);
            }
            yield return null;
        }
    }

    public void LookAtTarget(Transform Target)
    {
        StartCoroutine(Co_LookAtTarget(Target));
    }

    IEnumerator RotatePlayer()
    {
        float angleZ = 0;
        Transform Player = MovingTarget.transform;
        float posZ = Player.position.z;
        float posX = Player.position.x;

        while (angleZ < 55f)
        {
            angleZ += Time.deltaTime * 50f;
            Player.localRotation = Quaternion.Euler(0, 0, -angleZ);
            var pos = Player.position;

            pos.z = posZ;
            pos.x = posX;

            Player.position = pos;

            yield return null;
        }

        float angleX = 0;


        while (angleX < 180f)
        {
            Player.localRotation = Quaternion.Euler(angleX, 0, -angleZ);

            angleX -= Time.deltaTime * 100f;

            var pos = Player.position;

            pos.z = posZ;
            pos.x = posX;

            Player.position = pos;

            yield return null;
        }
    }
    private void Update()
    {
        if (IsFollowingVRCamera)
        {
            var camPos = VRCamera.position;
            float y = transform.position.y;

            y += VRCamera.position.y - MovingTarget.transform.position.y;
            camPos.y = y;


            transform.position = camPos;
        }
    }
}
