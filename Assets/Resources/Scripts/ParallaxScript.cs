using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScript : MonoBehaviour {

    Vector2 offset;
    public GameObject player;

    // Use this for initialization
	void Start () {
        offset = new Vector2(transform.position.x, transform.position.y);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Scroll()
    {
      //  transform.position = new Vector2(player.transform.position.x * -0.1f + offset.x, transform.position.y);
    }
    //public void ScrollLeft()
    //{
    //    transform.position = new Vector2(player.transform.position.x * 0.95f + offset.x, transform.position.y);

    //}
}
