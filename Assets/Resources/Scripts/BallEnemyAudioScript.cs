using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEnemyAudioScript : MonoBehaviour {

    public AudioSource audSrc;
    public AudioClip swordAttack;

    public void SwordAttack()
    {
        audSrc.clip = swordAttack;
        audSrc.Play();
    }
}
