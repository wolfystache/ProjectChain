using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxControllerScript : MonoBehaviour {

    public float maxSpeed = 10f;
    public GameObject bulletPreFab;
    public Transform bulletSpawn;
    private bool isAimUp;
    
	// Use this for initialization
	void Start () {
        //  audio = GetComponent<AudioSource>();  
        isAimUp = false;
	}
	
	// Update is called once per 
	void Update () {
     //   audio = Getc
        float move = Input.GetAxis("Horizontal");
    //    Debug.Log(move);
        Vector2 rigidbody2DVel = GetComponent<Rigidbody2D>().velocity;
        GetComponent<Rigidbody2D>().velocity = new Vector2(move * maxSpeed, rigidbody2DVel.y);  

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
            Debug.Log("Jump Pressed!!");
        }

        if (Input.GetButtonDown("Fire1")) 


        {
            Shoot(isAimUp);
        } 
        if (Input.GetKey("up") || Input.GetKey("w") || Input.GetButton("LookUp"))
        {
        //    GetComponent<AimController>().aimUp();
            isAimUp = true;
        } 
        else if (Input.GetKeyUp("up") || Input.GetKeyUp("w") || Input.GetButtonUp("LookUp"))
        {
      //      GetComponent<AimController>().resetAim();
            isAimUp = false;
        }
       // if (Input.GetKey("left"))
       // {
       ////     GetComponent<AimController>().turnLeft();
       // }
	} 
    void Jump ()
    {
        float jump = Input.GetAxis("Jump") * 10;
        Debug.Log(jump);
        Vector2 rigidbody2DVel = GetComponent<Rigidbody2D>().velocity;
        GetComponent<Rigidbody2D>().velocity = new Vector2(rigidbody2DVel.x, jump);
    }  

    void Shoot (bool aimUp)
    {
        var bullet = (GameObject)Instantiate(bulletPreFab, 
            bulletSpawn.position, bulletSpawn.rotation);
        if (aimUp)
        {
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * 6;
              Debug.Log("Bullet's position is " + bullet.transform.position);
            bullet.GetComponent<Rigidbody2D>().gravityScale = 0;
        } 
        else
        {
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * 6;
               Debug.Log("Bullet's position is " + bullet.transform.position);
            bullet.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
        GetComponent<WowController>().BulletSound();
        Destroy(bullet, 2.0f);

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            Debug.Log("Taking Damage!!");
            Destroy(collision.gameObject);
            GetComponent<PlayerHealthScript>().DamageOrHealth(-1);
            GetComponent<WowController>().Impact();   
   
            

        }
    }
}
