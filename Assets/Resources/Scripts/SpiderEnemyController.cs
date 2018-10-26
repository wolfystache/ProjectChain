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
        

	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Spider color = " + GetComponent<Renderer>().material.color);
	}

    public override void FixedUpdate()
    {
        base.FixedUpdate();
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
                    characPhysics.StartPhysics(time, rgdBdy.position, 15, 90);
                    SetState(jumping);
                }
                break;

            case jumping:
                localOffset += characPhysics.Gravity(rgdBdy.position);
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

        }
    }
}
