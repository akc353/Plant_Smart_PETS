using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : GameMode
{
  
    public override IEnumerator StartGame()
    {
        GameObject vrCamera = Get("VRCamera");
        UIMgr.Inst.FadeINOutUIImage(DefineUpUI.상단달비계VR센서동작확인);
        bool EyeCheck = false;
        DynamicUpdate ExamUc = new DynamicUpdate("ExamLookCheck", true,
          () =>
          {
              Ray ray = new Ray(vrCamera.transform.position, vrCamera.transform.forward);
              RaycastHit hit;

              if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("LookCheck")))
              {
                  if (hit.collider.gameObject.name.Equals("TestSceneCube"))
                  {
                      Debug.Log("봤다봤어");

                      return true;
                  }
              }
              return false;
          },
          () =>
          {
              EyeCheck = true;
          });

        ucList.Add(ExamUc);
        while (!EyeCheck) yield return null;
        yield return new WaitForSeconds(1f);


        UIMgr.Inst.FadeInUIImage("안전체험버튼", 1f);   //현재 이미지 없습니다. 맡은 프로젝트에 맡는 버튼ui 넣으세요.

        LeapHandMgr.Inst.GetRightHand(2).Get<FingerDetection>().SubscribeEnterEvent("안전체험버튼", () =>
        {
            // NetworkMgr.Inst.GlovesRightVive(); 
            MoveNextPoint();
        });
        LeapHandMgr.Inst.GetLeftHand(2).Get<FingerDetection>().SubscribeEnterEvent("안전체험버튼", () =>
        {
            // NetworkMgr.Inst.GlovesLeftVive();
            // SoundMgr.Inst.Play(DefineSound.확인UI버튼클릭, 1.0f);
            MoveNextPoint();
        });
        yield return StartCoroutine(CommonWaitPoint());
        LeapHandMgr.Inst.GetRightHand(2).Get<FingerDetection>().Clean();
        LeapHandMgr.Inst.GetLeftHand(2).Get<FingerDetection>().Clean();

        yield return StartCoroutine(SoundMgr.Inst.PlayEFFECTCo(DefineEffect.확인UI버튼클릭));
        UIMgr.Inst.FadeOutUIImage("안전체험버튼", 1f);


        yield return new WaitForSeconds(1f);

    }
}