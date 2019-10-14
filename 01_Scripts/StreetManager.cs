using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetManager : MonoBehaviour {
    public Transform StartPositionParent;
    Transform[] StartPosition;

    List<int> streetNum;

    private void Awake()
    {
        StartPosition = new Transform[StartPositionParent.childCount];

        for (int i = 0; i < StartPosition.Length; i++)
            StartPosition[i] = StartPositionParent.GetChild(i);
        streetNum = new List<int> { 0, 5, 7 };

        //이미 길에 있는 스트릿 넘버 빼주기
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Car"))
        {
            int preNum = other.gameObject.GetComponent<MoveCar>().line;

            streetNum.Add(other.gameObject.GetComponent<MoveCar>().line);

            int randomNum = Random.Range(0, 100);
            randomNum %= streetNum.Count;

            float y = other.gameObject.transform.position.y;
            var pos = StartPosition[streetNum[randomNum]].position;
            pos.y = y;

            other.gameObject.transform.position = pos;

            int postNum = streetNum[randomNum];
            streetNum.RemoveAt(randomNum);

            if (preNum < 4 && postNum > 3)
                other.transform.Rotate(new Vector3(0, 0, 180));
            else if (postNum < 4 && preNum > 3)
                other.transform.Rotate(new Vector3(0, 0, 180));
        }
    }
}
