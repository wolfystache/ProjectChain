using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimArmController : MonoBehaviour {

    private float lastAngle;
    private float currAngle;
    private Vector2 origPosition;
    private Vector2 currAim;
    public Sprite StandingAim;
    public Sprite ClimbingAim;
    public Transform armPivot;

	// Use this for initialization
	void Start () {
        lastAngle = 0;
        origPosition = transform.localPosition;
        currAim = GetComponentInParent<ArtrobotController>().GetRadialInput();
    }

    // Update is called once per frame
    private void Update()
    {
  //      Debug.Log("Arm Rotation = " + transform.rotation);
  //      Debug.Log("Arm Position = " + transform.position);
    }

    public void aim(float desiredAngle)
    {
  //      Debug.Log("Aim has been called");
        //  transform.GetChild(0).localPosition = new Vector2(0.06f, 0.21f);
        
        string state = transform.parent.GetComponent<ArtrobotController>().GetState();
        bool facingRight = transform.parent.GetComponent<ArtrobotController>().IsFacingRight();
        GetComponent<SpriteRenderer>().enabled = true;

     //   Vector2 aimDir = GetComponentInParent<ArtrobotController>().GetRadialInput();
        Vector2 aimDir = GetComponentInParent<ArtrobotController>().GetAimDir();
        Debug.Log("AimDir = (" + aimDir.x + ", " + aimDir.y + ")");
    //    Debug.Log(aimDir.x + "," + aimDir.y);
        if (desiredAngle == 0)
        {
            //Convert radial input to degrees. Trig is done in radians
            //    currAngle = Mathf.Atan(aimDir.y / aimDir.x) * 57.2958f;
            currAngle = GetComponentInParent<ArtrobotController>().GetAimAngle();
         //   Debug.Log("currAngle = " + currAngle);


        }
       
        else
        {
            currAngle = desiredAngle;
        }

        
    //    Debug.Log("lastAngle = " + lastAngle);

        //    Debug.Log("Current Angle = " + currAngle);
        //    Debug.Log("Last Angle = " + lastAngle);
        //   Debug.Log("Transform value = " + (currAngle - lastAngle));
        if (desiredAngle > 0)
        {
            if (facingRight)
            {
                lastAngle = 0;
            }
            else
            {
                lastAngle = 180;
            }
            transform.RotateAround(armPivot.transform.position, Vector3.forward, (currAngle - lastAngle));
            lastAngle = currAngle;
            currAim = aimDir;
        }
        else
        {
            if (facingRight)
            {
                if (state.Equals("climbing"))
                {
                    if (currAngle > 130 && currAngle < 230)
                    {
                        transform.RotateAround(armPivot.transform.position, Vector3.forward, (currAngle - lastAngle));
                        lastAngle = currAngle;
                        currAim = aimDir;

                    }
                }
                else
                {
                    if (currAngle > 90)
                    {
                        GetComponentInParent<ArtrobotController>().TurnAround(1);
                        transform.localPosition = origPosition;
                        transform.localRotation = Quaternion.identity;
                        lastAngle = 180;
                      //  GetComponentInParent<ArtrobotController>().AimReset();
                        GetComponentInChildren<AimDotController>().StopSights();
                        aim(0);
                        GetComponentInChildren<AimDotController>().StartSights();
                        return;
                    }
             
                    transform.RotateAround(armPivot.transform.position, Vector3.forward, (currAngle - lastAngle));
                    lastAngle = currAngle;
            //          Debug.Log("LastAngle = " + lastAngle);
                    currAim = aimDir;
                    
                }
            }

            else
            {
                

                if (state.Equals("climbing"))
                {
                    if (currAngle < 50 && currAngle > -50)
                    {
                        transform.RotateAround(armPivot.transform.position, Vector3.forward, (currAngle - lastAngle));
                        lastAngle = currAngle;
                        currAim = aimDir;

                    }
                }
                else
                {
                    if (currAngle < 90)
                    {
                        Debug.Log("CurrAngle < 90");
                       GetComponentInParent<ArtrobotController>().TurnAround(1);
                        transform.localPosition = origPosition;
                        transform.localRotation = Quaternion.identity;
                        lastAngle = 0;
                        //  GetComponentInParent<ArtrobotController>().AimReset();
                        GetComponentInChildren<AimDotController>().StopSights();
                        aim(0);
                        Debug.Log("Starting Sights");
                        GetComponentInChildren<AimDotController>().StartSights();
                        return;
                    }
                    //else if (currAngle < 100) { currAngle = 100; }

                    //else if ( currAngle > 260) { currAngle = 260; }

                    Debug.Log("Current Angle = " + currAngle);
                    Debug.Log("Last Angle = " + lastAngle);

                    transform.RotateAround(armPivot.transform.position, 
                        Vector3.forward, (currAngle - lastAngle));
                    lastAngle = currAngle;
                    currAim = aimDir;

                    
                }
            }
        }
        //transform.RotateAround(armPivot.transform.position, Vector3.forward, (currAngle - lastAngle));
        //lastAngle = currAngle;
        //currAim = aimDir;

        //Debug.Log("Post Arm Position = " + transform.position);
        //Debug.Log("Post Arm rotation = " + transform.rotation);
        transform.parent.GetComponent<ArtrobotController>().SetAimDir(currAim);
 //       transform.RotateAround(armPivot.transform.position, Vector3.forward, (currAngle - lastAngle));


        



    }
    public void resetAim()
    {
    //    Debug.Log("Resetting Aim");
        //       Debug.Log("Found " + transform.childCount + " number of children");
        // transform.GetChild(0).localPosition = new Vector2(0.15f, 0.007f);
        //   transform.GetChild(0).localPosition = new Vector2(0.5199999999999f, 0.23f);
        //  Debug.Log("Resetting Aim");
        GetComponent<SpriteRenderer>().enabled = false;
        transform.parent.GetComponent<Animator>().SetBool("isAiming", false);
        
        //lastAngle = currAngle;
        //currAngle = 0;
        //transform.RotateAround(armPivot.transform.position, Vector3.forward, currAngle - lastAngle);
     //   lastAngle = 0;
     //   Debug.Log("origPosition = " + origPosition);
        transform.localPosition = origPosition;
        transform.localRotation = Quaternion.identity;
    }
    public void setLastAngle(float lastAngle)
    {
        this.lastAngle = lastAngle;
    //    Debug.Log("LastAngle = " + lastAngle);
    } 

    public void setSprite (bool isClimbing)
    {
        if (isClimbing)

        {
        //    transform.Rotate(Vector3.up, 180f);
            GetComponent<SpriteRenderer>().sprite = ClimbingAim;
        }
        else
        {
       //     transform.localRotation = Quaternion.identity;
            GetComponent<SpriteRenderer>().sprite = StandingAim;
        }
    }
}
