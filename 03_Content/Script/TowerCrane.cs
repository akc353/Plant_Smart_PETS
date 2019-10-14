using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TowerCrane : CollectionableMonoBehaviour
{
    public void Update()
    {
        MoveTrolley();
        MoveHook();

        //MoveSmallTrolley();
        //MoveSmallHook();
    }

    public void RotateCrane()
    {
        var mech = Get("[P]Mechanism");
        var goalPoint = Get("[P]Goal");
        var angle = Vector3.Angle(mech.transform.right, goalPoint.transform.right);

        Debug.Log(mech.transform.localRotation.eulerAngles);

        if (angle > 1f)
        {
            mech.transform.LookAt(goalPoint.transform);
            Vector3 rot = mech.transform.localRotation.eulerAngles;
            mech.transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);
            //mech.transform.localRotation = Quaternion.Euler(rot.x, 90, 90);
        }
    }

    public void MoveTrolley()
    {
        var root = Get("[P]Root");
        var trolley = Get("Trolley");
        //var start = G("[P]TrolleyStart");
        //var end = G("[P]TrolleyEnd");

        // Goal 포인트를 받아서 높이를 루트만큼 낮춘 점과 루트사이의 거리를 측정합니다.
        var goal = Get("[P]Goal");
        var goalpos = goal.transform.position;
        goalpos = new Vector3(goalpos.x, root.transform.position.y, goalpos.z);  
        var rootpos = root.transform.position;
        var hookdeltapos = goalpos - rootpos;

        // 트롤리의 포지션을 받아와서 위치 값을 반영합니다.
        var trolleypos = trolley.transform.localPosition;
        trolleypos.x = hookdeltapos.magnitude;
        trolley.transform.localPosition = trolleypos;
    }

    //public void MoveSmallTrolley()
    //{
    //    var root = Get("[P]Root");
    //    var trolley = Get("Trolley");
    //    //var start = G("[P]TrolleyStart");
    //    //var end = G("[P]TrolleyEnd");
    //
    //    // Goal 포인트를 받아서 높이를 루트만큼 낮춘 점과 루트사이의 거리를 측정합니다.
    //    var goal = Get("[P]Goal");
    //    var goalpos = goal.transform.position;
    //    goalpos = new Vector3(goalpos.x, root.transform.position.y, goalpos.z);
    //    var rootpos = root.transform.position;
    //    var hookdeltapos = goalpos - rootpos;
    //
    //    // 트롤리의 포지션을 받아와서 위치 값을 반영합니다.
    //    var trolleypos = trolley.transform.localPosition;
    //    trolleypos.x = hookdeltapos.magnitude;
    //    trolley.transform.localPosition = trolleypos;
    //}

    public void MoveHook()
    {
        var goal = Get("[P]Goal");
        var hookani = Get("Hookblock_ani");
        var height = goal.transform.position.y;        

        var craneHeight = this.gameObject.transform.position.y;
        var craneHookAnimMin = 6.362549f + craneHeight;
        var craneHookAnimMax = 82.18607f + craneHeight;
        var craneIntervalLength = 75.823512f;

        if (height < craneHookAnimMin || craneHookAnimMax < height)
        {
            return;
        }
        else
        {
            var lerpHeight = (height - craneHookAnimMin) / craneIntervalLength;
            var time = Mathf.Lerp(0, 1.67f, (float)lerpHeight);
            hookani.Get<Animation>()["Take 001"].Reset();
            hookani.Get<Animation>().Play();
            hookani.Get<Animation>()["Take 001"].SetTime(1.67f - time);
            hookani.Get<Animation>().Play();
        }
    }

    //public void MoveSmallHook()
    //{
    //    var goal = Get("[P]Goal");
    //    var hookani = Get("Hookblock_ani");
    //    var height = goal.transform.position.y;
    //
    //    var craneHeight = this.gameObject.transform.position.y;
    //    var craneHookAnimMin = 6.362549f + craneHeight;
    //    var craneHookAnimMax = 82.18607f + craneHeight;
    //    var craneIntervalLength = 75.823512f;
    //
    //    if (height < craneHookAnimMin || craneHookAnimMax < height)
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        var lerpHeight = (height - craneHookAnimMin) / craneIntervalLength;
    //        var time = Mathf.Lerp(0, 1.67f, (float)lerpHeight);
    //        hookani.Get<Animation>()["Take 001"].Reset();
    //        hookani.Get<Animation>().Play();
    //        hookani.Get<Animation>()["Take 001"].SetTime(1.67f - time);
    //        hookani.Get<Animation>().Play();
    //    }
    //}

    public IEnumerator MoveUpCrane(int up)
    {
        var am = Get("hydraulic assembly_ani_AM");
        var cage = Get("Telescoping_cage_MAIN");

        am.Get<Animation>()["Take 001"].time = 0;
        am.Get<Animation>()["Take 001"].speed = 1.0f;

        am.Get<Animation>().Play();
        yield return new WaitForSeconds(am.Get<Animation>().clip.length + 0.3f);

        cage.Get<IgnoreParentPosition>().enabled = false;
        yield return null;

        am.Get<Animation>()["Take 001"].time = 0;
        am.Get<Animation>()["Take 001"].speed = 0.0f;
        am.Get<Animation>().Play();

        am.Get<Transform>().Translate(new Vector3(0, 0.53187f * up, 0), Space.World);
        yield return null;

        cage.Get<IgnoreParentPosition>().enabled = true;
        yield return null;
    }
}