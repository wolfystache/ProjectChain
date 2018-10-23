using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainScript : MonoBehaviour {

    private GameObject player;
    // Use this for initialization
    void Awake()
    {
        player = GameObject.Find("Artrobot 1");
    }
    // Update is called once per frame
    void FixedUpdate () {


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;
        if (transform.parent.GetComponent<ChainController>().GetState().Equals("shooting"))
        {
            if (collider.CompareTag("Chainable"))
            {

                GameObject ChainGroup = transform.parent.gameObject;
                //     Debug.Log("Name = " + collider.transform.parent.gameObject.name);
                ChainGroup.GetComponent<ChainController>().StruckChainable(collider.transform.parent.gameObject);


                Debug.Log("Chain Detected on Chainable Surface");
            }

            else
            {
                if (!(collider.CompareTag("Climbable") || collider.CompareTag("SurfaceShape")))
                {
                    GameObject ChainGroup = transform.parent.gameObject;
                    //     Debug.Log("Name = " + collider.transform.parent.gameObject.name);
                    ChainGroup.GetComponent<ChainController>().ChainReturn();

                }

                Debug.Log("Chain Detected on Non Chainable Surface");
            }
        } 

        if (collider.layer == 13)
        {
            if (transform.parent.GetComponent<ChainController>().GetState().Equals("swinging"))
            {
                player.GetComponent<ArtrobotController>().FallOffSwing();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Chain Head has collided with something");
        //if (!collision.gameObject.CompareTag("SurfaceShape"))
        //{
        //    GameObject ChainGroup = transform.parent.gameObject;
        //    //     Debug.Log("Name = " + collider.transform.parent.gameObject.name);
        //    ChainGroup.GetComponent<ChainController>().ChainReturn();
        //}
    }

}
