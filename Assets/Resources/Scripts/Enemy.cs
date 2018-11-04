using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    public bool isFading = false;
    protected Color origColor;
    public float health;
    public float totalHealth;
    public bool stillAlive = true;
    protected float detectionRadius;
    protected bool isFollowing = false;
    protected int damageCaused;
    Collider2D rgdCollider;
    Collider2D playerTrigger;
    protected AudioSource audio;

    // Use this for initialization
    public virtual void Start () {
        origColor = GetComponent<Renderer>().material.color;
        Collider2D[] playerCollider = new Collider2D[ArtrobotController.artTrans.
            GetComponent<Rigidbody2D>().attachedColliderCount];
        ArtrobotController.artTrans.
            GetComponent<Rigidbody2D>().GetAttachedColliders(playerCollider);
        rgdCollider = GetComponents<Collider2D>()[0];
        playerTrigger = ArtrobotController.player.GetComponents<Collider2D>()[0];
        Debug.Log("Collider count = " + ArtrobotController.artTrans.GetComponent<Rigidbody2D>().attachedColliderCount);

        foreach (Collider2D c in ArtrobotController.artTrans.GetComponents< Collider2D > ())
        {

                Debug.Log("Ignoring Collision with collider " + c.name);
                Physics2D.IgnoreCollision(rgdCollider, c, true);
                
                Debug.Log("rgdCollider name = " + rgdCollider.name);
                

        }
        int num = 0;
        foreach (Collider2D c in ArtrobotController.artTrans.GetComponents<Collider2D>())
        {
            
            Debug.Log("Ignore Collision = " + num + " " + Physics2D.GetIgnoreCollision(rgdCollider, c));
            Debug.Log("rgdCollider is trigger = " + c.isTrigger);
            num++;
        }
            audio = GetComponent<AudioSource>();

    }

    protected override void Awake()
    {
        base.Awake();
    }
	
	// Update is called once per frame
	public override void FixedUpdate () {
        base.FixedUpdate();
        Debug.Log("Ignore Collision = " + Physics2D.GetIgnoreCollision(rgdCollider,
            playerTrigger));
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Taking Damage");
        health -= damage;



        if (health <= 0)
        {
            Die();
        }
        else
        {
            AudioClip impact = (AudioClip)Resources.Load("SoundFX/" +
                   "zapsplat_impact_metal_thin_001_10694");
            audio.clip = impact;
            audio.Play();
            audio.loop = false;
        }
    } 

    public virtual void Die()
    {
        stillAlive = false;
        StopAllCoroutines();

        audio.Stop();
        AudioClip clip = (AudioClip)Resources.Load("SoundFX/power_down");
        if (clip == null)
        {
            Debug.Log("Power Down SoundFX cannot be found");
        }
        else
        {
            audio.clip = clip;
            audio.loop = false;
            audio.Play();
        }
        

        StartCoroutine("FadeToGrey");
    }


    //Enemy Coroutines

    public IEnumerator DamageFade()
    {
        Debug.Log("Starting Flash");
        //Color fade = new Color(255f, 0f, 0f);
        //Color origColor = GetComponent<Renderer>().material.color;
        //GetComponent<Renderer>().material.color = redFlash;
        isFading = true;
        float fade = 1.0f;
        bool fadingDown = true;


        for (float i = 0; i < 60; i++)
        {
            Color c = GetComponent<Renderer>().material.color;
            if (fadingDown)
            {
                if (fade > 0)
                {
                    fade -= 0.08f;
                    c.g = fade;
                    c.b = fade;
                    GetComponent<Renderer>().material.color = c;
                    Debug.Log("Color is " + c);
                }
                else
                {
                    fadingDown = false;
                }
            }
            else
            {
                if (fade <= 1)
                {
                    fade += 0.08f;
                    c.g = fade;
                    c.b = fade;
                    GetComponent<Renderer>().material.color = c;
                    Debug.Log("Color is " + c);
                }
                else
                {
                    fadingDown = true;
                }
            }
            yield return null;

        }
        GetComponent<Renderer>().material.color = origColor;
        isFading = false;
    }

    public IEnumerator FadeToGrey()
    {
        Debug.Log("Fading to grey");
        Color color = origColor;
        GetComponent<Renderer>().material.color = color;

        float H, S, V;

        float r, g, b;

        r = color.r; g = color.g; b = color.b;

        Color.RGBToHSV(color, out H, out S, out V);

        Debug.Log(H + "," + S + "," + V);

        while (V > 0.4f)
        {
            V -= 0.01f;
            color = Color.HSVToRGB(H, S, V);
            GetComponent<Renderer>().material.color = color;
            Debug.Log("Color = " + color);
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Enemy Base Class colliders 

    public virtual void OnTriggerEnter2D(Collider2D collision)

    {
        Debug.Log("Enemy has collided with " + collision.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!PlayerHealthScript.player.GetIsHitStunned())
            {

                if (ArtrobotController.player.GetSubState().Equals("sword"))
                {
                    ArtrobotController.player.SetSubState("none");
                    ArtrobotController.player.GetComponentInChildren<SwordController>().Reset();
                    ArtrobotController.player.anim.SetBool("sword", false);
                }


                PlayerHealthScript.player.DamageOrHealth(damageCaused);
                if (ArtrobotController.player.state.Equals("swinging") ||
                    ArtrobotController.player.state.Equals("chaining"))
                {
                    Debug.Log("Starting fall off swing");
                    ArtrobotController.player.FallOffSwing();
                }
                else
                {
                    ArtrobotController.player.KnockBack();
                }
                WowController.player.Impact();
            }

        }
        
        else if (collision.gameObject.CompareTag("Bullet") && !isFading)
        {
            StartCoroutine("DamageFade");

            TakeDamage(0.25f);
            //Destroy(gameObject);         
            Destroy(collision.gameObject);
        }  
        else if (collision.gameObject.CompareTag("Sword") && !isFading)
            {
                StartCoroutine("DamageFade");

                TakeDamage(1.0f);
            }
       

        
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Enemy has collided with " + collision.name);
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

    }


}
