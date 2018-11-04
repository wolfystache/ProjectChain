using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainScript : MonoBehaviour {

    private GameObject player;
    private bool isPulling;
    private Vector2 collisionPoint = new Vector2();

    // Use this for initialization
    void Awake()
    {
       
    }
    // Update is called once per frame


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;
        if (transform.parent.GetComponent<ChainController>().GetState().Equals("shooting"))
        {


            if (collider.CompareTag("Chainable"))
            {

                GameObject ChainGroup = transform.parent.gameObject;
                //     Debug.Log("Name = " + collider.transform.parent.gameObject.name); 
                Collider2D collide = GetComponent<Collider2D>();
                bool isClimbable = false;
                LayerMask mask = Physics2D.GetLayerCollisionMask(11);
                if (collide.IsTouchingLayers(mask))
                {
                    isClimbable = true;
                }

                ChainGroup.GetComponent<ChainController>().StruckChainable(collider.transform.parent.gameObject,
                    isClimbable); 

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

        // Fall off if swing hits enemy
        if (collider.layer == 13)
        {
            if (transform.parent.GetComponent<ChainController>().GetState().Equals("swinging"))
            {
                ArtrobotController.player.FallOffSwing();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }



}
