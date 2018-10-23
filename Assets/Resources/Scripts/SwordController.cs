using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour {

    public Quaternion originalRotValue;
    
    // Use this for initialization
	void Start () {
        originalRotValue = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		
	} 
    public void HitBox()
    {
        StartCoroutine("RotateSword");
        

    }
    IEnumerator RotateSword()
    {
        GetComponent<Collider2D>().enabled = true; 

        float time = 0;
        float totalTime = 0.3f;

        while (time < totalTime)
        {
            time += Time.deltaTime;
       //     Debug.Log("Time = " + time);
            transform.Rotate(Vector3.forward * 5.5f, Space.Self);
            yield return null;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, originalRotValue, Time.time);
        GetComponent<Collider2D>().enabled = false;
    }
}
