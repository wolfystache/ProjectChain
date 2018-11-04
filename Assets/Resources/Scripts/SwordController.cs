using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour {

    public Quaternion originalRotValue; 

    private BoxCollider2D origCollider;
    private Vector2 origSize;
    private Vector2 origOffset;
    private string state;
    private Animator anim;

    private const string antic = "antic", attack = "attack", recov = "recov";

    private const float FRAME_RATE = 1 / 48;
    
    // Use this for initialization
	void Start () {
        originalRotValue = transform.rotation;
        origCollider = GetComponent<BoxCollider2D>();
        origSize = origCollider.size;
        origOffset = origCollider.offset;
        state = "idle";
        anim = transform.parent.GetComponent<Animator>();

	}
	
	// Update is called once per frame
    public void HitBox()
    {
        if (!state.Equals(attack))
        {
            Debug.Log("Enabling Hit Box");
            state = attack;
            transform.parent.GetComponent<WowController>().Woosh();
            StartCoroutine("RotateSword");
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    //private void Update()
    //{
    //    Debug.Log("Sword state = " + state);
    //    Debug.Log("BoxCollider enabled = " + GetComponent<BoxCollider2D>().enabled);
    //}

    public void Reset()
    {
        if (!state.Equals(recov))
        {
            Debug.Log("Turning off Hit Box");
            state = recov;
            GetComponent<BoxCollider2D>().size = origSize;
            GetComponent<BoxCollider2D>().offset = origOffset;
            GetComponent<BoxCollider2D>().enabled = false;
            StopAllCoroutines();
        }
    }
    IEnumerator RotateSword()
    {
    //    GetComponent<BoxCollider2D>().enabled = true;
        Collider2D hitBox = origCollider;

        float startTime = Time.realtimeSinceStartup;

        float startFrame = 1;
        float frame = startFrame;
        float EndFrame = 9;
        float colliderChangeSize = 1.26f;

        while (frame < EndFrame)
        {
            
            Debug.Log("Time = " + (Time.realtimeSinceStartup - startTime));
            Debug.Log("Frame = " + frame);
            Debug.Log("NormTime = " + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            Vector2 offset = GetComponent<BoxCollider2D>().offset;
            GetComponent<BoxCollider2D>().offset = new Vector2(offset.x, offset.y += ((colliderChangeSize / 2)
                / EndFrame));
            Vector2 size = GetComponent<BoxCollider2D>().size;
            GetComponent<BoxCollider2D>().size = new Vector2(size.x, size.y += (colliderChangeSize / EndFrame));
            

            frame ++;

            yield return new WaitForSeconds (FRAME_RATE);
        }
        
      //  transform.rotation = Quaternion.Slerp(transform.rotation, originalRotValue, Time.time);
    //    GetComponent<Collider2D>().enabled = false;
    }
}
