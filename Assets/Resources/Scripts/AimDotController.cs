using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AimDotController : MonoBehaviour {

    public GameObject AimDot;
    public GameObject player;
    public Transform ClimbAimSpawn;

    private int dotCount;
    private bool stillAiming = false;
    private bool stillDrawing = false;
    private Vector3 nextPosition;
    private bool wasColliding = false;
    private Vector3 originalPosition;
    private float ballLength;
    private GameObject aimBall;
    private int aimBallCount;

    private const float AIM_SPACE = 0.25f;
    private const int TOTAL_BALL_COUNT = 60;

    // Use this for initialization
    void Start () {
        dotCount = 0;
        originalPosition = transform.localPosition;
        aimBall = (GameObject)Resources.Load("Sprites/Prefabs/AimBall"); 

        if (aimBall != null)
        {
            ballLength = aimBall.GetComponent<BoxCollider2D>().bounds.size.x;
        }
        else
        {
            Debug.Log("Cannot find aimBall object");
        }

	}
	
	// Update is called once per frame
	void Update () {

        if (stillAiming)
        {
            //Debug.Log("Arm Position = " + transform.parent.position);
            //Debug.Log("Dot Position = " + transform.position);
            //Debug.Log("Dot Rotation = " + transform.rotation);

            int numChild = transform.childCount;
            //     Debug.Log("stillAiming = " + stillAiming);
            //    Debug.Log("DotCount = " + dotCount);
            //    Debug.Log("Position = " + transform.position);
            //     Debug.Log("WasColliding = " + wasColliding);
            LayerMask mask = LayerMask.GetMask("Ground", "Enemy");
            Vector2 aimDir = player.GetComponent<ArtrobotController>().GetAimDir();
            //    if (numChild > 0)
            //   {
            //  Debug.DrawRay(transform.position, aimDir, Color.black);
            //Vector3 furthSightPos = transform.GetChild(numChild - 1).position;
            //float length = (furthSightPos - transform.position)
            //    .magnitude;
            //       Debug.Log("Length = " + length);
            RaycastHit2D[] hit = Physics2D.RaycastAll(
                    transform.position, aimDir, 100, mask);
            if (hit.Length != 0)
            {
                //       Debug.Log("Hit size = " + hit.Length);
                if (hit[0])
                {
                    Vector3 point = hit[0].point;
                    float aimDistance = (point - transform.position).magnitude;

                    Debug.Log("aimDistance = " + aimDistance);

                    Debug.Log("Point = " + point);

                    aimBallCount = Mathf.FloorToInt((aimDistance + AIM_SPACE) / (ballLength + AIM_SPACE));
                    Debug.Log("AimBallCount = " + aimBallCount);
                    if (aimBallCount > TOTAL_BALL_COUNT)
                    {
                        aimBallCount = TOTAL_BALL_COUNT;
                    }



                    //if (!wasColliding)
                    //{
                    //    wasColliding = true;
                    //    stillDrawing = false;                     

                    //}
                    //if ((point - furthSightPos).magnitude > 3)
                    //{

                    //    Debug.Log("Large Distance resetting sights");
                    //    StopSights();
                    //    StartSights();
                    //}
                }
                

            }
            else
            {
                Debug.Log("Not colliding with anything");
                aimBallCount = TOTAL_BALL_COUNT;

            }
            StopSights();
            StartSights();
        }


            //     }
            //else
            //{

            //    if (wasColliding)
            //    {
            //        Debug.Log("Redrawing balls, no longer Raycasting");
            //        stillDrawing = true;
            //        wasColliding = false;

            //    }
            //}
        
	}

    public void StartSights()
    {
        StartCoroutine("SpawnSights");
        

    }

    public void StopSights()
    {
        StopCoroutine("SpawnSights");
        stillAiming = false;
        stillDrawing = false;
        dotCount = 0;
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    IEnumerator SpawnSights()
    {
        stillAiming = true;
        stillDrawing = true;
        Debug.Log("Spawning Sights");
        dotCount = 0;
        if (player.GetComponent<ArtrobotController>().GetState()
            .Equals("climbing"))
        {
            transform.position = ClimbAimSpawn.position;
        }
        else
        {
            transform.localPosition = originalPosition;
        }
            nextPosition = transform.position;
        
  //      yield return new WaitForSeconds(0.1f);
        while (dotCount < aimBallCount)
        { 
            if (!stillDrawing)
            {
                yield return new WaitUntil(() => stillDrawing);
            }
        //    player.GetComponent<ArtrobotController>().GetAimAngle();
            Vector2 aimDir = player.GetComponent<ArtrobotController>().GetAimDir();
        //    Debug.Log("AimDir = " + aimDir);


            Quaternion rotate;
            if (dotCount != 0)
            {
                int count = transform.childCount;
                nextPosition = transform.GetChild(count - 1).position;
                rotate = transform.rotation;
            }
            else
            {
                rotate = transform.parent.rotation;
            }
            aimDir *= AIM_SPACE;
            Vector3 nextDotPosition = new Vector3(nextPosition.x + aimDir.x, 
                nextPosition.y + aimDir.y);
            GameObject dot = Instantiate(aimBall, nextDotPosition, transform.rotation);
            dot.GetComponent<SpriteRenderer>().enabled = false;
            dot.transform.parent = transform;
            dot.GetComponent<SpriteRenderer>().enabled = true;
            //    
            if (dotCount == 0)
            {
                //Debug.Log("AimDir = " + aimDir);
                //Debug.Log("Dot Position = " + dot.transform.position);
                //Debug.Log("Dot Rotation = " + dot.transform.rotation);
            }

            
            dotCount++;
            
        }
        yield return null;
        stillDrawing = false;
        
        //  yield return new WaitUntil(() => stillAiming == false);


    }
}
