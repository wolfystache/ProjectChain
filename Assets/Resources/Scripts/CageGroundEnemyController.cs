using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageGroundEnemyController : MonoBehaviour {

    public GameObject player;
    private bool isChasingRight;
    private bool isChasingLeft;
    float count;
    private Vector2 circPattern;
    private float center;
    private Vector2 offset;
    private float chaseOffset;

    // Use this for initialization
    void Start () {
        isChasingRight = false;
        isChasingLeft = false;
        count = 0;
        center = transform.position.x;
        offset = new Vector2(transform.position.x, transform.position.y);
        chaseOffset = 0;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
        count += 0.1f;
        Debug.Log("Cage Enemy Position is: " + transform.position);
        
        
        
        Vector2 playerPos = player.transform.position;
        Vector2 enemyPos = transform.position;
        float space = center - playerPos.x;
        Debug.Log("Center = " + center);

        Debug.Log("Space = " + space);
        if (space <= Mathf.Abs(8.0f))
        {
            if (space > 0)
            {
                //transform.position = new Vector2(transform.position.x - 0.1f, transform.position.y);
                chaseOffset -= 0.01f;
                center -= 0.01f;
                isChasingRight = true;
                Debug.Log("Chasing Right");

            }
            else
            {
                //transform.position = new Vector2(transform.position.x + 0.1f, transform.position.y);
                chaseOffset += 0.01f;
                center += 0.01f;
                isChasingLeft = true;
                Debug.Log("Chasing Left");
            }
        }
        circPattern = new Vector2(offset.x + Mathf.Cos(count) + chaseOffset, offset.y + Mathf.Sin(count));
        transform.position = circPattern;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enemy Bullet collision detected");
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Bullet-Bullet collision detected");
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}
