using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enemy collision detected with " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Bullet-Bullet collision detected");
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}
