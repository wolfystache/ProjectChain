using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    public const string standing = "standing", falling = "falling", idle = "idle", jumping = "jumping", 
        running = "running";
    public string state;
    public string substate = idle;
    public Physics characPhysics;
    public List<Vector2> prevPos = new List<Vector2>();
    public bool isFacingRight;
    public bool justTurnedAround = false;
    public Rigidbody2D rgdBdy; 
    public Vector2 localOffset;


    // Use this for initialization
    protected virtual void Awake () {

        characPhysics = new Physics(name);
        substate = idle; 
        if (GetComponent<Rigidbody2D>() != null)
        {
            rgdBdy = GetComponent<Rigidbody2D>();
        }
    }

    public virtual void FixedUpdate()
    { 

        localOffset = new Vector2();

        

        

        switch (state)
        {
            case falling:
                localOffset += characPhysics.Gravity(transform.position);
                break;

            default:
                break;
        }  

        if (GetComponent<Rigidbody2D>() == null)
        {
            transform.position += new Vector3(localOffset.x, localOffset.y, 0);
        }
        
    }

    // Update is called once per frame
    void Update () {
		
	} 

    public virtual void LateUpdate()
    {
        if (prevPos.Count == 2)
        {
            prevPos.RemoveAt(0);
            prevPos.Add(transform.position);
        }
        else
        {
            prevPos.Add(transform.position);
        }
    }

    public virtual void SetState(string state)
    {
        this.state = state;
    }

    public string GetState()
    {
        return state;
    }

    public int IsTravelingHoriz() {

        if (prevPos.Count >= 2)
        {
            if (Mathf.Approximately(transform.position.x, prevPos[0].x))
            {
                return 0;
            }

            // Return 1 if traveling right
            if (transform.position.x > prevPos[0].x)
            {
                return 1;
            }
            //Return -1 if traveling left
            else if (transform.position.x < prevPos[0].x)
            {
                return -1;
            }
        }

        // Return 0 if not moving at all vertically
        return 0;
    }

    public int IsTravelingVert()  {

        if (prevPos.Count >= 2)
        {

            if (Mathf.Approximately(transform.position.y, prevPos[0].y))
            {
                return 0;
            }

            // Return 1 if traveling right
            if (transform.position.y > prevPos[0].y)
            {
                return 1;
            }
            //Return -1 if traveling left
            else if (transform.position.y < prevPos[0].y)
            {
                return -1;
            }
        }
        return 0;

    }

    public void toggleDir()
    {
        isFacingRight = !isFacingRight;
    }

    public virtual void TurnAround(int colliderChoice)
    {
        Debug.Log("Turning around");
        StartCoroutine("JustTurned");
        
        //    aimDir = Vector2.Scale(aimDir, new Vector2(-1, 1));
        //  Debug.Log("aimDir = " + aimDir);
        isFacingRight = !isFacingRight;
        if (!GetComponents<BoxCollider2D>()[0].enabled)
        {
            GetComponents<BoxCollider2D>()[0].enabled = true;
        }

        if (colliderChoice == 3)
        {
            float xMin = GetComponents<BoxCollider2D>()[0].bounds.min.x;
            Debug.Log("xMin = " + xMin);
            float xMax = GetComponents<BoxCollider2D>()[2].bounds.max.x;
            Debug.Log("xMax = " + xMax);
            float xAverage = (xMin + xMax) / 2;
            float yCenter = GetComponents<BoxCollider2D>()[3].bounds.center.y;
            Debug.Log("Turn pivot = " + xAverage);
            transform.RotateAround(new Vector3(xAverage, yCenter, 0), Vector3.up, 180.0f);
        }
        else
        {
            transform.RotateAround(GetComponents<BoxCollider2D>()
                [colliderChoice].bounds.center, Vector3.up, 180.0f);
        }
    }

    public virtual void SetFallState(bool hasPhysics)
    {
        if (!hasPhysics)
        {
            float fallTime = Time.realtimeSinceStartup;
            characPhysics.StartPhysics(fallTime, transform.position, 0, 0, 1);

        }
        
        Debug.Log("Starting to Fall");
        //    anim.SetBool("IsFalling", true);
        SetState(falling);

    }

    IEnumerator JustTurned()
    {
        justTurnedAround = true;
        yield return new WaitForSeconds(0.01f);
        justTurnedAround = false;
    }


    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        
    } 


    public virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 11 && !justTurnedAround)
        {
            Collider2D feetCollider = GetComponents<Collider2D>()[0];
            Debug.Log("Leaving Ground");
            if ((state.Equals(standing) || state.Equals(idle) || state.Equals(running)) && !substate.Equals("chaining") 
                && collision.gameObject.layer == 12)
            {

                if (feetCollider.bounds.min.y >= collision.collider.bounds.max.y)
                {
                    Debug.Log("Setting state to fall");
                    //int layerCount;
                    //CollideMap.TryGetValue(layer, out layerCount);
                    //layerCount--;
                    //CollideMap.Remove(collisionName);
                    //CollideMap.Remove(layer);
                    //CollideMap.Add(layer, layerCount);
                    SetFallState(false);
                }
            }
        }
    }
}
