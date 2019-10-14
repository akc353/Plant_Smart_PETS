using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class OutlineAni : MonoBehaviour
{
    private OutlineEffect m_OutlineEffect;
    private bool pingPong = false;
    public float m_fFlashTime = 1f;

    private void Awake()
    {
        m_OutlineEffect = GetComponent<OutlineEffect>();
    }

    private void OnEnable()
    {
        Color c = m_OutlineEffect.lineColor0;
        c.a = 0;
        m_OutlineEffect.lineColor0 = c;
        m_OutlineEffect.UpdateMaterialsPublicProperties();
        pingPong = true;
    }

    private void OnDisable()
    {
        Color c = m_OutlineEffect.lineColor0;
        c.a = 0;
        m_OutlineEffect.lineColor0 = c;
        m_OutlineEffect.UpdateMaterialsPublicProperties();
    }

    void Update()
    {
        Color c = m_OutlineEffect.lineColor0;

        if (pingPong)
        {
            c.a += Time.deltaTime / m_fFlashTime;

            if (c.a >= 1)
                pingPong = false;
        }
        else
        {
            c.a -= Time.deltaTime/m_fFlashTime;

            if (c.a <= 0)
            {
                pingPong = true;
            }
        }

        c.a = Mathf.Clamp01(c.a);
        m_OutlineEffect.lineColor0 = c;
        m_OutlineEffect.UpdateMaterialsPublicProperties();
    }

}


