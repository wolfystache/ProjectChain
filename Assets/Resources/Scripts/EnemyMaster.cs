using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMaster : MonoBehaviour {

    public GameObject enemy;

	// Use this for initialization
	void Start () {
     //   Debug.Log("Enemy Master started");
        //     StartCoroutine("Spawn"); 
      

    }

    // Update is called once per frame
        IEnumerator Spawn ()
        {
                bool isAlive = true;
                while (isAlive)
                {

           //         Debug.Log("You're still alive??");
                    int width = Camera.main.pixelWidth;
                    int height = Camera.main.pixelHeight; 

                    System.Random rdm = new System.Random();
                    int xPos = rdm.Next(100, width - 100);
                    Vector3 start = Camera.main.ScreenToWorldPoint(new Vector3(xPos, height, 0));
              //     Debug.Log("World Position is " + start);
                                // Transform tran = new Transform();
                                var enemy1 = (GameObject)Instantiate(enemy, start, Quaternion.identity);
         //           Debug.Log("Enemy's Position is " + enemy1.transform.position);
         //                      Debug.Log("Take this!!");
                    yield return new WaitForSeconds(8.0f);
        }
     
    }
}
