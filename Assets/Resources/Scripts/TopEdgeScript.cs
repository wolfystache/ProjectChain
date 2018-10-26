using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopEdgeScript : MonoBehaviour {

    // Use this for initialization
    private Vector2 speed;
	void Start () {
        speed = new Vector2();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void FixedUpdate()
    {
        Rigidbody2D rgdBdy = GetComponent<Rigidbody2D>();
        rgdBdy.MovePosition(rgdBdy.position + speed);
    }

    public void SetSpeed(Vector2 speed)
    {
        this.speed = speed;
    }
    public Vector2 GetSpeed ()
    {
        return speed;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject; 

        if (obj.CompareTag("Player"))
        {
      //      Debug.Log("Player is atop platform");
        //    Debug.Log("Collision name = " + collision.gameObject.name);

      //      transform.parent.GetComponent<ChainableController>().MovePlayer(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;

        if (obj.CompareTag("Player"))
        {
            Debug.Log("Player has left platform");
            Debug.Log("Collision name = " + collision.gameObject.name);
       //     transform.parent.GetComponent<ChainableController>().MovePlayer(false);
        }
    }
}
