using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics {


    private float fallTime;
    private bool startedFalling = false;
    private Vector2 startPosition;
    private float initialVelocity;
    private float velAngle;
    private bool isJumpingOrFalling;
    private Vector2 lastDisp;
    private Vector2 lastOffset;
    private string name = "NotAssigned";

    public static float grav = 3 * - 9.81f;

    public Physics (string name)
    {
        this.name = name;
    }
 


    public void StartPhysics(float fallTime, Vector2 startPosition, float initialVelocity, 
        float velAngle)
    {
        Debug.Log("Starting Physics calculations");
        this.fallTime = fallTime;
        this.startPosition = startPosition;
        //    lastPos = startPosition;
        lastDisp = Vector2.zero;
        lastOffset = Vector2.zero;
        this.initialVelocity = initialVelocity;
        velAngle /= (180 / Mathf.PI);
        this.velAngle = velAngle;
    }
    public void StopPhysics()
    {
        fallTime = initialVelocity = velAngle = 0;
        startPosition = Vector2.zero;
        isJumpingOrFalling = false;
        
    }


    public Vector2 Gravity(Vector2 position)
    {
        
        float initXVel = Mathf.Cos(velAngle) * initialVelocity;
        float initYVel = Mathf.Sin(velAngle) * initialVelocity;

        startedFalling = false;

        Debug.Log("initXVel = " + initXVel);
        float currTime = Time.realtimeSinceStartup - fallTime;
        float xPos = 0;
        float maxVel = 0;
        bool reachedMaxVel = false;
        currTime = Time.realtimeSinceStartup - fallTime;
        if (currTime > 0.7f && !reachedMaxVel)
        {
            //     Debug.Log("Approximately 0.7f");
            currTime = Time.realtimeSinceStartup - fallTime;
            //    maxVel = (initYVel * currTime + 0.5f * grav * Mathf.Pow(currTime, 2.0f) + startPosition.y)
            //        - position.y;
            maxVel = (initYVel * currTime + 0.5f * grav * Mathf.Pow(currTime, 2.0f) - lastDisp.y);
            reachedMaxVel = true;

        }
        //   Debug.Log("Current Time = " + currTime);
        //   Debug.Log("MaxVel = " + maxVel);


        float xDisp = (initXVel * currTime);
        float yDisp = initYVel * currTime + 0.5f * grav * Mathf.Pow(currTime, 2.0f);
        
        

        Vector2 offset = new Vector2(xDisp - lastDisp.x, yDisp - lastDisp.y);

        Debug.Log("Last Offset = " + lastOffset + "\nOffset = " + offset);
        if (lastOffset.y > 0 && offset.y <= 0)
        {
            Debug.Log("Now Falling");
            startedFalling = true;
        }

        Debug.Log("Offset = " + offset.x + "," + offset.y);

        lastDisp = new Vector2(xDisp, yDisp);
        lastOffset = offset;
        if (currTime < 0.7f)
        {
            return offset;
        }
        else
        {
            return new Vector2(offset.x, maxVel);
        }
    } 

    public static float PendulumDisplacement(float thetaMax, float length, float time, float grav)
    {
  //      Debug.Log("thetaMax = " + thetaMax + "\nlength = " + length
   //         + "\ntime = " + time + "\ngrav = " + grav);
        float displ = thetaMax * Mathf.Cos((Mathf.Sqrt(grav / length) * time));
   //     Debug.Log("Displ = " + displ);
        return displ;
    }

    public string GetName()
    {
        return name;
    } 

    public bool StartedFalling()
    {
        return startedFalling;
    }
}
