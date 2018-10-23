using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DemoScript : MonoBehaviour {

    public Text myText;
 //   public List<GameObject> platforms;
    private float FallSpeed;
    private bool countdown;
    public Canvas canvas;
    public GameObject player;
    public GameObject ledge;
    private float totalSeconds;
    private bool gameOver;
    float currTime;

    
	// Use this for initialization
	void Start () {
        StartCoroutine("Countdown");

        FallSpeed = 0;
        totalSeconds = 0;
        countdown = false;
        gameOver = false;
        currTime = 0;
    }

    // Update is called once per frame
    void Update() {
        if (!gameOver)
        {
        //    transform.position = new Vector2(transform.position.x,
        //        transform.position.y + (FallSpeed * Time.deltaTime));
          //  GetComponent<Rigidbody2D>().velocity = new Vector2(0f, FallSpeed);
            if (countdown)
            {
                totalSeconds = Time.realtimeSinceStartup - currTime;
                myText.text = totalSeconds.ToString("F2");
            }
        } 
        if (player.GetComponent<PlayerHealthScript>().GetHealth() == 0)
        {
            EndDemo();
        }
	}
    private void FixedUpdate()
    {
        if (!gameOver)
        {
            //  List<GameObject> platforms = transform.chil
        
        }
    }
    public void FallingLedges()
    {
        FallSpeed = -0.03f;
        Component[] scriptList = GetComponentsInChildren<ChainableController>();

        foreach (ChainableController v in scriptList)
        {
            Debug.Log("FallSpeed = " + FallSpeed);
            v.SetSpeed(new Vector2(0, FallSpeed));
            //transform.position = new Vector2(transform.position.x,
            //    transform.position.y + (FallSpeed * Time.deltaTime));
        }

    }

    public void EndDemo()
    {
        if (countdown)
        {
           // Time.timeScale = 0;
            gameOver = true;
            totalSeconds = Mathf.Round(totalSeconds * 100.0f) / 100.0f;
            myText.text = "Game Over \nYou lasted " + 
                totalSeconds + " seconds";
            
            if (totalSeconds > GameControl.control.highScore)
            {
                GameControl.control.highScore = totalSeconds;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
    }

    IEnumerator Countdown()
    {
        int count = 5; 
        while (count > 0)
        {
           // Debug.Log(count);
          
            myText.text = count.ToString();
            yield return new WaitForSeconds(1.0f);
            count--;
        }

      
        myText.text = "";
        

        FallingLedges();
        StartCoroutine("LedgeSpawn");
        countdown = true;
        currTime = Time.realtimeSinceStartup;
        yield return null;
    } 
    IEnumerator LedgeSpawn()
    {
        int iter = 1;

        while (!gameOver)
        {
            int GetChildCount = transform.childCount;
            //Vector2 screenCoord = Camera.main.WorldToScreenPoint
            //    (transform.GetChild(GetChildCount-1).transform.position);
            Vector2 screenCoord = (transform.GetChild(GetChildCount - 1).transform.position);
            float xPos;
                 
            if (Random.Range(0,2) == 1)
            {
                xPos = screenCoord.x + (Random.Range(8.0f, 10.0f));
            }
            else
            {
                xPos = screenCoord.x - (Random.Range(7.0f, 9.0f));
            }
            float yPos = screenCoord.y + 8.0f;
            // Vector2 globalCoord = Camera.main.ScreenToWorldPoint(new Vector3(xPos, yPos, 0));
            Vector2 globalCoord = (new Vector3(xPos, yPos, 0));
            GameObject ledgeSpawned = Instantiate(ledge, globalCoord, ledge.transform.rotation);
            ledgeSpawned.transform.parent = transform;
            ledgeSpawned.GetComponent<ChainableController>().SetSpeed(new Vector2(0, FallSpeed));
            ledgeSpawned.name = "VertSurface " + iter;
            iter++;
            //Debug.Log("Set speed to " + ledgeSpawned.GetComponent<ChainableController>().GetSpeed().y);
            yield return new WaitForSeconds(2.0f);
        }

    }
    
}
