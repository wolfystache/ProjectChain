using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthScript : MonoBehaviour {

    public Image[] healthList;
    public Sprite[] healthPics;
    int healthLevel;
    int totalHealth = 6;
    int numHearts = 3;
    int [] heartList;
    bool isHitStunned;


    // Use this for initialization
    void Start () {
        //  health.sprite = full; 
        //health2.sprite = full;
        //health1.sprite = half;
        //health0.sprite = empty;
        healthLevel = 6;
        heartList = new int[numHearts];
        HealthUpdate(healthLevel);
        isHitStunned = false;
        
    }
	
	// Update is called once per frame
    public void HealthUpdate(int newHealth)
    {
        healthLevel = newHealth;
        Debug.Log("Health Update Invoked: Health is at " + healthLevel);
        // Determine status of each heart in total health and set sprites
        for (int i = 0; i < numHearts; i ++)
        {
            if (newHealth >= 2 * (i+1))
            {
           //     Debug.Log("Full Heart");
                heartList[i] = 2;
                healthList[i].sprite = healthPics[2];
            } 
            else if (newHealth == (2 * (i+1) - 1))
            {
         //       Debug.Log("Half Heart");
                heartList[i] = 1;
                healthList[i].sprite = healthPics[1];
            }
            else
            {
           //     Debug.Log("Empty Heart");
                heartList[i] = 0;
                healthList[i].sprite = healthPics[0];
            }
        }

        if (healthLevel == 0) {
            Debug.Log("Out of health!");
  //          UnityEditor.EditorApplication.isPaused = true; 
          //  Application.Quit();
        }
    }   
    public int GetHealth()
    {
        return healthLevel;
    }

    public bool GetIsHitStunned ()
    {
        return isHitStunned;
    }

    public void DamageOrHealth (int offset)
    {
        if (!isHitStunned)
        {
            Debug.Log("Recieved " + offset + "units of damage");
            HealthUpdate(healthLevel + offset);
            if (offset < 0)
            {
                StartCoroutine("DamageFade");
            }
        }
    }
    IEnumerator DamageFade()
    {
        Debug.Log("Starting Flash");
        isHitStunned = true;
        float fade = 1.0f;
        Color origColor = GetComponent<Renderer>().material.color;
        Debug.Log("Orig Color is " + origColor);
        bool fadingDown = true;


        for (float i = 0; i < 120; i++)
        {
            Color c = GetComponent<Renderer>().material.color;
            if (fadingDown)
            {
                if (fade > 0)
                {
                    fade -= 0.05f;
                    c.a = fade;
                    GetComponent<Renderer>().material.color = c;
      //              Debug.Log("Color is " + c);
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
                    fade += 0.05f;
                    c.a = fade;
                    GetComponent<Renderer>().material.color = c;
     //               Debug.Log("Color is " + c);
                }
                else
                {
                    fadingDown = true;
                }
            }
            yield return null;

        }
        isHitStunned = false;
        GetComponent<Renderer>().material.color = origColor;
        Debug.Log("Final Color is " + GetComponent<Renderer>().material.color);
        
    }
}
