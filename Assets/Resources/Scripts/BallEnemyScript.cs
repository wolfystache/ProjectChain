using System.Collections;
using System.Collections.Generic;
//using System;
using System;
using UnityEngine; 

public class BallEnemyScript : Enemy
{

    public GameObject player;

    private bool playerDetected;
    private bool isChasingRight;
    private bool isChasingLeft;
    private bool hitState;
    private AudioClip roll;

    float count;
    private Vector2 circPattern;
    private Vector2 offset;
    private float chaseOffset;
    
    private float space;
    private Quaternion lightOrigin;
    private CircleCollider2D BallCollide;
    private BoxCollider2D PlatBox;
    private const string top = "top", right = "right", left = "left",
        bottom = "bottom", cornering = "cornering";
    private string[] surfaceStates = { top, right, bottom, left };
    private int nextState;
    private float clockWise = 1;
    private float originalSpeed = 0.1f;
    private float speed;
    private Vector2 pivotPos = Vector2.zero;
    private float angleOffset = 0;

    private GameObject laserHead;
    private GameObject laserBody;

    float headLength;
    float bodyLength;



    private float lastRayLength;

    private Stack<GameObject> laserGroup;



    // Use this for initialization
    public override void Start()
    {
        base.Start();
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

        damageCaused = -1;
   
        //  lightOrigin = GetComponentsInChildren<Transform>()[1].rotation;



    }

    private void Update()
    {
       
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Debug.Log("Ball State = " + state);
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

        if (!(state.Equals(idle) || state.Equals(falling)))
        {
            SpawnLaser();
        } 

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
        rgdBdy.MovePosition(rgdBdy.position += localOffset);

     //   Debug.Log("Cage Enemy Position is: " + transform.position);



        Vector2 playerPos = player.transform.position;
        Vector2 center = GetComponent<Collider2D>().bounds.center;
        Vector2 enemyPos = transform.position;
        space = center.x - playerPos.x;
  
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
                    player.GetComponent<PlayerHealthScript>().DamageOrHealth(damageCaused);

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
            LaserMaster.transform.position = BallCollide.bounds.center +
                (direction * 200);
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

    public override void SetState(string state)
    {
        base.SetState(state);
        if (!audio.isPlaying && !state.Equals(idle))
        {
            audio.clip = roll;
            audio.Play();
            audio.loop = true;
        }
    }


    public override void Die()
    {
        base.Die();
        //  Destroy(gameObject, 0.5f);
        Debug.Log("Ball has died. RIP");
        foreach (GameObject g in laserGroup)
        {
            Destroy(g);
        }

        if (state.Equals(top))
        {
            SetState(idle);
        }
        else
        {
            SetFallState(false);
        }
      
      
        
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
       // float count = 0;

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

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        //      Debug.Log("Enemy Bullet collision detected");
    //    base.OnTriggerEnter2D(collision);
        if (collision.IsTouching(GetComponents<Collider2D>()[1]))
        {
            
            if (collision.gameObject.tag.Equals("SurfaceShape"))
            {
                Debug.Log("Ball colliding with platform suface");
                if (PlatBox == null)
                {
                    PlatBox = collision.gameObject.GetComponent<BoxCollider2D>();
                }
            }
            if (collision.gameObject.CompareTag("Bullet") && !isFading)
            {
                //        Debug.Log("Bullet-Bullet collision detected");
                //     GetComponent<BallEnemyAudioScript>().SwordAttack();
               
                StopCoroutine("RollBack");
                StopCoroutine("DamageFade");
                StartCoroutine("RollBack");
                StartCoroutine("DamageFade");

                TakeDamage(1);
                //Destroy(gameObject);         
                Destroy(collision.gameObject);
            }
            else if (collision.gameObject.CompareTag("Sword") && !isFading)
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
            else if (collision.gameObject.CompareTag("Player"))
            {
                if (!PlayerHealthScript.player.GetIsHitStunned())
                {


                    if (ArtrobotController.player.GetSubState().Equals("sword"))
                    {
                        ArtrobotController.player.SetSubState("none");
                        ArtrobotController.player.GetComponentInChildren<SwordController>().Reset();
                        ArtrobotController.player.anim.SetBool("sword", false);
                    }

                    PlayerHealthScript.player.DamageOrHealth(damageCaused);
                    if (ArtrobotController.player.state.Equals("swinging") ||
                        ArtrobotController.player.state.Equals("chaining"))
                    {
                        Debug.Log("Starting fall off swing");
                        ArtrobotController.player.FallOffSwing();
                    }
                    else
                    {
                        ArtrobotController.player.KnockBack();
                    }
                    WowController.player.Impact();
                }

            }


        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //    Debug.Log("Colliding with " + collision.name);
       

    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
      

        Debug.Log("Colliding with " + collision.gameObject);
        

        if (collision.gameObject.layer == 12)
        {
            if (GetState().Equals(falling))
            {
                SetState(idle);
                foreach (Collider2D c in GetComponents<Collider2D>())
                {
                    c.enabled = false;
                }
            }
        } 
        //if (collision.gameObject.layer == 9)
        //{
        //    Collider2D[] playerCollider = new Collider2D[ArtrobotController.artTrans.
        //    GetComponent<Rigidbody2D>().attachedColliderCount];
        //    ArtrobotController.artTrans.
        //        GetComponent<Rigidbody2D>().GetAttachedColliders(playerCollider);
        //    Collider2D rgdCollider = GetComponents<Collider2D>()[0];
        //    Debug.Log("Collider count = " + ArtrobotController.artTrans.GetComponent<Rigidbody2D>().attachedColliderCount);



        //    foreach (Collider2D c in playerCollider)
        //    {
        //        Debug.Log("Ignore Collision = " + Physics2D.GetIgnoreCollision(rgdCollider, c));
        //    }
        //    Debug.Log("Ball collision with " + collision.gameObject.name);
        //}
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Ball is currently colliding with " + collision.gameObject.name);
       
    }

    public override void OnCollisionExit2D(Collision2D collision)
    {
        //if (collision.gameObject.layer == 12)
        //{
        //    Debug.Log("Ball leaving collision with platform surface");
        //    PlatBox = null;
        //}
    }
}
