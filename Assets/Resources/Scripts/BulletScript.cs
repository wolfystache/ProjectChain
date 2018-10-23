using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletScript : MonoBehaviour {

   


	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bullet entered collision");
        if (collision.gameObject.layer != 13)
        {
            Destroy(gameObject);
        }
    }


}
