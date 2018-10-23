using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeperScript : MonoBehaviour {
    private int score;
    public Text myText;
	// Use this for initialization
	void Start () {
        score = 0;
	}
	
	// Update is called once per frame

    public void inc()
    {   
        
        score++;
        Debug.Log("Score! You now have " + score + "points.");
        myText.text = "Score: " + score;
        myText.GetComponent<Text>(); 



    }
}
