using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    // Use this for initialization
    public AudioSource audio;
    GameObject ship;
    public Transform cageSpawn;
    public GameObject cageBullet; 


    bool collDetection = false;



    void Start () {
  //      Debug.Log("Started Enemy Controller");
        audio = GetComponent<AudioSource>();
        StartCoroutine("Attack");
        StartCoroutine("Shoot");
        ship = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame

    IEnumerator Attack()
    {
  //      Debug.Log("Started Attack method");
        bool isAlive = true;
        int i = 0;
        int width = Camera.main.pixelWidth;
        int height = Camera.main.pixelHeight;
        Vector2 end = Camera.main.ScreenToWorldPoint(new Vector2(width, height));
        Vector2 origin = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        while (isAlive)
        {
            switch (i)
            {
                case 0:
                    Down();
                    i++;
                    break;
                case 1:
                    Down();
                    i++;
                    break;
                case 2:
                    if (transform.position.x <= ((end.x + origin.x) / 2))
                    {
                        Right();
                    }
                    else
                    {
                        Left();
                    }
                    i = 0;
                    break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    } 
    void Down()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y - 0.1f);
    }
    void Right()
    {
        transform.position = new Vector2(transform.position.x + 0.2f, transform.position.y);

    }
    void Left()
    {
        transform.position = new Vector2(transform.position.x - 1f, transform.position.y);

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        BoxCollider2D[] allCollide = gameObject.GetComponents<BoxCollider2D>();
        //    Debug.Log("Collision detected"); 
        
        if (other.CompareTag("Bullet") && !collDetection)
        {
            collDetection = true;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            for (int i = 0; i < allCollide.Length; i++)
            {
                allCollide[i].enabled = false;
            }
            Debug.Log("Collision detected");
            ship.GetComponent<ScoreKeeperScript>().inc();
            audio.Play();
            
            
            Destroy(gameObject, 3.0f); 
            Destroy(other.gameObject);
            
           
        }
    }
    IEnumerator Shoot()
    {
        var bullet = (GameObject)Instantiate(cageBullet, cageSpawn.position, cageSpawn.rotation);
        bool isAlive = true;
        int i = 0;
        int width = Camera.main.pixelWidth;
        int height = Camera.main.pixelHeight;
        Vector2 end = Camera.main.ScreenToWorldPoint(new Vector2(width, height));
        Vector2 origin = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        while (isAlive)
        {
            if (bullet == null)
            {
                isAlive = false;
                break;
            }
            if (i <= 10)
            {
                bullet.transform.position = new Vector2(bullet.transform.position.x + 0.1f,
                    bullet.transform.position.y - 0.1f); 
                
                
            } 
            else
            {
                bullet.transform.position = new Vector2(bullet.transform.position.x - 0.1f,
                    bullet.transform.position.y - 0.1f);
                if (i == 20) i = 0;
            }
            i++;
            yield return new WaitForSeconds(0.05f);
        }
           
    }
}

