using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEnemyController : Enemy {

    private Animator anim;
    private const string preJump = "preJump";
    

	// Use this for initialization
	public override void Start () {
        base.Start();
        anim = GetComponent<Animator>();
        state = idle;
        totalHealth = health = 2.0f;
        detectionRadius = 10.0f;
        damageCaused = -1;
      
        



	}
	
	// Update is called once per frame
	void Update () {
    //    Debug.Log("Spider color = " + GetComponent<Renderer>().material.color);
	}

    public override void FixedUpdate()
    {
        Debug.Log("Spider state = " + state);
        base.FixedUpdate();
        float playerDistance = (ArtrobotController.artTrans.position - transform.position).magnitude;

        
        



        Vector2 fixedPos = rgdBdy.position;
        switch (state)
        {
            case idle:
                if (stillAlive) { StartJump(); }
                    break;

            case preJump:
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("SpiderJump") &&
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    anim.SetBool("SpiderJump", false);
                    float time = Time.realtimeSinceStartup;

                    float angle;

                    if (Mathf.Abs(playerDistance) <= detectionRadius)
                    {
                        isFollowing = true;
                        
                        if (ArtrobotController.artTrans.position.x > transform.position.x)
                        {
                            angle = 45 + (45 * ((detectionRadius - playerDistance) / detectionRadius));
                            Debug.Log("Angle = " + angle);
                            

                        }
                        else
                        {
                            angle = 90 + (45 * (playerDistance) / detectionRadius);
                            Debug.Log("Angle = " + angle);
                        }

                    }
                    else
                    {
                        angle = 90;
                    }

                    characPhysics.StartPhysics(time, rgdBdy.position, 15, angle, 0.5f);
                    SetState(jumping);
                }
                break;

            case jumping:
                localOffset += characPhysics.Gravity(rgdBdy.position);

                if ((IsTravelingHoriz() == 1 && transform.position.x >=
                    ArtrobotController.player.transform.position.x) || (IsTravelingHoriz() == -1 && 
                    transform.position.x <= ArtrobotController.player.transform.position.x))
                {
                    float time = Time.realtimeSinceStartup;
                    characPhysics.StartPhysics(time, rgdBdy.position, 0, 0, 1);
                }
                break;
           
            default:

                break;
        }
        fixedPos += localOffset;
        rgdBdy.MovePosition(fixedPos);
    }

    public void StartJump()
    {
        anim.SetBool("SpiderJump", true);
        SetState(preJump);
    }

    public override void Die()
    {
        base.Die();
        
        if (state.Equals(preJump))
        {
            SetState(idle);
        }

        GetComponents<Collider2D>()[1].enabled = false;
    }

    //public void OnTriggerEnter2D(Collider2D collision)
    //{

    //}



    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 12 && state.Equals(jumping))
        {
            if (!stillAlive)
            {

                foreach (Collider2D c in GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }

            }
                
            SetState(idle);

        } 

        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Collider is from " + collision.gameObject.name);
        }
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        Debug.Log("Spider collided with + " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Collider is from " + collision.gameObject.name);
        }
        if (collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("Sword"))
        {
            if (state.Equals(jumping))
            {
                float time = Time.realtimeSinceStartup;
                
                characPhysics.StartPhysics(time, rgdBdy.position, 0, 0, 1);
            }
            
        }

    }
}
