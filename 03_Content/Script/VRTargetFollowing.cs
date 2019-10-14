using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRTargetFollowing : MonoBehaviour
{
    public GameObject OriginTarget; //움직이는 애 //VRCamera 상속으로 가도 될듯
    public GameObject InstTarget;

    public Transform Field; //VROrigin : 기준점이 되는 필드
    public Transform CameraDummy; //VRCamera 부모 : DeviceHeight
    public Transform VRCamera; //실제 VR카메라(보이는 기준) : 이걸 필드의 기준점과 맞춰줘야 함 

    public bool isFollow = false;

    //ob 캡슐
    public void PlayFall(GameObject ob, System.Action action)
    {
        enabled = true;
        isFollow = true;

        OriginTarget = ob;
        //OriginTarget.Get<Collider>().enabled = false;

        InstTarget = Instantiate<GameObject>(OriginTarget.gameObject);

        InstTarget.transform.position = OriginTarget.transform.position;
        InstTarget.transform.rotation = OriginTarget.transform.rotation;

        InstTarget.GetComponent<Animation>().Play();

       
        //InstTarget.Get<Rigidbody>().useGravity = true;
        //InstTarget.Get<Rigidbody>().isKinematic = false;
        // InstTarget.Get<Collider>().enabled = true;


        //InstTarget.Get<AutoDetect>().SubscribeEnterEvent("Main_Tile_Panel", () =>
        //{
        //    action();
        //    //NetworkMgr.Inst.NetworkObjectStop(false);
        //});
    }

    public void Rot()
    {
        if (InstTarget != null)
        {
            InstTarget.transform.rotation = new Quaternion();
        }
    }

    public void StopFall()
    {
        enabled = false;
        isFollow = false;

        InstTarget = null;

        Destroy(InstTarget);
    }

    void Update()
    {
        if (!isFollow)
        {
            // OriginTarget.transform.position = VRCamera.transform.position;
        }

        if (isFollow)
        {
            if (InstTarget != null)
            {
                VRFieldRotationUpdate();
                VRFieldPositionUpdate();
            }
        }
    }

    private void VRFieldPositionUpdate()
    {
        var interval = VRCamera.transform.position - Field.transform.position;

        Field.transform.position = InstTarget.transform.position;
        Field.transform.position -= interval;
    }

    private void VRFieldRotationUpdate()
    {
        CameraDummy.transform.rotation = InstTarget.transform.rotation;
    }
}
