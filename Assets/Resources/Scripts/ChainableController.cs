using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainableController : MonoBehaviour {

    // Use this for initialization
   // private GameObject ChainGroup; 
    private Vector2 speed;
    private bool playerRiding = false;
    private GameObject player;
    private Vector2 lastPos;
    private int FixedCount;

	void Start () {
       // speed = new Vector2();
     //   ChainGroup = GameObject.Find("ChainGroup");
        player = GameObject.Find("Artrobot 1");
        lastPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
     //   Debug.Log("Change in Y Platform = " + (transform.position.y - lastPos.y));

    }
    private void FixedUpdate()
    {
        //     Debug.Log("Platform FixedCount = " + FixedCount); 

        //     Debug.Log("Platform Speed = " + speed.y);
      //  Debug.Log("Chainable IsPlayerRiding = " + playerRiding + " " + gameObject.name);

        if (speed.y > 0)
        {
        }
        //   Debug.Log("Platform fixed update time = " + Time.realtimeSinceStartup);
           
        FixedCount++;

        Rigidbody2D rgdBdy = GetComponent<Rigidbody2D>();
         rgdBdy.MovePosition(rgdBdy.position + speed);
        //transform.position = new Vector2(transform.position.x, 
        //   transform.position.y)+ speed;
        if (GetComponentInChildren<TopEdgeScript>() != null)
        {
            GetComponentInChildren<TopEdgeScript>().SetSpeed(speed);
        }
        Rigidbody2D plyRgdBdy = player.GetComponent<Rigidbody2D>();

     //   Debug.Log("Platform difference is  = " + (transform.position.y - lastPos.y));
     //   Debug.Log("Platform position = " + rgdBdy.position.y);

        if (playerRiding)
        {

            
            //   Debug.Log("Speed = " + speed.y);
            //  

            //        plyRgdBdy.MovePosition(plyRgdBdy.position + speed);

                //      Debug.Log("Player is riding platform");
                //      Debug.Log("Speed = " + speed);

                //      plyRgdBdy.velocity = speed;
                player.GetComponent<ArtrobotController>().SetRidingMotion(speed, gameObject);

                //       Debug.Log("Player's position = " + plyRgdBdy.position.y);
        }
    
    }
    private void LateUpdate()
    {

        Rigidbody2D rgdBdy = GetComponent<Rigidbody2D>();
        lastPos = transform.position;
    }

    public void SetSpeed(Vector2 speed)
    {
        this.speed = speed;
    } 

    public Vector2 GetSpeed() {
        return speed;
    } 

    public void MovePlayer(bool playerRiding)
    {
        this.playerRiding = playerRiding;

    //    player.GetComponent<ArtrobotController>().SetIsRiding(playerRiding);
     //   Debug.Log("Moving Player = " + playerRiding + " on " + name);
        if (playerRiding)
        {
            player.GetComponent<ArtrobotController>().SetIsRiding(playerRiding);
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collider = collision.gameObject;
     //   Debug.Log("Player Detected on Chainable Surface " + name);
        if (collider.CompareTag("Player"))
        {
            MovePlayer(true);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject collider = collision.gameObject;
        Debug.Log("Moving off of Chainable Surface " + name);
        if (collider.CompareTag("Player"))
        {
            string state = collider.GetComponent<ArtrobotController>().GetState();
            Debug.Log("State = " + state);
            if (state.Equals("jumping") || state.Equals("standing"))
            {
                MovePlayer(false);
                collider.GetComponent<ArtrobotController>().SetIsRiding(false);
            } 
            else if (state.Equals("chaining"))
            {
                MovePlayer(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;

        //  Debug.Log(collision.gameObject.tag);
        if (collider.CompareTag("ChainHead"))
        {
            if (collider.transform.parent.
                GetComponent<ChainController>().GetState().Equals("shooting"))
            {
            //    GameObject ChainGroup = collider.transform.parent.gameObject;
            //    ChainGroup.GetComponent<ChainController>().StruckChainable(collider);

                Debug.Log("Chain Detected on Chainable Surface");
            }


        }
        //else if (collider.CompareTag("Player"))
        //{

        //    Debug.Log("Player is touching chainable object");
        //    collider.GetComponent<ArtrobotController>().SetClimbing(true, gameObject);
        //    collider.GetComponent<ArtrobotController>().AimReset();
        //}
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;

        if (collider.CompareTag("ChainHead"))
        {
            GameObject ChainGroup = collider.transform.parent.gameObject;
            ChainGroup.GetComponent<ChainController>().SetPlatformSpeed(speed);

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;

        if (collider.CompareTag("Player"))
        {
            Debug.Log("Player is leaving chainable");
            MovePlayer(false);
            collider.GetComponent<ArtrobotController>().SetClimbing(false, gameObject);
            
        }
    }
}
