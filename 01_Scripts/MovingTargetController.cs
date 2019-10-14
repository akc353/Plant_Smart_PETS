using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTargetController : MonoBehaviour
{
    public GameObject Floor;
    public GameObject Collider;
    public GameObject MovingTarget;

    Transform Height;
    Transform VRCamera;

    bool IsFollowingVRCamera = true;

    bool followingRotation = false;

    float DeltaY;

    private void Awake()
    {
        DeltaY = transform.position.y - MovingTarget.transform.position.y;

        MovingTarget.SetActive(false);
        Height = PlayerMgr.Inst.Get("DeviceHeight").transform;
        VRCamera = PlayerMgr.Inst.Get("VRCamera").transform;
    }

    public enum Pos
    {
        HeadCollision,
        Falling
    }
    
    public void SetParentMovingTargetController(Pos pos)
    {
        switch (pos)
        {
            case Pos.HeadCollision:
                transform.parent = GameState.Inst.NowLocation.Get("Slewing mechanism_AM2").transform;
                //위치 잡아주기
                break;
            case Pos.Falling:
                transform.parent = GameState.Inst.NowLocation.Get("Counter jib_AM").transform;
                //위치 잡아주기
                break;
        }
    }

    public void SetIsFollowingVRCamera(bool bStatus, float waitTime = 0f)
    {
        IsFollowingVRCamera = bStatus;

        if (!IsFollowingVRCamera)
        {
            followingRotation = true;

            
            StartCoroutine(WaitFalling(waitTime));
        }
    }

    IEnumerator Co_LookAtTarget(Transform targetTrans)
    {
        Transform Player = transform.Find("TestPlayer");

        Vector3 preRot = Player.localEulerAngles;
        Player.LookAt(targetTrans.position);//,Vector3.up);
        Vector3 rot = Player.localEulerAngles;
        Player.localRotation = Quaternion.Euler(preRot);

        Vector3 deltaRot = (preRot + rot) / 20;
        Vector3 cameraPos = VRCamera.position;

        IsFollowingVRCamera = false;
        for (int i = 0; i < 20; i++)
        {
            //Player.localEulerAngles -= deltaRot;
            preRot -= deltaRot;
            Player.localRotation = Quaternion.Euler(preRot);
            yield return new WaitForSeconds(0.01f);
        }

        float angle = 0;
        float up = VRCamera.localEulerAngles.x;
        var playerAngle = Player.localEulerAngles;
        

        IsFollowingVRCamera = false;
        
        while (angle < up)
        {
            angle += Time.deltaTime * 50f;
            Player.localRotation = Quaternion.Euler(-angle + playerAngle.x, playerAngle.y, playerAngle.z);

            Player.position += cameraPos - VRCamera.position;

            yield return null;
        }
        
        yield return null;
    }

    public void LookAtTarget(Transform Target)
    {
        //MovingTarget.SetActive(true);
        StartCoroutine(Co_LookAtTarget(Target));
    }


    public void InitMovingTarget()
    {
        IsFollowingVRCamera = true;
        Transform Player = transform.Find("TestPlayer");

        Floor.gameObject.SetActive(true);

        Player.localRotation = Quaternion.Euler(Vector3.zero);
        Player.localPosition = Vector3.zero;
        Player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        MovingTarget.SetActive(false);

        transform.position = new Vector3(0, 700, 0);

        transform.localRotation = Quaternion.Euler(new Vector3(180f, -90f, -90f));
    }


    IEnumerator WaitFalling(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        MovingTarget.SetActive(true);
        followingRotation = false;

        /////////////////////
        MovingTarget.transform.rotation = Height.rotation;
        ////////////////////

        //transform.SetParent(GameState.Inst.gameObject.transform);
        Floor.gameObject.SetActive(false); //이렇게되면 이건 굳이 필요없음 //Floor 자체
        //IsFollowingVRCamera = false;
   //     TestFollowing.isFollowRotation = true;
        StartCoroutine(RotatePlayer());
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
            Player.Rotate(0, 0, -Time.deltaTime * 50f);
            var pos = Player.position;

            pos.z = posZ;
            pos.x = posX;

            Player.position = pos;

            yield return null;
        }

        float angleX = 0;


        while (angleX < 180f)
        {
            Player.Rotate(-Time.deltaTime * 100f, 0, 0);

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
        //if (followingRotation)
        //    MovingTarget.transform.forward = VRCamera.forward;
        //{
        //    //var angle = MovingTarget.transform.rotation;
        //    //angle.y = VRCamera.eulerAngles.y;
        //    //MovingTarget.transform.eulerAngles = angle;
        //}
        if (IsFollowingVRCamera)
        {
            var camPos = VRCamera.position;
            float y = transform.position.y;

            y += VRCamera.position.y - MovingTarget.transform.position.y;
            camPos.y = y;

            transform.position = camPos;
        }
    }

    public void StopAllUpdate()
    {
       // IsFollowingVRCamera = false;
        StopCoroutine(RotatePlayer());
    }
}
