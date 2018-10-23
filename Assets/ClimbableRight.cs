using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableRight : MonoBehaviour {

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)

    {
        Debug.Log("Collision detected");
        GameObject collider = collision.gameObject;

        //  Debug.Log(collision.gameObject.tag);
        if (collider.CompareTag("Player"))
        {
            Debug.Log("Player is touching chainable object");
            if (!collider.GetComponent<ArtrobotController>().IsFacingRight() && 
                collider.GetComponent<ArtrobotController>().IsTravelingHoriz() == -1)
            {
                Debug.Log("Climbing Right Climbable");
                collider.GetComponent<ArtrobotController>().SetClimbing(true, transform.parent.gameObject);


                collider.GetComponent<ArtrobotController>().SetClimbingCollider(
                    GetComponent<BoxCollider2D>());
                collider.GetComponent<ArtrobotController>().AimReset();
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
  //      Debug.Log("Collision detected");
        GameObject collider = collision.gameObject;

        //if (collider.CompareTag("ChainHead"))
        //{
        //    GameObject ChainGroup = collider.transform.parent.gameObject;
        //    ChainGroup.GetComponent<ChainController>().SetPlatformSpeed(transform.parent.GetComponent<
        //        ChainableController>().GetSpeed());

        //}
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;

        if (collider.CompareTag("Player"))
        {
            Debug.Log("Player is leaving chainable");
            transform.parent.GetComponent<
                ChainableController>().MovePlayer(false);
            collider.GetComponent<ArtrobotController>().SetClimbing(false, transform.parent.gameObject);

        }
    }
}

