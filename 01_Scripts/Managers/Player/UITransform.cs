using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITransform : MonoBehaviour
{
    public UITransform 밑에(string name)
    {
        name = name.Replace(" ", "");
        //Debug.Log(name + " : 검색");
        UITransform tran = transform.Find(name).GetComponent<UITransform>();
        return tran;
    }
    public UITransform 밑(string name)
    {
        name = name.Replace(" ", "");
        //Debug.Log(name + " : 검색");
        UITransform tran = transform.Find(name).GetComponent<UITransform>();
        return tran;
    }

    public UITransform 밑에(int index)
    {
        UITransform tran = transform.GetChild(index).GetComponent<UITransform>();

        if (tran == null)
        {
            //Debug.Log(name + " : 없음");
        }

        return tran;
    }

    public UITransform 밑에다켜()
    {
        for (int ci = 0; ci < this.transform.childCount; ci++)
        {
            this.transform.GetChild(ci).gameObject.SetActive(true);
        }

        return this;
    }

    public UITransform 밑에다꺼()
    {
        for (int ci = 0; ci < this.transform.childCount; ci++)
        {
            this.transform.GetChild(ci).GetComponent<UITransform>().꺼();
        }

        return this;
    }

    public UITransform 밑에다페이드아웃()
    {
        for (int ci = 0; ci < this.transform.childCount; ci++)
        {
            this.transform.GetChild(ci).gameObject.SetActive(false);

            if (this.transform.GetChild(ci).gameObject.activeSelf)
            {
                this.transform.GetChild(ci).GetComponent<UITransform>().페이드아웃();
            }
        }

        return this;
    }

    public UITransform 켜()
    {
        this.gameObject.SetActive(true);

        Image i = this.transform.GetComponent<Image>();
        if (i != null)
        {
            Color c = i.color;
            c.a = 1.0f;
            i.color = c;
        }

        return this;
    }

    public UITransform 꺼()
    {
        this.gameObject.SetActive(false);

        Image i = this.transform.GetComponent<Image>();
        if (i != null)
        {
            Color c = i.color;
            c.a = 1.0f;
            i.color = c;
        }

        return this;
    }

    public UITransform 하나만켜()
    {
        GameObject ob = transform.parent.gameObject;
        for (int ci = 0; ci < ob.transform.childCount; ci++)
        {
            var obs = ob.transform.GetChild(ci).GetComponent<UITransform>().꺼();
        }

        this.gameObject.SetActive(true);

        Image i = this.transform.GetComponent<Image>();
        Color c = i.color;
        c.a = 1.0f;
        i.color = c;

        return this;
    }

    public UITransform 하나만켰다끔(float time = 3.0f)
    {
        GameObject ob = transform.parent.gameObject;
        for (int ci = 0; ci < ob.transform.childCount; ci++)
        {
            var obs = ob.transform.GetChild(ci).GetComponent<UITransform>().꺼();
        }

        this.gameObject.SetActive(true);

        Image i = this.transform.GetComponent<Image>();
        Color c = i.color;
        c.a = 1.0f;
        i.color = c;

        this.gameObject.SetActive(true);
        StartCoroutine(자동끔(time));

        return this;
    }

    public IEnumerator 자동끔(float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }

    public void 하나만페이드인(float time = 0.5f)
    {
        GameObject ob = transform.parent.gameObject;
        for (int ci = 0; ci < ob.transform.childCount; ci++)
        {
            var obs = ob.transform.GetChild(ci).GetComponent<UITransform>().꺼();
        }

        this.gameObject.SetActive(true);
        StartCoroutine(Co_FadeInImage(time));
    }

    public void 페이드인(float time = 0.5f)
    {
        this.gameObject.SetActive(true);
        StartCoroutine(Co_FadeInImage(time));
    }

    public void 페이드아웃(float time = 0.3f)
    {
        this.gameObject.SetActive(true);
        StartCoroutine(Co_FadeOutImage(time));
    }

    public void 페이드인아웃(float inTime = 0.4f, float time = 2.0f, float outTIme = 0.4f)
    {
        this.gameObject.SetActive(true);
        StartCoroutine(Co_FadeInOutImage(inTime, time, outTIme));
    }

    public void 딜레이후하나만페이드인아웃(float delay = 0.5f, float inTime = 0.5f, float time = 2.5f, float outTIme = 0.5f)
    {
        GameObject ob = transform.parent.gameObject;
        for (int ci = 0; ci < ob.transform.childCount; ci++)
        {
            var obs = ob.transform.GetChild(ci).GetComponent<UITransform>().꺼();
        }

        StartCoroutine(Co_FadeInOutImage(delay, inTime, time, outTIme));
    }

    private IEnumerator Co_FadeInOutImage(float delay, float inTime, float time, float outTime)
    {
        yield return new WaitForSeconds(delay);
        this.gameObject.SetActive(true);
        StartCoroutine(Co_FadeInImage(inTime));
        yield return new WaitForSeconds(inTime + time);
        StartCoroutine(Co_FadeOutImage(outTime));
    }

    public void 하나만페이드인아웃(float inTime = 0.5f, float time = 2.5f, float outTIme = 0.5f)
    {
        GameObject ob = transform.parent.gameObject;
        for (int ci = 0; ci < ob.transform.childCount; ci++)
        {
            var obs = ob.transform.GetChild(ci).GetComponent<UITransform>().꺼();
        }

        this.gameObject.SetActive(true);
        StartCoroutine(Co_FadeInOutImage(inTime, time, outTIme));
    }

    private IEnumerator Co_FadeInOutImage(float inTime, float time, float outTime)
    {
        StartCoroutine(Co_FadeInImage(inTime));
        yield return new WaitForSeconds(inTime + time);
        StartCoroutine(Co_FadeOutImage(outTime));
    }

    private IEnumerator Co_FadeOutImage(float time)
    {
        Image i = this.transform.GetComponent<Image>();
        Color c = i.color;
        float timer = 0;

        do
        {
            timer += Time.deltaTime;
            c.a = 1 - Mathf.Clamp(timer / time, 0f, 1f);
            i.color = c;

            yield return null;
        } while (timer < time);

        this.gameObject.SetActive(false);
    }

    private IEnumerator Co_FadeInImage(float time)
    {
        Image i = this.transform.GetComponent<Image>();
        Color c = i.color;
        float timer = 0;

        do
        {
            timer += Time.deltaTime;

            c.a = Mathf.Clamp(timer / time, 0f, 1f);
            i.color = c;

            yield return null;

        } while (timer < time);
    }

    public void GreenCompleteEffect(GameObject ob)
    {
        StartCoroutine(PlayEffect_Co(ob));
    }

    public IEnumerator PlayEffect_Co(GameObject ob)
    {
        ob.GetComponent<cakeslice.Outline>().color = 1;
        ob.GetComponent<cakeslice.Outline>().eraseRenderer = false;
        ob.GetComponent<cakeslice.Outline>().enabled = true;

        yield return new WaitForSeconds(0.8f);

        ob.GetComponent<cakeslice.Outline>().color = 0;
        ob.GetComponent<cakeslice.Outline>().eraseRenderer = true;
        ob.GetComponent<cakeslice.Outline>().enabled = false;
    }
}