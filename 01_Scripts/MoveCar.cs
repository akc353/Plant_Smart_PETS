using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCar : MonoBehaviour {
    public float speed = 5f;
    public int line;

	void Update () {
		transform.position -= Time.deltaTime * speed *transform.up;
	}
}
