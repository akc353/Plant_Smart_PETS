using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//편한대로 수정 사용 가능.
public enum eMOVINGARROW
{
    DIR_X,
    DIR_Y,
    DIR_Z
}

public class MovingArrow : MonoBehaviour
{
    public eMOVINGARROW mMovingArrow;
   
    private float mTime = 0;
    public float mScaler = 0.06f;
    public Coroutine mSoundCo;

    private void Update()
    {
        mTime += Time.deltaTime / 0.45f;
   
        //포지션 무브
        switch (mMovingArrow)
        {
            case eMOVINGARROW.DIR_X:
                float dirX = Mathf.Sin(mTime) * mScaler;
                this.transform.localPosition = new Vector3(dirX, 0, 0);
                break;

            case eMOVINGARROW.DIR_Y:
                float dirY = Mathf.Sin(mTime) * mScaler;
                this.transform.localPosition = new Vector3(0, dirY, 0);
                break;

            case eMOVINGARROW.DIR_Z:
                float dirZ = Mathf.Sin(mTime) * mScaler;
                this.transform.localPosition = new Vector3(0, 0, dirZ);
                break;
        }
    }


    public void StartArrowSound()
    {
        if (mSoundCo != null)
        {
            StopCoroutine(mSoundCo);
        }
        mSoundCo = StartCoroutine(ArrowSound());
    }


    private IEnumerator ArrowSound()
    {
        SoundMgr.Inst.Play(DefineEffect.좌우깜빡이는화살표소리, 0.6f);
        yield return new WaitForSeconds(1.4f);
    }
}
