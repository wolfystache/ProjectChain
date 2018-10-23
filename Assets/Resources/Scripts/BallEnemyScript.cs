using System.Collections;
using System.Collections.Generic;
//using System;
using System;
using UnityEngine; 

public class BallEnemyScript : MonoBehaviour
{

    public GameObject player;

    private bool playerDetected;
    private bool isChasingRight;
    private bool isChasingLeft;
    private bool hitState;
    private bool isFading = false;
    private AudioSource audio;
    private AudioClip roll;

    float count;
    private Vector2 circPattern;
    private Vector2 offset;
    private float chaseOffset;
    private float health;
    private float totalHealth;
    private float space;
    private Quaternion lightOrigin;
    private string state;
    private CircleCollider2D BallCollide;
    private BoxCollider2D PlatBox;
    private const string top = "top", right = "right", left = "left",
        bottom = "bottom", cornering = "cornering", idle = "idle";
    private string[] surfaceStates = { top, right, bottom, left };
    private int nextState;
    private float clockWise = 1;
    private float originalSpeed = 0.1f;
    private float speed;
    private Vector2 pivotPos = Vector2.zero;
    private Color origColor;
    private float angleOffset = 0;

    private GameObject laserHead;
    private GameObject laserBody;

    float headLength;
    float bodyLength; 



    private float lastRayLength;

    private Stack<GameObject> laserGroup;



    // Use this for initialization
    void Start()
    {
       
        playerDetected = false;
        isChasingRight = false;
        isChasingLeft = false;
        count = 0;
        offset = new Vector2(transform.position.x, transform.position.y);
        chaseOffset = 0;
        totalHealth = 2.0f;
        health = totalHealth;
        hitState = false;
        state = top;
        BallCollide = GetComponent<CircleCollider2D>();
        speed = originalSpeed;
        origColor = GetComponent<Renderer>().material.color;
        audio = GetComponent<AudioSource>();
        roll = (AudioClip)Resources.Load("SoundFX/corn-grain");
       
        audio.clip = roll;
        audio.loop = true;
        audio.Play();

        laserHead = (GameObject) Resources.Load("Sprites/Prefabs/BallLaserHead");
        laserGroup = new Stack<GameObject>();

        Animator headAnim = new Animator();
        Animator bodyAnim = new Animator();

        if (laserHead == null)
        {
            
            Debug.Log("Could not find laser head");
        }
        else
        {
            var head = Instantiate(laserHead);
            head.transform.parent = transform.GetChild(0);
            head.transform.localPosition = Vector3.zero;
            head.transform.localScale = Vector3.one;
            laserGroup.Push(head);
            headLength = head.GetComponent<BoxCollider2D>().bounds
                   .size.y;
            headAnim = head.GetComponent<Animator>();
            

        }

        laserBody = (GameObject)Resources.Load("Sprites/Prefabs/BallLaserBody");

        if (laserBody != null)
        {
            var head = laserGroup.Peek();
            Transform laserMaster = transform.GetChild(0);
            var body = Instantiate(laserBody, head.transform.GetChild(0).position, 
                laserMaster.rotation);
            body.transform.parent = transform.GetChild(0);
            body.transform.localScale = Vector3.one;
            laserGroup.Push(body);
            bodyLength = body.GetComponent<BoxCollider2D>().bounds.size.y;
            Debug.Log("Initl Body Position = " + body.transform.position.y);
            Debug.Log("Initial Spawn Position = " + body.transform.GetChild(0).position.y);
            bodyAnim = body.GetComponent<Animator>();
            AudioClip beam = (AudioClip) Resources.Load("SoundFX/BeamStream");
            if (beam != null)
            {
                AudioSource beamAudio = body.transform.GetChild(0).GetComponent<AudioSource>();
                beamAudio.loop = true;
                beamAudio.volume = 0.5f;
                beamAudio.clip = beam;
                beamAudio.Play();
            }
            else
            {
                Debug.Log("Unable to find laser beam audio");
            }
            

        }
        //   headAnim.SetBool("StartFlash", true);
        //   bodyAnim.SetBool("StartFlash", true); 
        StartCoroutine("LaserFlash");
        if (headAnim != null && bodyAnim!= null)
        {
            
        }
        else
        {
            Debug.Log("Could not find laser body");
        }
   
        //  lightOrigin = GetComponentsInChildren<Transform>()[1].rotation;



    }

    private void Update()
    {
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
     //   Debug.Log("Audio Distance = " + audio.dis)
        //Debug.Log("State = " + state);
        //Debug.Log("Speed = " + speed);
        //Debug.Log("Clockwise = " + clockWise);
        //Debug.Log("Original Speed = " + originalSpeed);
        
        count += 0.1f;
        Vector3 topRightCorner = Vector3.zero;
        Vector3 botRightCorner = Vector3.zero;
        Vector3 botLeftCorner = Vector3.zero;
        Vector3 topLeftCorner = Vector3.zero;

        SpawnLaser();

        if (audio.clip.Equals(roll))
        {
            audio.volume = speed * 10;
        }

        if (PlatBox != null)
        {
            //Debug.Log("PlatBox max bounds = " + PlatBox.bounds.max);
            //Debug.Log("PlatBox min bounds = " + PlatBox.bounds.min);
            //Debug.Log("PlatBox center bounds = " + PlatBox.bounds.center);
            topRightCorner = PlatBox.bounds.max;
            botRightCorner = new Vector3(PlatBox.bounds.max.x, PlatBox.bounds.min.y);
            botLeftCorner = PlatBox.bounds.min;
            topLeftCorner = new Vector3(PlatBox.bounds.min.x, PlatBox.bounds.max.y);
        }
        else
        {
            Debug.Log("PlatBox is null");
        }

     //   Collider2D collider = GetComponent<BoxCollider2D>(); 

        

        switch (state)
        {
            case cornering:
                float angle = 20 * -clockWise * speed;
                transform.RotateAround(pivotPos, Vector3.forward, angle);
                
                angleOffset += angle;
    //            Debug.Log("AngleOffet = " + angleOffset);

                if (Mathf.Abs(angleOffset) >= 90)
                {
                    angleOffset = 0;
                    Debug.Log("Next State = " + nextState);
                    SetState(surfaceStates[nextState]);  
                    
                }

                break;
            case top:
                if (PlatBox != null)
                {
                    if (BallCollide.bounds.center.x >=  topRightCorner.x
                        && clockWise ==1)
                    {
                        Debug.Log("Rotating around top right corner");
                        Debug.Log("TopRightCorner pos " + topRightCorner);
                        Debug.Log("CurrPos = " + transform.position);
                     //   transform.RotateAround(topRightCorner, Vector3.forward, -90);
                        pivotPos = topRightCorner;
                        nextState = 1;
                        SetState(cornering);
                    }
                    else if (BallCollide.bounds.center.x <= topLeftCorner.x && 
                        clockWise == -1)
                    {
                        // transform.RotateAround(topLeftCorner, Vector3.forward, 90);
                        pivotPos = topLeftCorner;
                        nextState = 3;
                        SetState(cornering);
                    }
                    else
                    {
                    //    Debug.Log("Moving ball right");
                        Move(new Vector2(1, 0), clockWise);
                    }
                }
                else
                {
                    Debug.Log("Ball is not colliding right now");
                }
                
                break;
            case right:
                if (PlatBox != null)
                {
                    if (BallCollide.bounds.center.y <= botRightCorner.y 
                        && clockWise == 1)
                    {
                        Debug.Log("Rotating around bottom right corner");
                        //  transform.RotateAround(botRightCorner, Vector3.forward, -90);
                        pivotPos = botRightCorner;
                        nextState = 2;
                        SetState(cornering);
                    }
                    else if (BallCollide.bounds.center.y >= topRightCorner.y &&
                       clockWise == -1)
                    {
                     //   transform.RotateAround(topRightCorner, Vector3.forward, 90);
                        pivotPos = topRightCorner;
                        nextState = 0;
                        SetState(cornering);
                    }
                    else
                    {
      //                  Debug.Log("Moving ball down");
                        Move(new Vector2(0,-1), clockWise);
                    }
                }
                else
                {
                    Debug.Log("Ball is not colliding right now");
                }
                break;
            case bottom:
                if (PlatBox != null)
                {
                    if (BallCollide.bounds.center.x <= botLeftCorner.x 
                        && clockWise == 1)
                    {
                        Debug.Log("Rotating around bottom left corner");
                        //   transform.RotateAround(botLeftCorner, Vector3.forward, -90);
                        pivotPos = botLeftCorner;
                        nextState = 3;
                        SetState(cornering);
                    }
                    else if (BallCollide.bounds.center.x >= botRightCorner.x &&
                       clockWise == -1)
                    {
                        //    transform.RotateAround(botRightCorner, Vector3.forward, 90);
                        pivotPos = botRightCorner;
                        nextState = 1;
                        SetState(cornering);
                    }
                    else
                    {
                //        Debug.Log("Moving ball left");
                        Move(new Vector2(-1,0), clockWise);
                    }
                }
                else
                {
                    Debug.Log("Ball is not colliding right now");
                }
                break;
            case left:
                if (PlatBox != null)
                {
                    if (BallCollide.bounds.center.y >= topLeftCorner.y && clockWise == 1)
                    {
                        Debug.Log("Rotating around top left corner");
                        //     transform.RotateAround(topLeftCorner, Vector3.forward, -90);
                        pivotPos = topLeftCorner;
                        nextState = 0;
                        SetState(cornering);
                    }
                    else if (BallCollide.bounds.center.y <= botLeftCorner.y &&
                       clockWise == -1)
                    {
                    //    transform.RotateAround(botLeftCorner, Vector3.forward, 90);
                        pivotPos = botLeftCorner;
                        nextState = 2;
                        SetState(cornering);
                    }
                    else
                    {
                    //    Debug.Log("Moving ball up");
                        Move(new Vector2(0, 1), clockWise);
                    }
                }
                else
                {
                    Debug.Log("Ball is not colliding right now");
                }
                break;
            case idle:
                break;
            default:
                break;
        }


     //   Debug.Log("Cage Enemy Position is: " + transform.position);



        Vector2 playerPos = player.transform.position;
        Vector2 center = GetComponent<Collider2D>().bounds.center;
        Vector2 enemyPos = transform.position;
        space = center.x - playerPos.x;
  
       
        //if (Mathf.Abs(space) <= 8)
        //{
        //    IEnumerator coroutine = Follow(space);
        //    if (!hitState)
        //    {
        //        StartCoroutine(coroutine);
        //    }
        //} 
    } 
    IEnumerator Follow (float space)
    {
        yield return new WaitForSeconds(0.8f);
        if (space > 0 && space <= 16.0f)
        {
          //  transform.position = new Vector2(transform.position.x - 0.05f, transform.position.y);
          //  transform.RotateAround(GetComponent<Collider2D>().bounds.center, Vector3.forward, Time.deltaTime * 100);
            Move(new Vector2(-1.0f,0), clockWise);
            isChasingLeft = true;
           // Debug.Log("Chasing Left");

        }
        else if (space <= 0)
        {
            //  transform.position = new Vector2(transform.position.x + 0.05f, transform.position.y);
            //  transform.RotateAround(GetComponent<Collider2D>().bounds.center, -Vector3.forward, Time.deltaTime * 100);
            Move(new Vector2(1,0), clockWise);
            isChasingRight = true;
           // Debug.Log("Chasing Right");
        } 
        else if (space >= 8.0f)
        {
            yield break;
        }
    }
    
  //  IEnumerator Idle
  //  {

  //  }
    private void Move(Vector3 dir, float clockWise)
    {
        // + 1 for right, -1 for left
        //     transform.position = new Vector2(transform.position.x + 0.05f * dir, transform.position.y);
        dir *= clockWise;
        transform.position += (speed * dir);
        float angle = Time.deltaTime * 2000 * speed;
        transform.RotateAround(GetComponent<Collider2D>().bounds.center, 
            Vector3.forward * -clockWise, angle);
        transform.GetChild(0).RotateAround(GetComponent<Collider2D>().bounds.center,
                    Vector3.forward * clockWise, angle);
        //  GetComponentsInChildren<Transform>()[1].RotateAround(GetComponent<Collider2D>().bounds.center, 
        //      Vector3.forward * -dir, -angle);
        //  Debug.Log("Rotation = " + GetComponentsInChildren<Transform>()[1].rotation);

    } 

    private void TakeDamage(float damage)
    {
        Debug.Log("Taking Damage");
        health -= damage;
       


        if (health <= 0)
        {
            Die();
        }
    }  

    private void SpawnLaser()
    {
        GameObject LaserMaster = transform.GetChild(0).gameObject;
        Transform laserEnd = LaserMaster.transform;

        
        
   //     Debug.Log("HeadLength = " + headLength + "\nBodyLength = " + bodyLength);

        LayerMask mask = LayerMask.GetMask("Ground", "Player");
        Vector3 eulerRot = laserEnd.rotation.eulerAngles;
    //    Debug.Log("Original Euler Rotations are " + eulerRot);
        eulerRot += new Vector3(0, 0, 90);
        float rayDist = (laserEnd.position - transform.position).magnitude;
        eulerRot.z *= (Mathf.PI / 180);
        Vector3 direction = new Vector3(Mathf.Cos(eulerRot.z), 
            Mathf.Sin(eulerRot.z), 0);
 //       Debug.Log("Direction = " + direction.x + "," + direction.y);
        RaycastHit2D lasRay = Physics2D.Raycast(BallCollide.bounds.center, direction,
            200, mask);
        Debug.DrawRay(BallCollide.bounds.center, direction);
        if (lasRay.collider != null)
        {
            if (lasRay.collider.CompareTag("Player")) {
                if (!player.GetComponent<PlayerHealthScript>().GetIsHitStunned())
                {
                    string playerState = player.GetComponent<ArtrobotController>().GetState();
                    if (playerState.Equals("swinging") || playerState.Equals("chaining"))
                    {
                        Debug.Log("Starting fall off swing");
                        player.GetComponent<ArtrobotController>().FallOffSwing();

                    }
                    player.GetComponent<ArtrobotController>().KnockBack();
                    player.GetComponent<PlayerHealthScript>().DamageOrHealth(-1);

                    player.GetComponent<WowController>().Impact();
                }
            }
  //          Debug.Log("Raycast has hit a target");
            if (!Mathf.Approximately(lasRay.distance, lastRayLength))
            {
                LaserMaster.transform.position = lasRay.point;
                float bodyScale = ((lasRay.distance - headLength) / bodyLength);
                laserGroup.Peek().GetComponent<Transform>().localScale =
                    new Vector3(1, bodyScale, 1);
                //Debug.Log("Laser End Pos = " + (LaserMaster.transform.position - (lasRay.distance
                //    * direction)));
                //Debug.Log("lasLength = " + lasRay.distance);
                //Debug.Log("Actual Laser Length = " + (headLength + bodyScale * bodyLength));
            }

            else
            {
  //              Debug.Log("Ray is same length as previous ray");
            }

            lastRayLength = lasRay.distance;
        }
        else
        {
    //        Debug.Log("Nothing was hit by laser raycast");
            //   Debug.Log("New Position = " + BallCollide.bounds.center +
            //       (direction * 20));
            LaserMaster.transform.position = BallCollide.bounds.center +
                (direction * 200);
         //   LaserMaster.transform.position = new Vector2(0, 0);
      //            Debug.Log("LaserMaster position = " + LaserMaster.transform.position);
            float lasLength = (LaserMaster.transform.position - BallCollide.bounds.center)
                .magnitude;
            float bodyScale = ((lasLength - headLength) / bodyLength);
            laserGroup.Peek().GetComponent<Transform>().localScale =
                new Vector3(1, bodyScale, 1);


            float scaleDiff = (laserGroup.Peek().GetComponent<Transform>().position -
                laserGroup.Peek().GetComponent<Transform>().GetChild(0).position).magnitude;


            lastRayLength = lasLength;
            
        }
 
        

    }

    private void SetState(string state)
    {
        if (!audio.isPlaying)
        {
            audio.clip = roll;
            audio.Play();
            audio.loop = true;
        }
        this.state = state;
    }

    private string GetState()
    {
        return state;
    }

    private void Die()
    {
        Destroy(gameObject, 0.5f);
    }

    private void ResetBall()
    {
        speed = originalSpeed;
        clockWise = 1;

    }


    IEnumerator RollBack()
    {
        Debug.Log("Starting RollBack");
        clockWise = -1;
        float origSpeed = speed;
        speed = originalSpeed * 1.5f;
        if (angleOffset != 0)
        {
            Debug.Log("Angle Offset = " + angleOffset);
            angleOffset = (90 - Mathf.Abs(angleOffset))
                * -(angleOffset / Mathf.Abs(angleOffset));
            nextState = ((int)clockWise + nextState) % 4;
            Debug.Log("Angle Offset = " + angleOffset);
        }
        float count = 0;

        while (speed > 0)
        {
            speed -= (0.01f);
            yield return new WaitForSeconds(0.1f);
        }
        

        clockWise *= -1;
        
        if (angleOffset != 0)
        {
            Debug.Log("Angle Offset = " + angleOffset);
            angleOffset = (90 - Mathf.Abs(angleOffset))
                * -(angleOffset / Mathf.Abs(angleOffset));
            nextState = ((int)clockWise + nextState) % 4;
            Debug.Log("Angle Offset = " + angleOffset);
        }

        while (speed < originalSpeed)
        {
            speed += (0.01f);
            yield return new WaitForSeconds(0.1f);

        }

        //Debug.Log("Rolling Back Now");
        //hitState = true;
        //int playerDirection = 0;
        //if (space > 0)
        //{
        //    playerDirection = 1;
        //}
        //else
        //{
        //    playerDirection = -1;
        //}

        //for (float i = 0; i < 60; i ++)
        //{
        //    transform.position = new Vector2(transform.position.x + 0.03f * playerDirection * power, transform.position.y);
        //    transform.RotateAround(GetComponent<Collider2D>().bounds.center, Vector3.forward * -playerDirection, Time.deltaTime * 1000);
        //    yield return null;
        //}
        //hitState = false;
    }
    IEnumerator DamageFade()
    {
        Debug.Log("Starting Flash");
        //Color fade = new Color(255f, 0f, 0f);
        //Color origColor = GetComponent<Renderer>().material.color;
        //GetComponent<Renderer>().material.color = redFlash;
        isFading = true;
        float fade = 1.0f;
        GetComponent<Renderer>().material.color = origColor;
        bool fadingDown = true;
        

        for (float i = 0; i < 60; i ++)
        {
            Color c = GetComponent<Renderer>().material.color;
            if (fadingDown)
            {
                if (fade > 0)
                {
                    fade -= 0.08f;
                    c.g = fade;
                    c.b = fade;
                    GetComponent<Renderer>().material.color = c;
                    Debug.Log("Color is " + c);
                }
                else
                {
                    fadingDown = false;
                }
            }
            else
            {
                if (fade <= 1)
                {
                    fade += 0.08f;
                    c.g = fade;
                    c.b = fade;
                    GetComponent<Renderer>().material.color = c;
                    Debug.Log("Color is " + c);
                }
                else
                {
                    fadingDown = true ;
                }
            }
            yield return null;

        }
        GetComponent<Renderer>().material.color = origColor;
        isFading = false;
    }

    IEnumerator LaserFlash()
    {
        var head = laserGroup.ToArray()[1];
        var body = laserGroup.Peek();

        Color headColor = head.GetComponent<Renderer>().material.color;
        Color bodyColor = body.GetComponent<Renderer>().material.color; 

        float H, S, V;

        Color.RGBToHSV(headColor, out H, out S, out V);

        float origSat = S;

        while (this != null)
        {
            for (float i = S; i < 1; i += 0.05f)
            {
                S = i;
                headColor = Color.HSVToRGB(H, S, V); 
                bodyColor = Color.HSVToRGB(H, S, V);
                head.GetComponent<Renderer>().material.color = headColor;
                body.GetComponent<Renderer>().material.color = bodyColor;
        //        Debug.Log("Increasing Saturation \nHead Color = " + headColor);
                yield return new WaitForSeconds(0.01f);
            } 
            for (float i = S; i > origSat; i -= 0.05f)
            {
                S = i;
                headColor = Color.HSVToRGB(H, S, V);
                bodyColor = Color.HSVToRGB(H, S, V);
                head.GetComponent<Renderer>().material.color = headColor;
                body.GetComponent<Renderer>().material.color = bodyColor;
  //              Debug.Log("Decreasing Saturation \nHead Color = " + headColor);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
  //      Debug.Log("Enemy Bullet collision detected");
        if (collision.gameObject.CompareTag("Bullet"))
        {
            //        Debug.Log("Bullet-Bullet collision detected");
            //     GetComponent<BallEnemyAudioScript>().SwordAttack();
            AudioClip impact = (AudioClip)Resources.Load("SoundFX/" +
                "zapsplat_impact_metal_thin_001_10694");
            audio.clip = impact;
            audio.Play();
            audio.loop = false;
            StopCoroutine("RollBack");
            StopCoroutine("DamageFade");
            StartCoroutine("RollBack");
            StartCoroutine("DamageFade");
            
            TakeDamage(1);
            //Destroy(gameObject);         
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Sword"))
        {
            Debug.Log("Sword strike detected");
            GetComponent<BallEnemyAudioScript>().SwordAttack();
            StopCoroutine("RollBack");
            StopCoroutine("DamageFade");
            StartCoroutine("RollBack");
            StartCoroutine("DamageFade");
            
            TakeDamage(1);
            // Destroy(gameObject);
        }
        if (collision.tag.Equals("SurfaceShape"))
        {
            Debug.Log("Ball colliding with platform suface");
            if (PlatBox == null)
            {
                PlatBox = collision.gameObject.GetComponent<BoxCollider2D>();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
    //    Debug.Log("Colliding with " + collision.name);
        
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 12)
        {
            Debug.Log("Ball leaving collision with platform surface");
            PlatBox = null;
        }
    }
}
