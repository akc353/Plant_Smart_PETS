using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignPanelSystem : MonoBehaviour
{
    public bool rotate = false;
    public GameObject rotateDummy;
    public GameObject Hand;
    public GameObject Panel;
    public GameObject OUT;
    public Coroutine co;

    private void Awake()
    {
        Hand = transform.GetChild(0).GetChild(0).gameObject;
        rotateDummy = transform.GetChild(1).gameObject;
        Panel = rotateDummy.transform.GetChild(0).gameObject;
        OUT = rotateDummy.transform.GetChild(1).gameObject;

        Clean();
    }

    private void Update()
    {
        if (rotate)
        {
            rotateDummy.transform.Rotate(Vector3.up, Time.deltaTime * 180, Space.Self);
        }
    }

    public void StartAnim()
    {
        co = StartCoroutine(StartScale());
    }

    public IEnumerator StartScale()
    {
        this.GetComponent<BoxCollider>().enabled = true;
        rotateDummy.transform.localScale = Vector3.zero;
        rotateDummy.SetActive(true);
        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime;

            yield return new WaitForFixedUpdate();

            rotateDummy.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time);
        }

        rotateDummy.transform.localScale = Vector3.one;
        // rotate = true;
    }

    public void Clean()
    {
        if (co != null)
        {
            StopCoroutine(co);
        }

        Hand.GetComponent<MeshRenderer>().enabled = false;
        Hand.GetComponent<Collider>().enabled = false;
        Hand.GetComponent<cakeslice.Outline>().enabled = false;
        Hand.GetComponent<cakeslice.Outline>().eraseRenderer = true;

        Panel.GetComponent<MeshRenderer>().enabled = false;
        Panel.GetComponent<Collider>().enabled = false;
        Panel.GetComponent<cakeslice.Outline>().enabled = false;
        Panel.GetComponent<cakeslice.Outline>().eraseRenderer = true;

        OUT.GetComponent<MeshRenderer>().enabled = false;
        //OUT.GetComponent<Collider>().enabled = false;
        OUT.GetComponent<cakeslice.Outline>().enabled = false;
        OUT.GetComponent<cakeslice.Outline>().eraseRenderer = true;
    }
}