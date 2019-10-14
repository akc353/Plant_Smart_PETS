using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replay : GameMode {

    const float FadeInTime = 0.5f;
    const float FadeTime = 2.5f;
    const float FadeOutTime = 0.5f;

    public override IEnumerator StartGame()
    {
        LeapHandMgr.Inst.ChangePlayerHand(DefineHand.LEFT_Hand_Default,DefineHand.RIGHT_Hand_Default);
        //PlayerMgr.Inst.FadeInUIImage("교육완료", 1f);
  
      //  yield return StartCoroutine(SoundMgr.Inst.PlayTTS(DefineSound.이상으로안전보건공단VR체험을모두마치겠습니다감사합니다));

       // PlayerMgr.Inst.FadeOutUIImage("교육완료", 1f);
        //SoundMgr.Inst.Play(DefineSound.교육확인효과음1, 0.7f);

        yield return new WaitForSeconds(1f);

      //  PlayerMgr.Inst.FadeInUIImage("학습다시하기2", 1f);

        LeapHandMgr.Inst.GetRightHand(1).Get<FingerDetection>().SubscribeEnterEvent("학습다시하기2", () =>
        {
           // NetworkMgr.Inst.GlovesRightVive();
            LeapHandMgr.Inst.GetRightHand(1).Get<FingerDetection>().Clean();
            LeapHandMgr.Inst.GetLeftHand(1).Get<FingerDetection>().Clean();

            MoveNextPoint();
        });
        LeapHandMgr.Inst.GetLeftHand(1).Get<FingerDetection>().SubscribeEnterEvent("학습다시하기2", () =>
        {
           // NetworkMgr.Inst.GlovesLeftVive();
            LeapHandMgr.Inst.GetRightHand(1).Get<FingerDetection>().Clean();
            LeapHandMgr.Inst.GetLeftHand(1).Get<FingerDetection>().Clean();

            MoveNextPoint();
        });

        yield return StartCoroutine(CommonWaitPoint());
      //  SoundMgr.Inst.Play(DefineSound.확인UI버튼클릭, 1.0f); //도로롱으로 변경

        GameObject vrorigin = GameObject.Find("VROrigin");
        vrorigin.GetComponent<Animator>().SetBool("exit",true);
      //  vrorigin.GetComponent<Animator>().SetBool("exit", false);
        vrorigin.GetComponent<Animator>().enabled = false;

        //  PlayerMgr.Inst.FadeOutUIImage("학습다시하기", 1f);
     //   PlayerMgr.Inst.FadeOutUIImage("학습다시하기2", 0f);
      //  PlayerMgr.Inst.FadeOutUIImage("교육완료", 0f);
      //  PlayerMgr.Inst.FadeOutUIImage("Circle", 0f);
        yield return new WaitForSeconds(1f);
    }
}
