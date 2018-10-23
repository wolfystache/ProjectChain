using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceScript : MonoBehaviour {

    // Use this for initialization
    public GameObject Demo;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
          //  Debug.Log("")
           Demo.GetComponent<DemoScript>().EndDemo();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag + " stopped touching surface");
        if (collision.gameObject.CompareTag("Chainable"))
        {
            Destroy(collision.gameObject);
        }
    }
}
