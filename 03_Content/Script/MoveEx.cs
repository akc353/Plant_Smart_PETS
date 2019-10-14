using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEx : MonoBehaviour {

    int speed = 1;
    public bool isKeyboard = true;
    // Update is called once per frame
    Vector3 temp;
    bool isSit;
    public float sitHeight;

    void Update()
    {

        moveObject();

        if (isKeyboard)
        {
            KeyboardInput();
        }
    }



    void moveObject()
    {

        float keyHorizontal = Input.GetAxis("Horizontal");

        float keyVertical = Input.GetAxis("Vertical");



        transform.Translate(Vector3.right * speed * Time.smoothDeltaTime * keyHorizontal, Space.Self);

        transform.Translate(Vector3.forward * speed * Time.smoothDeltaTime * keyVertical, Space.Self);



      


    }    
    void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSit = !isSit;

            temp = transform.localPosition;
            if (isSit)
            {
                temp.y = sitHeight;
            }
            else
            {
                temp.y = 0f;
            }
            transform.localPosition = temp;
        }
    }
}
