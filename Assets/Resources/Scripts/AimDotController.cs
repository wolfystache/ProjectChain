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

    // Use this for initialization
    void Start () {
        dotCount = 0;
        originalPosition = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {

        if (stillAiming)
        {
            //Debug.Log("Arm Position = " + transform.parent.position);
            //Debug.Log("Dot Position = " + transform.position);
            //Debug.Log("Dot Rotation = " + transform.rotation);
        }
        int numChild = transform.childCount;
        //     Debug.Log("stillAiming = " + stillAiming);
    //    Debug.Log("DotCount = " + dotCount);
    //    Debug.Log("Position = " + transform.position);
   //     Debug.Log("WasColliding = " + wasColliding);
        LayerMask mask = LayerMask.GetMask("Ground", "Enemy");
        Vector2 aimDir = player.GetComponent<ArtrobotController>().GetAimDir();
        if (numChild > 0)
        {
            //  Debug.DrawRay(transform.position, aimDir, Color.black);
            Vector3 furthSightPos = transform.GetChild(numChild - 1).position;
            float length = (furthSightPos - transform.position)
                .magnitude;
            //       Debug.Log("Length = " + length);
            RaycastHit2D []hit = Physics2D.RaycastAll(
                    transform.position, aimDir, length, mask);
            if (hit.Length != 0) {
         //       Debug.Log("Hit size = " + hit.Length);
                if (hit[0])
                {
                    foreach (RaycastHit2D h in hit)
                    {
             //           Debug.Log("h pos = " + h.point);
                    }
       //             Debug.Log("Raycast hit a collider");
                    Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y,
                        0), aimDir);


                    //Debug.Log("Ray Distance = " +
                    //    hit[0].distance);
                    //   Debug.Log(hit.rigidbody.name);
                    Vector3 point = hit[0].point;
                    //Debug.Log("Point pos = " + point);
                    //Debug.Log("Furthest Ball = " + furthSightPos);
                    //Debug.Log("Point/Furthest ball = " +
                    //        (point - furthSightPos).magnitude);
                    if (!wasColliding)
                    {
                        wasColliding = true;
                        stillDrawing = false;                     

                    }
                    if ((point - furthSightPos).magnitude > 3)
                    {

                        Debug.Log("Large Distance resetting sights");
                        StopSights();
                        StartSights();
                    }
                }

            }
            else
            {
                if (wasColliding)
                {
                    Debug.Log("Redrawing balls, no longer Raycasting");
                    stillDrawing = true;
                    wasColliding = false;
                    
                }
            }
        }
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
        while (dotCount < 60 && stillAiming)
        { 
            if (!stillDrawing)
            {
                yield return new WaitUntil(() => stillDrawing);
            }
        //    player.GetComponent<ArtrobotController>().GetAimAngle();
            Vector2 aimDir = player.GetComponent<ArtrobotController>().GetAimDir();
            Debug.Log("AimDir = " + aimDir);


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
            aimDir *= 0.25f;
            Vector3 nextDotPosition = new Vector3(nextPosition.x + aimDir.x, 
                nextPosition.y + aimDir.y);
            GameObject dot = Instantiate(AimDot, nextDotPosition, transform.rotation);
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
            yield return new WaitForSeconds(0.00005f);
        }
        stillDrawing = false;
      //  yield return new WaitUntil(() => stillAiming == false);
        
        
    }
}
