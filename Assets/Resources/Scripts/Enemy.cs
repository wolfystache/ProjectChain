using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    public bool isFading = false;
    protected Color origColor;
    public float health;
    public float totalHealth;
    public bool stillAlive = true;

    // Use this for initialization
    public virtual void Start () {
        origColor = GetComponent<Renderer>().material.color;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(float damage)
    {
        Debug.Log("Taking Damage");
        health -= damage;



        if (health <= 0)
        {
            Die();
        }
    } 

    public virtual void Die()
    {
        stillAlive = false;
        StopAllCoroutines();

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
        if (collision.gameObject.CompareTag("Bullet") && !isFading)
        {
            StartCoroutine("DamageFade");

            TakeDamage(1);
            //Destroy(gameObject);         
            Destroy(collision.gameObject);
        }
    }


}
