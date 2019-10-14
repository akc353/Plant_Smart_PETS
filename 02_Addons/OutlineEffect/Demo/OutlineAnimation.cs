using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

namespace cakeslice
{
    public class OutlineAnimation : MonoBehaviour
    {
        bool pingPong = false;
        public int flashNum;
        int tempNum = 0;
        

        void Update()
        {
            if (tempNum > flashNum)
            {
                tempNum = 0;
                this.enabled = false;
            }

            Color c = GetComponent<OutlineEffect>().lineColor0;

            if(pingPong)
            {
                c.a += Time.deltaTime;

                if (c.a >= 1)
                    pingPong = false;
            }
            else
            {
                c.a -= Time.deltaTime;

                if (c.a <= 0)
                {
                    pingPong = true;
                    tempNum++;
                }
            }

            c.a = Mathf.Clamp01(c.a);
            GetComponent<OutlineEffect>().lineColor0 = c;
            GetComponent<OutlineEffect>().UpdateMaterialsPublicProperties();
        }
    }
}