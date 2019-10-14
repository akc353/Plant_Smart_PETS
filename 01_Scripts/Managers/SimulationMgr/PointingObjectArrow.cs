using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointingObjectArrow : MonoBehaviour
{
    [SerializeField]
    GameObject[] arrows;
    int length;

    Dictionary<string, int> arrowDictionary;

    private void Awake()
    {
        length = transform.childCount;
        arrows = new GameObject[length];
        arrowDictionary = new Dictionary<string, int>();

        for (int i = 0; i < length; i++)
        {
            GameObject temp = transform.GetChild(i).gameObject;
            arrows[i] = temp;
            arrowDictionary[temp.name] = i;
        }
    }

    public void SetArrow(string name, bool enable)
    {
        int index = arrowDictionary[name];
        arrows[index].SetActive(enable);

        if (enable)
        {
            //if (soundSystem == null)
            //{
            //      
            //}

            //soundSystem.Play(DefineSound.좌우깜빡이는화살표소리);
            StartCoroutine(RingForTimes());
        }
    }

    IEnumerator RingForTimes()
    {
        for (int i = 0; i < 3; i++)
        {
            //soundSystem.Play(DefineSound.좌우깜빡이는소리, eSOUNDTYPE.EFFECT);
            yield return new WaitForSeconds(1.77f);
        }
    }
}
