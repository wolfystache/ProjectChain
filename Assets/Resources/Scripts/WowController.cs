using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WowController : MonoBehaviour {
    public AudioClip regShot;
    public AudioClip impactClip;
    public AudioClip swordWoosh;
    public AudioSource audio;
    public AudioSource impact;

	// Use this for initialization
	void Start () {
        //    audio = GetComponent<AudioSource>();
        //   audio.clip = wow0; 
       
    }

    // Update is called once per frame
    //void Update () {

    //}

    public void BulletSound() {
   //     System.Random rdm = new System.Random();
    //    int clip = rdm.Next(0, 26);
    //    Debug.Log("Clip # = " + clip);
        audio.clip = regShot;
        audio.Play();
    }  
    public void Impact()
    {
        audio.clip = impactClip;
        audio.Play();
    }
    public void Woosh()
    {
        audio.clip = swordWoosh;
        audio.Play();
    } 


}
