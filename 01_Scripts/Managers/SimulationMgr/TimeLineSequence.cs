using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TimeLineSequence : GameMode
{
    float playTime = 0;
    float sequenceEndTime = 0;
    int nowPlayActorIndex = 0;

    //public void Init()
    //{
    //    if (actorDic.Count == 0) return;

    //    actorList = actorDic.Values.OrderBy(x => x.OffsetTime).ToList();
    //    sequenceEndTime = actorList[actorList.Count - 1].Length + actorList[actorList.Count - 1].OffsetTime;
    //}

    //public void UpdateGuestMethod()
    //{
    //    playTime += Time.deltaTime;

    //    if (!CheckActorEventTime(playTime))
    //    {
    //        Director.Inst.ExecuteCommand(CommandType.SequenceStop, this.SequenceName);
    //    }
    //}

    //public bool CheckActorEventTime(float SequenceTime)
    //{
    //    if (actorList.Count == 0) return false;

    //    if (nowPlayActorIndex == actorList.Count)
    //    {
    //        if (sequenceEndTime <= SequenceTime)
    //            return false;
    //    }
    //    else if (actorList[nowPlayActorIndex].OffsetTime <= SequenceTime)
    //    {
    //        actorList[nowPlayActorIndex].PlayAction();
    //        nowPlayActorIndex++;
    //    }

    //    return true;
    //}

    //override public void Play()
    //{
    //Debug.Log(SequenceName + " : Start");
    //Connect(UpdateGuestMethod);
    //}

    //override public void Stop()
    //{
    //Debug.Log(SequenceName + " : Stop");
    //DisConnect(UpdateGuestMethod);

    //    playTime = 0;
    //    nowPlayActorIndex = 0;
    //}
}