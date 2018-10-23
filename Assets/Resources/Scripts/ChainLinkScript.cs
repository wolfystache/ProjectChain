using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLinkScript : MonoBehaviour {

    public Animator anim;
    public GameObject nextLink;
    public Transform linkSpawn;
    public GameObject player;
    float animLength;
    private float speed;
    private static float linkCount;
    bool isReturning;

    // Use this for initialization
	void Start () {
        isReturning = false; 
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        // GetComponent<Rigidbody2D>().velocity = new Vector2(3.2f, 0);
        bool isFacingRight = player.GetComponent<ArtrobotController>().IsFacingRight();

        if (linkCount == 7)
        {
            Debug.Log("Speed: " + speed);
          //  gameObject.GetComponentInParent<ChainController>().ChainReturn(0.15f);
            linkCount = 0;
        }

        // Debug.Log("Link Count = " + linkCount);

        Vector2 pos = GetComponent<Transform>().position;

        if (isFacingRight)
        {
                GetComponent<Transform>().position = new Vector2(pos.x + speed, pos.y);
        }
        else
        {
                GetComponent<Transform>().position = new Vector2(pos.x - speed, pos.y);

        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("ChainLink") == true)
        {
           // Debug.Log("Chain Link state"); 
        }
       else if (anim.GetCurrentAnimatorStateInfo(0).IsName("LinkCompleteState") == true)
        {
            //Debug.Log("Link Complete state");
        }
    }

    private void LateUpdate()
    {
       
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SpawnLink()
    {
        StartCoroutine("SpawnNextLink");
      //  StartCoroutine("ReturnTimer");

    }

    IEnumerator SpawnNextLink()
    {
        animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);
        linkCount++;
        // Debug.Log("Link Spawn position is " + linkSpawn.position);
        GameObject link = (GameObject)Instantiate(nextLink, linkSpawn.position, linkSpawn.rotation);
        link.transform.parent = gameObject.transform.parent;
        link.GetComponent<ChainLinkScript>().speed = speed;
        link.GetComponent<ChainLinkScript>().SpawnLink();

    }
    //IEnumerator ReturnTimer()
    //{
    //    yield return new WaitForSeconds(2.0f);
    //    gameObject.GetComponentInParent<ChainController>().ChainReturn(speed);
    //    isReturning = true;
    //}
}

