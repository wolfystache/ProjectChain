using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour {

    public Sprite[] spriteList;
    public static AimController player;



    private void Awake()
    {
        player = this;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame


    public void turnAround()
    {
        Debug.Log("Turn Around");
        transform.RotateAround(GetComponents<Collider2D>()[2].bounds.center, Vector3.up, 180.0f);
    } 

    public void localTurnAround()
    {

        transform.RotateAround(GetComponents<Collider2D>()[1].bounds.center, Vector3.up, 180.0f);
        for (int i = 0; i < transform.childCount; i ++)
        {
            if (transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>() != null)
            {
                transform.GetChild(i).RotateAround(GetComponents<Collider2D>()[1].bounds.center, 
                    Vector3.up, 180.0f);
            }
            else {
                transform.GetChild(i).transform.RotateAround
                   (transform.GetChild(i).transform.position, Vector3.up, 180.0f);
            }
        }
        GetComponent<ArtrobotController>().toggleDir();
    } 
    public void childrenTurnAround()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
      //      transform.GetChild(i).transform.RotateAround
        //        (GetComponents<Collider2D>()[1].bounds.center, Vector3.up, 180.0f);
        }
        
    }

}
