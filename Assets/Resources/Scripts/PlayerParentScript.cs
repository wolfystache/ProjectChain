using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParentScript : MonoBehaviour {

    float move;
    // Use this for initialization
	void Start () {
        move = 0;
	}
	
	// Update is called once per frame
	void Update () {

        move = Input.GetAxis("Horizontal");
    }
    private void FixedUpdate()
    {
        Vector2 rigidbody2DVel = GetComponent<Rigidbody2D>().velocity;
        GetComponent<Rigidbody2D>().velocity = new Vector2(move * 1.0f, rigidbody2DVel.y);
    }
}
