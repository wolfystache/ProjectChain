using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject character;
    public GameObject background;
    Vector2 offset;
    float width;
    float height;
    Vector3 end;
    Vector3 origin;
    Vector2 rightOffset;
    Vector2 leftOffset;
    bool isScrollingRight;
    bool isScrollingLeft;
    bool scrolledRightLast;
    bool scrolledLeftLast;
    float upOffset;
    float downOffset;
    private float rightStickVal;


    // Use this for initialization
	void Start () {

        width = Camera.main.pixelWidth;
        height = Camera.main.pixelHeight;
        offset = new Vector2(transform.position.x - character.transform.position.x, transform.position.y - character.transform.position.y);
        float half = Camera.main.ScreenToWorldPoint(new Vector3(width/2.0f, 0, 0)).x; 
        float quart = Camera.main.ScreenToWorldPoint(new Vector3(width/4.0f, 0, 0)).x;
        float heightHalf = Camera.main.ScreenToWorldPoint(new Vector3(0, height / 2.0f, 0)).y;
        float height10th = Camera.main.ScreenToWorldPoint(new Vector3(0, height / 10.0f, 0)).y;
        end = Camera.main.ScreenToWorldPoint(new Vector2(width, height));
        origin = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        Debug.Log("width = " + width + " height = " + height);
        Debug.Log("heightHalf is " + heightHalf);
        Debug.Log("height10th is " + height10th);
        Debug.Log("Camera pos is " + transform.position);
        rightOffset = new Vector2(transform.position.x - half, transform.position.y - character.transform.position.y); 
        leftOffset = new Vector2(transform.position.x - quart, transform.position.y - character.transform.position.y);
        upOffset = transform.position.y - heightHalf;
        downOffset = transform.position.y - height10th;
        Debug.Log("right offset is " + rightOffset);
        Debug.Log("left offset is " + leftOffset);
        Debug.Log("up offset is " + upOffset);
        Debug.Log("down offset is " + downOffset);
        isScrollingRight = false;
        isScrollingLeft = false;
        scrolledRightLast = false;
        scrolledLeftLast = false;
        rightStickVal = 0;



    }
   

    // Update is called once per frame
    void LateUpdate () {
        //   transform.position = new Vector3(transform.position.x, character.transform.position.y + offset.y, -10); 
       // Vector2 pos = Camera.main.WorldToScreenPoint(character.transform.position);
      // Debug.Log("Camera position: " + transform.position.y);
       // if (isScrollingRight) {

       //     //    
       //     Debug.Log("Scrolling Right Camera Controller");
       //     transform.position = new Vector3(character.transform.position.x + rightOffset.x, transform.position.y, -10);
       //     background.transform.position = new Vector3(character.transform.position.x * 0.9f + rightOffset.x * 0.9f, background.transform.position.y, 0);

       ////     Debug.Log("X's Screen Position is: " + pos.x);
       //     scrolledRightLast = true;
       //     isScrollingRight = false;
       // } 
       // else if (isScrollingLeft)
       // {
       //     Debug.Log("Scrolling Left Camera Controller");

       //     transform.position = new Vector3(character.transform.position.x + leftOffset.x, transform.position.y, -10);
       //     background.transform.position = new Vector3(character.transform.position.x * 0.9f  + leftOffset.x * 0.9f, background.transform.position.y, 0);
            
            
       //     scrolledLeftLast = true;
       //     isScrollingLeft = false;
       // }
       // else
       // {
       //     scrolledRightLast = false;
       //     scrolledLeftLast = false;
       // }

    }
    public void ScrollRight()
    {
        isScrollingRight = true;
        transform.position = new Vector3(character.transform.position.x + rightOffset.x, transform.position.y, -10);
        background.transform.position = new Vector3(character.transform.position.x * 0.9f + rightOffset.x * 0.9f, background.transform.position.y, 0);
    }

    public void ScrollLeft()
    {
        isScrollingLeft = true;
        transform.position = new Vector3(character.transform.position.x + leftOffset.x, transform.position.y, -10);
        background.transform.position = new Vector3(character.transform.position.x * 0.9f + leftOffset.x * 0.9f, background.transform.position.y, 0);
    }
    public void ScrollUp()
    {
        transform.position = new Vector3(transform.position.x, character.transform.position.y + upOffset, -10);
    //    Debug.Log("Scrolling Up");
        background.transform.position = new Vector3(background.transform.position.x, 0.98f * (character.transform.position.y + upOffset), 0);
    }
    public void ScrollDown()
    {
   //     Debug.Log("Scrolling Down");
        transform.position = new Vector3(transform.position.x, character.transform.position.y + upOffset, -10);
        background.transform.position = new Vector3(background.transform.position.x, 0.98f * (character.transform.position.y + upOffset), 0);
    }
    public void LookUp(float rightStickVal)
    {
        if (this.rightStickVal != rightStickVal) 
        {
            float diff = rightStickVal - this.rightStickVal;
            transform.position = new Vector3(transform.position.x, transform.position.y + (1 * diff), -10);
        }

        this.rightStickVal = rightStickVal; 

    }
}
