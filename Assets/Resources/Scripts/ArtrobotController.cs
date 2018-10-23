using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArtrobotController : MonoBehaviour {


    public GameObject bulletPreFab;
    public Transform bulletSpawn;
    public GameObject chain;
    public GameObject background;
    public Transform climbingSpawn;


    float maxSpeed;
    public int lastDir;
    float lastMove;
    Vector2 lastVel;

    private bool isAimUp;
    private float width;
    private float height;
    private int FixedCount = 0;
    private Quaternion turnAngle;

    Vector2 end;
    Vector2 origin;
    Vector2 lastPos;
    private bool isFacingRight;
    private bool isCrouching;
    float move;
    private bool isHitStunned;
    private bool isChaining;
    private bool isClimbing;
    private bool gravityOn;
    private float radius;
    private float lastAngle;

    private Vector2 leftStickInput;
    private Vector2 rightStickInput;
    private Vector2 dPadInput;
    private Vector2 triggInput;

    private Vector2 input;
    private Vector2 aimDir;
    private const string standing = "standing", running = "running", climbing = "climbing",
        chaining = "chaining", aiming = "aiming", climbingAiming = "climbingAiming",
        jumping = "jumping", falling = "falling", paused = "paused",
        swinging = "swinging";
    private string state;
    private string substate;
    private bool isTravelingUp;
    private bool isTravelingDown;
    private bool startFall;
    private float bottomHalf;
    private bool isLeaning = false;
    private GameObject buttonPrompt;
    private bool pullUpZone = false;
    private GameObject top;
    private float fallTime;
    private float fallPosition;
    private bool isColliding = false;
    private Rigidbody2D rgdBdy;
    private bool isRidingPlatform = false;
    private Vector2 platformSpeed;
    private string pausedState;
    private bool justTurnedAround = false;
    private Vector2 offset;
    private GameObject ridingPlatform;
    private BoxCollider2D climbCollide;

    private List<Vector2> prevPos;
    private List<Vector2> prevTriggVal;

    private Dictionary<string, int> CollideMap;

    public Physics playerPhysics;

    Animator anim;

    // Use this for initialization
    void Start()
    {

        //  audio = GetComponent<AudioSource>();  
        isAimUp = false;
        maxSpeed = 7.5f;
        lastMove = Input.GetAxis("Horizontal");
        lastVel = GetComponent<Rigidbody2D>().velocity;
        lastDir = 1;
        anim = GetComponent<Animator>();
        width = Camera.main.pixelWidth;
        height = Camera.main.pixelHeight;
        end = Camera.main.ScreenToWorldPoint(new Vector2(width, height));
        origin = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        bottomHalf = (end.y + origin.y) / 2.0f;
        lastPos = transform.position;
        isFacingRight = true;
        move = 0;
        isCrouching = false;
        isHitStunned = false;
        isChaining = false;
        isClimbing = false;
        gravityOn = true;
        lastAngle = 0;
        aimDir = new Vector2(1, 0);
        state = falling;
        substate = "none";
        startFall = true;
        StartCoroutine("StartFall");
        rgdBdy = GetComponent<Rigidbody2D>();
        platformSpeed = new Vector2();
        offset = new Vector2();
        prevPos = new List<Vector2>();
        prevTriggVal = new List<Vector2>();

        transform.RotateAround(GetComponents<Collider2D>()
            [1].bounds.center, Vector3.up, 180.0f);
        turnAngle = transform.rotation;
        transform.rotation = Quaternion.identity;

        playerPhysics = new Physics(name);

        CollideMap = new Dictionary<string, int>();

    }

    void Update()
    {

        //     Debug.Log("Change in Y Player = " + (transform.position.y - lastPos.y));

        //  Debug.Log("IsFacingRight = " + isFacingRight);
        Debug.Log("State = " + state);
        
        leftStickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rightStickInput = new Vector2(Input.GetAxis("RightHorizontal"),
            Input.GetAxis("RightVertical"));
        if (rightStickInput.magnitude > 0.99f)
        {
            rightStickInput = Vector3.Normalize(rightStickInput);
            GetAimAngle();
        }
        if (leftStickInput.magnitude > 0.99f)
        {
            leftStickInput = Vector3.Normalize(leftStickInput);
        }


      //  Debug.Log("AimDir = " + aimDir);
        Vector2 rawRightStick = new Vector2(Input.GetAxisRaw("RightHorizontal"),
           Input.GetAxisRaw("RightVertical"));
        //Debug.Log("Raw Input = (" + rawRightStick.x + "," + rawRightStick.y +
        //    ")");
        //Debug.Log("Raw Magnitude = " + rawRightStick.magnitude);
        //Debug.Log("Post-Raw Input = (" + rightStickInput.x + "," + rightStickInput.y +
        //    ")");
        //Debug.Log("Post Raw Magnitude: " + rightStickInput.magnitude);
        

        if (Mathf.Approximately(rightStickInput.x, rightStickInput.y))
        {
       //     Debug.Log("X and Y approximately Equal\n" + rightStickInput.x
       //         + "," + rightStickInput.y);
        }

        dPadInput = new Vector2(Input.GetAxis("DPadHoriz"), Input.GetAxis("DPadVert"));

        triggInput = new Vector2(Input.GetAxis("L2"), Input.GetAxis("R2"));
        if (prevTriggVal.Count == 2)
        {
            prevTriggVal.RemoveAt(0);
            prevTriggVal.Add(triggInput);
    //        Debug.Log("prevTriggVal[0] = " + prevTriggVal[0]);
    //        Debug.Log("prevTriggVal[1] = " + prevTriggVal[1]);
        }
        else
        {
            prevTriggVal.Add(triggInput);
        }


 //       Debug.Log("Right Trigger val = " + Input.GetAxisRaw("R2"));
 //f       Debug.Log("GetR2Down = " + GetR2Down());

        if (leftStickInput.magnitude >= dPadInput.magnitude) {
            input = leftStickInput;
        }
        else
        {
            input = dPadInput;
        }
        //    Debug.Log("Velocity = " + GetComponent<Rigidbody2D>().velocity);
        //    Debug.Log("State = " + state);
        //     Debug.Log("IsColliding = " + isColliding);
        //   Debug.Log("IsTravelingVert = " + IsTravelingVert());
        Vector2 currPos = transform.position;
        //    Debug.Log("Y Pos is " + currPos.y);

        switch (state)
        {
            case standing:
                move = input.x;
                break;
            case chaining:
                move = 0;
                break;
            case climbing:
                break;
            //case aiming:
            //    move = 0;
            //    GetComponentInChildren<AimArmController>().aim(0);
            //    break;
            case climbingAiming:
                move = 0;
                GetComponentInChildren<AimArmController>().aim(0);
                break;
            case jumping:
                move = input.x;
                break;
            case falling:
                move = input.x;
                break;
            default:
                break;

        }
        switch (substate)
        {
            case aiming:
                GetComponentInChildren<AimArmController>().aim(0);
                break;
            default:
                break;
        }

        //   Debug.Log("pos = " + transform.position.x);
      //  if (!GetState().Equals(climbing)) { //fallPhysics(); }

        //  Debug.Log("Velocity = " + GetComponent<Rigidbody2D>().velocity);
        if (isCrouching)
        {
            if (Mathf.Abs(move) < 0.001f)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("ProneWalk"))
                {
                    anim.enabled = false;
                }
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (pullUpZone)
            {
                if (GetTopLedge() != null)
                {
                    Debug.Log("Climbing ledge");
                    ClimbLedge(GetTopLedge());
                    pullUpZone = false;
                }
            }
            else if ((!(GetState().Equals(climbing) && !GetState().Equals(jumping) && !isLeaning)) 
                && (!(GetState().Equals(jumping) || GetState().Equals(falling))))
            {
                StartCoroutine("Jump");
            }
            //   Debug.Log("Jump Pressed!!");
        }

        if (!Input.GetButton("Jump"))
        {
            if (GetState().Equals(jumping))
            {
                Debug.Log("Bailing on jump");
           //     SetFallState(false);
          //      StopCoroutine("Jump");
              //  SetFallState();
            }
        }


        if (GetR2Down())


        {
            Shoot(isAimUp);

        }

        if (Input.GetButtonDown("LookUp"))
        {

        }
        else if (Input.GetKeyUp("up") || Input.GetKeyUp("w") || Input.GetButtonUp("LookUp"))
        {


        }

        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("SwordAttack"))
        {
            //   Debug.Log("Sword attack has stopped");
            anim.SetBool("sword", false);
            if (Input.GetButtonDown("Sword"))
            {
                Sword();
            }
        }


      

        //   if (Input.GetButtonDown("Chain"))
        if (GetL2Down())
        {
            Debug.Log("Shooting Chain");
            //  if (!(state.Equals(aiming) || (state.Equals(climbingAiming))))
            Debug.Log(state);
            
            if (state.Equals(swinging) || state.Equals(chaining))
            {

                FallOffSwing();
                
       //         chain.GetComponent<ChainController>().ChangeSwingAngle();
       //         float speed = chain.GetComponent<ChainController>().GetSpeed() * 6;            
       //         chain.GetComponent<ChainController>().SetSpeed(speed);
       //         if (state.Equals(swinging))
       //         {
       //             chain.GetComponent<ChainController>().SwingOff();
       //         }
       //         chain.GetComponent<ChainController>().ChainReturn();
       //         StartCoroutine("RotateAfterRetraction");
               
       ////         transform.rotation = Quaternion.identity;
       //         SetFallState(true);
               
            }
            else
            {
               // if (state.Equals(standing) || state.Equals(climbing))
                  if (!GetSubState().Equals(aiming) && GetState().Equals(standing))
                {
                    anim.SetBool("Chain", true);
                }
                if (state.Equals(jumping) || state.Equals(falling))
                {
                    GetComponentInChildren<AimArmController>().setSprite(false);
                    anim.SetBool("isAiming", true);
                    if (isFacingRight)
                    {
                        GetComponentInChildren<AimArmController>().aim(45);
                           SetAimDir(new Vector2(Mathf.Sqrt(2) / 2, Mathf.Sqrt(2) / 2));
                      //  SetAimDir(new Vector2(0.5f, Mathf.Sqrt(3) / 2));
                    }
                    else
                    {
                        GetComponentInChildren<AimArmController>().aim(135);
                         SetAimDir(new Vector2(-Mathf.Sqrt(2) / 2, Mathf.Sqrt(2) / 2));
                       // SetAimDir(new Vector2(-0.5f, Mathf.Sqrt(3) / 2));
                    }

                    anim.SetBool("Jumped", false);
                    anim.SetBool("IsFalling", false);
                    Debug.Log("AimDir = " + aimDir);
                }
                else
                {
                  //  SetState(chaining);
                }
                GetComponentInChildren<AimDotController>().StopSights();
                     
                SetSubState(chaining);
                chain.GetComponent<ChainController>().ShootChain();
            }
        }
        if (Input.GetButtonDown("Chain") && 
            chain.GetComponent<ChainController>().GetState().Equals("pulling"))
        {
            chain.GetComponent<ChainController>().Swing();
            SetSubState("none");
        }
        if (Input.GetButtonDown("Start"))
        {
            Debug.Log("State = " + GetState());
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //if (!state.Equals(paused))
            //{
            //    pausedState = GetState();
            //    SetState(paused);
            //    Time.timeScale = 0;

            //}
            //else
            //{
            //    Time.timeScale = 1;
            //    SetState(pausedState);
            //}
        }
     //   Debug.Log("Right Stick Magnitude = " + rightStickInput.magnitude);
        if (( rightStickInput.magnitude >= 0.999f || Mathf.Abs(rightStickInput.x) == 1 ||
            Mathf.Abs(rightStickInput.y) == 1) &&
            //  !(GetSubState().Equals(aiming) || GetState().Equals(chaining) ||
            !(GetSubState().Equals(aiming) || GetSubState().Equals(chaining) ||
            GetState().Equals(swinging)))
        {

            anim.SetBool("isAiming", true);
            if (GetState().Equals(climbing))
            {

            //    SetState(climbingAiming);
                SetSubState(aiming);
                float lastAngle = 180;
                aimDir = new Vector2(-1, 0);
                if (!isFacingRight)
                {
                    aimDir = new Vector2(1, 0);
                    lastAngle = 0;
                }
                GetComponentInChildren<AimArmController>().setLastAngle(lastAngle);
                GetComponentInChildren<AimArmController>().setSprite(true);
                Debug.Log("Calling Aim from Artrobot");
                
                GetComponentInChildren<AimDotController>().StartSights();


            }
            else
            {

            //    float lastAngle = 0;
                if (!isFacingRight)
                {
            //        lastAngle = 180;
                }

             //   Debug.Log("aimDir = " + aimDir);

                GetComponentInChildren<AimArmController>().setLastAngle(lastAngle);

                //    Debug.Log("Looking up");
                //  if (isF)
                isAimUp = true;
                //      SetState(aiming);
                SetSubState(aiming);
                GetComponentInChildren<AimArmController>().setSprite(false);
                GetComponentInChildren<AimArmController>().aim(0);
                Debug.Log("Spawning Sights");
                GetComponentInChildren<AimDotController>().StopSights();
                GetComponentInChildren<AimDotController>().StartSights();
            }


            //Debug.Log("Moving Camera now");
            //float rightStickVal = Input.GetAxis("RightVertical");
            //Camera.main.GetComponent<CameraController>().LookUp(rightStickVal);
        } 
        else if (rightStickInput.magnitude < 0.99f)
        {
            
            if (!(GetSubState().Equals(chaining) || GetState().Equals(swinging)))
            {
       //         Debug.Log("RightStick back to DZ. Resetting Aim");
                AimReset();

            }
        }

        lastMove = Input.GetAxis("Horizontal");
        lastVel = GetComponent<Rigidbody2D>().velocity;
        if (lastMove < 0)
        {
            lastDir = 0;
        }
        else if (lastMove > 0)
        {
            lastDir = 1;
        }
       
        if (isCrouching)
        {
            move *= 0.25f;
        }

   

    }

    // Update is called once per 
    void FixedUpdate()
    {
   //     Debug.Log("IsFacingRight = " + isFacingRight);
        FixedCount++;

        Vector2 currPos;
        Vector2 fixedPos = rgdBdy.position;
        Vector2 localOffset = new Vector2();
        Debug.Log("Fixed State = " + state);
        //Debug.Log("SubState = " + substate);
        switch (state)
        {
            case falling:
                currPos = transform.position;
                //fixedPos = new Vector2(rgdBdy.position.x, 
                //    Physics.Gravity(fallTime, fallPosition, rgdBdy.position).y);
                localOffset += playerPhysics.Gravity(transform.position);
               // localOffset += new Vector2(0, -10.0f * Time.deltaTime);
                if (chain.GetComponent<ChainController>().GetState().Equals("idle"))
                {
                    anim.SetBool("IsFalling", true);
                }
                    
           //     Debug.Log("After grav position = " + rgdBdy.position.y);
                break;
            case jumping:
                currPos = transform.position;
                //        Debug.Log("Jumping now");
                //        Debug.Log("Y position = " + currPos.y);
                //fixedPos =  new Vector2(rgdBdy.position.x,
                //    rgdBdy.position.y + (10.0f * Time.deltaTime));
                //  localOffset += new Vector2(0, 10.0f * Time.deltaTime);
                localOffset += playerPhysics.Gravity(transform.position);  
                if (playerPhysics.StartedFalling())
                {
                    SetFallState(true);
                }



                break;
            case climbing:
                isClimbing = true;
                isLeaning = false;
                anim.speed = 1;
                float vertMove = input.y;
                //BoxCollider2D topBox = GetComponents<BoxCollider2D>()[1];
                //BoxCollider2D botBox = GetComponents<BoxCollider2D>()[0]; 

                
                BoxCollider2D box = GetComponents<BoxCollider2D>()[3];


            //    if (ridingPlatform != null)
            //    {
              //      Debug.Log("Riding Platform name = " + ridingPlatform.name);
                   // BoxCollider2D platformCol = ridingPlatform.GetComponent<BoxCollider2D>();
             //       Debug.Log("TopBox Max = " + topBox.bounds.max.y +
             //           "\nPlatform Top = " + platformCol.bounds.max.y);
                    if ((box.bounds.max.y <= climbCollide.bounds.max.y || vertMove < 0) &&
                        (box.bounds.min.y >= climbCollide.bounds.min.y || vertMove > 0))
                    {

                        //  fixedPos = new Vector2(rgdBdy.position.x, (rgdBdy.position.y + (vertMove * 0.07f))
                        localOffset += new Vector2(0, vertMove * 0.07f);
                       //        );
                    }
                    //else
                    //{
                    //    fixedPos = rgdBdy.position;
                    //}
           //     }

                
               
                anim.SetFloat("climbSpeed", Mathf.Pow(Mathf.Abs(vertMove), 0.2f));
                anim.SetBool("Jumped", false);
                if (isFacingRight && input.x >= 0 || !isFacingRight && input.x <= 0)
                {
                    anim.SetBool("isLeaning", false);
                    move = 0;
                }
                else if (isFacingRight && input.x < -0.3f || !isFacingRight && input.x > 0.3f)
                {
                    isLeaning = true;
                    // move = input.x;
                    move = 0;
                    anim.SetBool("isLeaning", true);
                    anim.speed = 0;

                }
                else
                {
                    anim.SetBool("isLeaning", false);

                }
                break;
            case chaining:
                //   isRidingPlatform = false;
                //    fixedPos += new Vector2(0, Physics.Gravity(fallTime, 
                //         fallPosition, rgdBdy.position).y);
                //fixedPos += new Vector2(0, Physics.Gravity(fallTime,
                //         fallPosition, rgdBdy.position).y);
                move = 0;
                break;
            case aiming:
         //       fixedPos += new Vector2(0, Physics.Gravity(fallTime,
         //           fallPosition, rgdBdy.position).y);
                break;

            case swinging:
                move = 0;
                break;
            default:
        //        fixedPos = new Vector2(rgdBdy.position.x + (move * 10f * Time.deltaTime),
        //            rgdBdy.position.y);
                break;
            
        }
        switch (substate)
        {
            case aiming:
                move = 0;               
                break;
            case chaining:
                move = 0;
                break;
            default:
                break;
        }
    //    Debug.Log("move = " + move);
        localOffset += new Vector2((move * 10f * Time.deltaTime), 0);
        fixedPos += localOffset;
        chain.GetComponent<ChainController>().SetOffset(localOffset);
        
   //     Debug.Log("localOffset = " + localOffset.x + "," + localOffset.y);
        if (isRidingPlatform)
        {
            fixedPos += platformSpeed;
        }
        else
        {
        //    Debug.Log("Is No Longer Riding Platform");
        }
        fixedPos += offset;
        rgdBdy.MovePosition(fixedPos);
        //   transform.position = fixedPos;
        offset = Vector2.zero;

        if (move < 0 && isFacingRight || move > 0 && !isFacingRight)
        {
            //   GetComponent<AimController>().turnAround();
            Debug.Log("Turning Around");
            TurnAround(1);
        }
    }

    void LateUpdate()
    {
        float half = width / 2.0f;
        float quart = width / 4.0f;
        float heightHalf = height / 2.0f;
        float height10th = height / 10.0f;

     

        if (IsTravelingVert() == -1 && GetState().Equals(standing))
        {
          //  Debug.Log("Falling, turning Gravity High");
        //    transform.GetComponent<Rigidbody2D>().gravityScale = 5.0f;
        }
        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
        // Debug.Log("Pos = " + pos.x);
        //  Debug.Log("Is Facing Right: " + isFacingRight);
        //  Debug.Log("Right constraint = " + (half - 0.001));
        int horizDir = IsTravelingHoriz();
        // Scroll the camera when character gets to edge of screen

       // if ((pos.x >= (half)) && isFacingRight)
        if ((pos.x >= (half) && horizDir == 1))
        {
       //   Debug.Log("Scrolling Right");
            Camera.main.GetComponent<CameraController>().ScrollRight();
        }
        if (pos.x <= (quart) && horizDir == -1)
        {
      //   Debug.Log("Scrolling Left");
            Camera.main.GetComponent<CameraController>().ScrollLeft();
        }
        // if (pos.x == half)
        // {
        ////     Debug.Log("Half point reached");
        // }
        // if (pos.x == quart)
        // {
        // //    Debug.Log("Quarter point reached");
        // }
    //    Debug.Log("IsTravelingDir = " + IsTravelingVert());
     //   Debug.Log("Current Pos = " + transform.position.y);
     //   Debug.Log("Last Pos = " + lastPos.y);

        int travelingDir = IsTravelingVert();
        if (pos.y >= heightHalf && travelingDir == 1 && !startFall)
        {
            Camera.main.GetComponent<CameraController>().ScrollUp();
        }
        else if (pos.y <= heightHalf && travelingDir == -1 && !startFall 
            && transform.position.y > bottomHalf)
        {
            Debug.Log("Scrolling Down");
            Camera.main.GetComponent<CameraController>().ScrollDown();
        }
    //    Debug.Log("X velocity is " + GetComponent<Rigidbody2D>().velocity.x);
        anim.SetFloat("move", Mathf.Abs(move));
        anim.SetFloat("speed",  Mathf.Pow(Mathf.Abs(move),0.2f));

        //   Debug.Log("anim speed = " + anim.speed); 

        if (Input.GetButtonDown("Crouch"))
        {
            Vector3 currPos = GetComponent<Transform>().position;
            isCrouching = !isCrouching;
            if (isCrouching)
                
            {
                StartCoroutine("CrouchToggle");
                anim.SetBool("isCrouched", true);
                


                Debug.Log("Crouching now");

            }
            else
            {
                StartCoroutine("CrouchToggle");
                Debug.Log("Standing now");
                anim.SetBool("isCrouched", false);                

            }
        }

        if (prevPos.Count == 2)
        {
            prevPos.RemoveAt(0);
            prevPos.Add(transform.position);
        }
        else
        {
            prevPos.Add(transform.position);
        }

    } 
    
    public void SetIsRiding(bool isRidingPlatform)
    {
   //     Debug.Log("Setting IsRidingPlatform to " + isRidingPlatform);
        this.isRidingPlatform = isRidingPlatform;
  //      Debug.Log("IsRidingPlatform = " + isRidingPlatform);
    }
    IEnumerator Jump()
    {
    
        if (isLeaning)
        {
            isClimbing = false;
            isLeaning = false;
            anim.speed = 1;
            anim.SetBool("isLeaning", false);
            anim.SetBool("isClimbing", false);
            anim.SetBool("Jumped", true);
            TurnAround(3);
            SetGravity(true);
            //  transform.parent = null;
            isRidingPlatform = false;
            anim.enabled = true;
            Debug.Log("Moving away from climbing surface");
           
        }
        else
        {
            anim.SetBool("Jumped", true);
            Debug.Log("Crouching now");
            Debug.Log("Pre-Jump Time: " + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("PreJump"));
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f);
            Debug.Log("Jump Time: " + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }
      //  Vector2 rigidbody2DVel = GetComponent<Rigidbody2D>().velocity;
        // GetComponent<Rigidbody2D>().velocity = new Vector2(rigidbody2DVel.x, 12.0f);
        SetState(jumping);
        float jumpTime = Time.realtimeSinceStartup;
        playerPhysics.StartPhysics(jumpTime, transform.position, 10.5f, 90.0f);
        yield return new WaitForSeconds(0.3f);
        if (GetState().Equals(jumping))
        {
            Debug.Log("End of Jump");
            SetFallState(true);
        }
     
      //  GetComponent<Rigidbody2D>().velocity = new Vector2(rigidbody2DVel.x, 0.0f);
      //  Debug.Log("Velocity = " + rigidbody2DVel);

    } 

    void Shoot(bool aimUp)
    {
        Debug.Log("Parent is " + transform.parent);
        if (GetState().Equals(standing) && !GetSubState().Equals(aiming)) 
        {
            StartCoroutine("ShootPose");
        }
        Transform spawn; 
        if (GetState().Equals(climbing))
        {
            spawn = climbingSpawn;
        }
        else
        {
            spawn = bulletSpawn;
        }
        var bullet = (GameObject)Instantiate(bulletPreFab,
            spawn.position, spawn.rotation);
                bullet.GetComponent<Rigidbody2D>().velocity =  aimDir * 50;
            
         
            Debug.Log("Bullet's position is " + bullet.GetComponent<Rigidbody2D>().velocity);
        
        bullet.GetComponent<Rigidbody2D>().gravityScale = 0;

        GetComponent<WowController>().BulletSound();
        Destroy(bullet, 2.0f);

    } 

    public void fallPhysics()
    {
        Vector2 rigidBody2DVel = GetComponent<Rigidbody2D>().velocity;

        int GroundCollisions;
        CollideMap.TryGetValue("12", out GroundCollisions);

        Debug.Log("GroundCollisions = " + GroundCollisions);

        
        if (GetState().Equals(standing) && IsTravelingVert() != 1 
            && GroundCollisions == 0 && !chain.GetComponent<ChainController>().GetState().Equals("pulling"))
        {
            
            Debug.Log("Falling Now");
            SetFallState(false);
          //  StartCoroutine("Gravity");
            
            //GetComponent<Rigidbody2D>().gravityScale = 10f; 
        } 
        else if (GroundCollisions != 0 && GetState().Equals(falling))
        {
            Debug.Log("Back on Ground, resetting gravity");
            SetState(standing);
            rgdBdy.velocity = new Vector2(0, 0);
           // GetComponent<Rigidbody2D>().gravityScale = 1f;
        }
    } 
    public void SetFallState(bool hasPhysics)
    {
        if (!hasPhysics)
        {
            fallTime = Time.realtimeSinceStartup;
            fallPosition = transform.position.y;
            playerPhysics.StartPhysics(fallTime, transform.position, 0, 0);
            
        }
        if (GetState().Equals(climbing))
        {
            anim.SetBool("isClimbing", false);
        }
        anim.SetBool("Jumped", false);
        Debug.Log("Starting to Fall");
        //    anim.SetBool("IsFalling", true);
        SetState(falling);
        
    } 

    
   
    void Sword()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        GetComponent<WowController>().Woosh();
        anim.SetBool("sword", true);
        GetComponentInChildren<SwordController>().HitBox();
        if (Mathf.Abs(move) > 0.001f)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

    }
    public bool IsFacingRight()
    {
        return isFacingRight;
    }

    public void SetChaining(bool isChaining)
    {
        this.isChaining = isChaining;
        SetState(chaining);
    }
    public bool GetChaining()
    {
        return isChaining;
    }
    public void SetClimbing(bool isClimbing)
    {
        this.isClimbing = isClimbing;
        SetState(climbing);
    }
    public void SetClimbing(bool isClimbing, GameObject surface)
    {
        
        this.isClimbing = isClimbing;
        anim.SetBool("IsFalling", false);
        if (isClimbing)
        {
            SetState(climbing);
            playerPhysics.StopPhysics();
            //transform.parent = surface.transform;
            //surface.GetComponent<ChainableController>().MovePlayer(true);
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            Debug.Log("Setting isClimbing on");
            anim.SetBool("isClimbing", true);
        }
        else
        {
            isRidingPlatform = false;
            surface.GetComponent<ChainableController>().MovePlayer(false);
        }
        //   surface.transform.position = new Vector2(2.0f,2.0f);

        SetGravity(false);
    }
    public bool GetClimbing()
    {
        return isClimbing;
    } 

    public void SetState(string state)
    {
        Debug.Log("Setting State from " + GetState() + " to " + state);
        if (GetState().Equals(climbing) || state.Equals(climbing))
        {
            BoxCollider2D[] boxes = GetComponents<BoxCollider2D>();
            if (GetState().Equals(climbing) && !state.Equals(climbing))
            {
                if (!GetSubState().Equals(chaining))
                {
                    anim.SetBool("isClimbing", false);
                }
                boxes[0].enabled = true;
                boxes[1].enabled = true;
                boxes[3].enabled = false;
            }
            else if (!GetState().Equals(climbing) && state.Equals(climbing))
            {
                boxes[0].enabled = false;
                boxes[1].enabled = false;
                boxes[3].enabled = true;
            }
        }
        this.state = state; 
    }

    public string GetState()
    {
        return state;
    } 

    public void SetSubState(string substate)
    {
        this.substate = substate;
    }

    public string GetSubState()
    {
        return substate;
    }
    public void AddOffset (Vector2 offset)
    {
        this.offset += offset;
    }

    public Vector2 GetRadialInput()
    {
        float horizMove = rightStickInput.x;
        float vertMove = rightStickInput.y;

        


        Vector2 radialInput = new Vector2(horizMove, vertMove);
        float dead = 0.15f;
        if (rightStickInput.magnitude <= dead)
        {
            //    Debug.Log("In dead zone");
            rightStickInput = new Vector2(0, 0);
        }
        else if (rightStickInput.magnitude >= 0.999f ||
            Mathf.Abs(rightStickInput.x) == 1 ||
            Mathf.Abs(rightStickInput.y) == 1)
        {
            aimDir = radialInput;
            //        Debug.Log("aimDir = (" + aimDir.x + ", " + aimDir.y + ")");
            //        Debug.Log("Magnitude: " + aimDir.magnitude);


            //   Debug.Log("Move = (" + horizMove + "," + vertMove + ")");
            // Y is locked to 1 or -1 and x is correct
            // if ((horizMove >= 0.99f && (vertMove) < 1 && (vertMove) > -1)
            //     || (horizMove < 0.99f && vertMove < 0.99f && horizMove > 0 && vertMove > 0))
            // {
            //       vertMove *= 45f;
            //    // vertMove *= 0.50.9444f;
            //     //    Debug.Log("VertMove = " + vertMove);
            //     Debug.Log("1");
            //     vertMove = Mathf.Sin(vertMove / (180f / Mathf.PI));
            //     horizMove = Mathf.Sqrt(1 - (vertMove * vertMove));
            // }

            // else if ((vertMove >= 0.99f && (horizMove) < 1 && (horizMove) > -1)
            //     || (horizMove > -0.99f && vertMove < 0.99f && horizMove < 0 && vertMove > 0))
            // {
            //     horizMove = (horizMove * -45f) + 90f;
            //     //  Debug.Log("horizMove = " + horizMove);
            //     Debug.Log("2");

            //     horizMove = Mathf.Cos(horizMove / (180f / Mathf.PI));
            //     vertMove = Mathf.Sqrt(1 - (horizMove * horizMove));
            // }
            // // X is locked to 1 or -1 and y is correct
            // else if ((horizMove <= -0.99f && (vertMove) < 1 && (vertMove) > -1)
            //     || (horizMove > -0.99f && vertMove > -0.99f && horizMove < 0 && vertMove < 0))
            // {
            //     vertMove = (vertMove * -45f) + 180f;
            //     Debug.Log("3");

            ////         Debug.Log("VertMove = " + vertMove);
            //     vertMove = Mathf.Sin(vertMove / (180f / Mathf.PI));
            //     horizMove = -Mathf.Sqrt(1 - (vertMove * vertMove));
            // }
            // else if ((vertMove <= -0.99f && (horizMove) < 1 && (horizMove) > -1)
            //         || (horizMove < 0.99f && vertMove > -0.99f && horizMove > 0 && vertMove < 0))
            // {
            //     horizMove = (horizMove * 45f) + 270f;
            //     Debug.Log("4");

            //        Debug.Log("horizMove = " + horizMove);
            //     horizMove = Mathf.Cos(horizMove / (180f / Mathf.PI));
            //     vertMove = -Mathf.Sqrt(1 - (horizMove * horizMove));
            // }
            // else if (horizMove == 1 && vertMove == 1)
            // {
            //     horizMove = vertMove = 45;
            //     horizMove = Mathf.Cos(vertMove / (180f / Mathf.PI));
            //     vertMove = Mathf.Sin(vertMove / (180f / Mathf.PI));
            //     Debug.Log("First Quad");
            // }
            // else if (horizMove == -1 && vertMove == 1)
            // {
            //     horizMove = vertMove = 135;
            //     horizMove = Mathf.Cos(vertMove / (180f / Mathf.PI));
            //     vertMove = Mathf.Sin(vertMove / (180f / Mathf.PI));
            //     Debug.Log("2nd Quad");

            // }
            // else if (horizMove == -1 && vertMove == -1)
            // {
            //     horizMove = vertMove = 225;
            //     horizMove = Mathf.Cos(vertMove / (180f / Mathf.PI));
            //     vertMove = Mathf.Sin(vertMove / (180f / Mathf.PI));
            //     Debug.Log("3rd Quad");

            // }
            // else if (horizMove == 1 && vertMove == -1)
            // {
            //     horizMove = vertMove = 315;
            //     horizMove = Mathf.Cos(vertMove / (180f / Mathf.PI));
            //     vertMove = Mathf.Sin(vertMove / (180f / Mathf.PI));
            //     Debug.Log("4th Quad");

            // }

        }

        return rightStickInput;
    }


    public float GetAimAngle()
    {
        aimDir = new Vector2(rightStickInput.x, rightStickInput.y);
        float currAngle = Mathf.Atan((aimDir.y / aimDir.x));

       

        float currAngleDegrees = currAngle * 57.2958f;

        if (aimDir.x < 0)
        {
            currAngleDegrees += 180;
        }
        //Debug.Log("aimDir = " + aimDir.x + "," + aimDir.y);
        //Debug.Log("AimAngle = " + currAngle);
        //Debug.Log("AimAngleDegrees " + currAngleDegrees);

        if (isFacingRight) {    
            if (currAngleDegrees > 80 && currAngleDegrees <= 90)
            {
                Debug.Log("currAngle is more than 80");
                currAngleDegrees = 80;
                currAngle = currAngleDegrees / 57.2958f;
                aimDir = new Vector2(Mathf.Cos(currAngle), Mathf.Sin(currAngle));
            }

            else if (currAngleDegrees < -80 && currAngleDegrees >= -90)
            {
                Debug.Log("currAngle is less than 80");
                currAngleDegrees = -80;
                currAngle = currAngleDegrees / 57.2958f;
                aimDir = new Vector2(Mathf.Cos(currAngle), Mathf.Sin(currAngle));
            }
        }
        else
        {
            if (currAngleDegrees < 100 && currAngleDegrees > 90)
            {
                Debug.Log("currAngle is less than 100");
                currAngleDegrees = 100;
                currAngle = currAngleDegrees / 57.2958f;
                aimDir = new Vector2(Mathf.Cos(currAngle), Mathf.Sin(currAngle));
            }

            else if (currAngleDegrees > 260 && currAngleDegrees < 270)
            {
                Debug.Log("currAngle is more than 260");
                currAngleDegrees = 260;
                currAngle = currAngleDegrees / 57.2958f;
                aimDir = new Vector2(Mathf.Cos(currAngle), Mathf.Sin(currAngle));
            }
        }
     //   Debug.Log("AimDir = " + aimDir.x + "," + aimDir.y);
        return currAngleDegrees;
    }

    public void SetGravity(bool gravityOn)
    {
        this.gravityOn = gravityOn;
    } 

    public Vector2 GetAimDir()
    {
        return aimDir;
    } 
    public void SetAimDir(Vector2 aimDir)
    {
        this.aimDir = aimDir;
    } 

    public bool GetL2Down()
    {
        if (prevTriggVal.Count == 2)
        {
            if (triggInput.x > prevTriggVal[0].x && prevTriggVal[0].x < 0.1f
                && triggInput.x > 0.1f)
            {
                // If pressing down for first time send true
                return true;
            }
            else
            {
                return false;
            }
        } 
        else
        {
            if (triggInput.x >= 0.1f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool GetL2Up()
    {
        if (prevTriggVal.Count == 2)
        {
            if (triggInput.x < prevTriggVal[0].x && prevTriggVal[0].x > 0.1f)
            {
                // If pressing up for first time send true
                return true;
            }
        }
        return false;       
 
    } 


    public bool GetR2Down()
    {
        if (prevTriggVal.Count == 2)
        {
          //  Debug.Log("Current R2 Input = " + triggInput.y);
          //  Debug.Log("Last R2 Input = " + triggInput.y);
            if (triggInput.y > prevTriggVal[0].y && triggInput.y > 0.1f 
                && prevTriggVal[0].y < 0.1f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (triggInput.y >= 0.1f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }

    public bool GetR2Up()
    {
        if (prevTriggVal.Count == 2)
        {
            if (triggInput.y < prevTriggVal[0].y && prevTriggVal[0].y > 0.1f)
            {
                // If pressing up for first time send true
                return true;
            }
        }
        return false;

    }

    public void toggleDir()
    {
        isFacingRight = !isFacingRight;
    }
    public int IsTravelingVert()

    {

     //   Debug.Log("Current pos = " + transform.position.y);
       
    //    Debug.Log("Last Pos = " + lastPos.y);

        if (prevPos.Count >= 2)
        {
       //     Debug.Log("prevPos[1] = " + prevPos[1].y);
       //     Debug.Log("prevPos[0] = " + prevPos[0].y);

            if (Mathf.Approximately(transform.position.y, prevPos[0].y))
            {
                return 0;
            }

            // Return 1 if traveling right
            if (transform.position.y > prevPos[0].y)
            {
                return 1;
            }
            //Return -1 if traveling left
            else if (transform.position.y < prevPos[0].y)
            {
                return -1;
            }
        }
        return 0;


        //   Debug.Log("Current pos = " + transform.position.y);
        //   Debug.Log("Last Pos = " + lastPos.y); 

        //float diff = transform.position.y - lastPos.y;

        //if (Mathf.Approximately(transform.position.y,lastPos.y))
        //{
        //    return 0;
        //}

        //// Return 1 if traveling up
        //if (transform.position.y > lastPos.y)
        //{
        //    return 1;
        //}
        ////Return -1 if traveling down
        //else if (transform.position.y < lastPos.y)
        //{
        //    return -1;
        //}
        //// Return 0 if not moving at all vertically
        //else
        //{
        //    return 0;
        //}


    }
    public int IsTravelingHoriz()
    {
     //   Debug.Log("Current pos = " + transform.position.x);
        
     //   Debug.Log("Last Pos = " + lastPos.x); 

        float diff = transform.position.x - lastPos.x;

        if (prevPos.Count >= 2)
        {
       //     Debug.Log("prevPos[1] = " + prevPos[1].x);
       //     Debug.Log("prevPos[0] = " + prevPos[0].x);
            if (Mathf.Approximately(transform.position.x, prevPos[0].x))
            {
                return 0;
            }

            // Return 1 if traveling right
            if (transform.position.x > prevPos[0].x)
            {
                return 1;
            }
            //Return -1 if traveling left
            else if (transform.position.x < prevPos[0].x)
            {
                return -1;
            }
        }
        
        // Return 0 if not moving at all vertically
            return 0;
    }

    public void RgdBdy(Vector2 newPos)
    {
        
    }

    public void ClimbLedge(GameObject topLedge)
    {
     
        
        
        
        Vector2 top;
        if (isFacingRight) 
        {
            Bounds ledgeBnds = topLedge.GetComponents<Collider2D>()[0].bounds;
            //  top = new Vector2(ledgeBnds.min.x + 0.69f, ledgeBnds.max.y + 1.25926275f);
            top = new Vector2(ledgeBnds.center.x , ledgeBnds.max.y + 0.9f);

        }
        else
        {
            Bounds ledgeBnds = topLedge.GetComponents<Collider2D>()[1].bounds;

            top = new Vector2(ledgeBnds.center.x, ledgeBnds.max.y + 0.9f);

        }
        transform.parent = null;
        Rigidbody2D rgdBdy = GetComponent<Rigidbody2D>();
        //  rgdBdy.MovePosition(top);
        transform.position = top;
        anim.SetBool("isClimbing", false);
        SetGravity(true);
        anim.enabled = true;
        isClimbing = false;
        SetFallState(false);

    }

    public void AimReset()
    {
  //      Debug.Log("Resetting Aim");
        GetComponentInChildren<AimDotController>().StopSights();
        GetComponentInChildren<AimArmController>().resetAim();
        SetSubState("none");

          if (GetState().Equals(aiming))
        {
        //    Debug.Log("Setting state to standing");
            SetState(standing);
        }
        else if (GetState().Equals(climbingAiming))
        {
            SetState(climbing);
        }
        if (isFacingRight)
        {
            aimDir = new Vector2(1, 0);
            lastAngle = 0;
        }
        else
        {
            aimDir = new Vector2(-1, 0);
            lastAngle = 180;
        }
        GetComponentInChildren<AimArmController>().setLastAngle(lastAngle);
    }  

    public void SetRidingMotion(Vector2 speed, GameObject ridingPlatform)
    {
       // Debug.Log("Riding platform");
        isRidingPlatform = true;
        platformSpeed = speed;
        this.ridingPlatform = ridingPlatform;
        chain.GetComponent<ChainController>().SetPlatformSpeed(platformSpeed);
   //     Debug.Log("Platform Speed = " + platformSpeed); 
    } 

    public void SetClimbingCollider (BoxCollider2D climbCollide)
    {
        Debug.Log("Setting Climbing Collider");
        this.climbCollide = climbCollide;
    }

    public Vector2 GetInput()
    {
        return input;
    }

    public bool GetIsLeaning()
    {
        return isLeaning;
    }
    public void SetTopLedge(GameObject top)
    {
        this.top = top;
    } 

    public GameObject GetTopLedge()
    {
        return top;
    } 
    
    public void KnockBack()
    {
        Debug.Log("Player getting Knocked Back");

        float currTime = Time.realtimeSinceStartup;
        float knockAngle;
        float knockVelocity;



        switch (state)
        {
    

            case climbing: 

                if (isFacingRight)
                {
                    knockAngle = 170.0f;
                }
                else
                {
                    knockAngle = 10.0f;
                }
                knockVelocity = 2.0f;
                SetState(jumping);
                break;

           
            default:

                if (isFacingRight)
                {
                    knockAngle = 165.0f;
                }
                else
                {
                    knockAngle = 15.0f;
                }
                knockVelocity = 15.0f;
                SetState(jumping);
                break;
        }

        playerPhysics.StartPhysics(currTime, transform.position, knockVelocity, knockAngle);
        
      //  SetFallState(true);
    }

    public Vector2 Gravity()
    {
        float grav = 5.0f * -9.81f;
        float currTime = Time.realtimeSinceStartup - fallTime;
        float yDisp = 0;
        float xPos = 0;
        float maxVel = 0;
        bool reachedMaxVel = false;
        currTime = Time.realtimeSinceStartup - fallTime; 
        if (currTime > 0.7f && !reachedMaxVel)
        {
       //     Debug.Log("Approximately 0.7f");
            currTime = Time.realtimeSinceStartup - fallTime;
            maxVel = (0.5f * grav * Mathf.Pow(currTime, 2.0f) + fallPosition)
                - transform.position.y;
            reachedMaxVel = true;

        }
     //   Debug.Log("Current Time = " + currTime);
     //   Debug.Log("MaxVel = " + maxVel);

        yDisp = 0.5f * grav * Mathf.Pow(currTime, 2.0f);
        xPos = transform.position.x;
     //   Debug.Log("Delta Y = " + (transform.position.y - (yDisp + fallPosition)));

        if (currTime < 0.7f)
        {
            return new Vector2(xPos, yDisp + fallPosition);
        }
        else
        {
            return new Vector2(xPos, transform.position.y + maxVel);
        }
    }
    public float[] GetFallInfo()
    {
        float[] fallInfo = new float[2] { fallTime, fallPosition };

        return fallInfo;
    }

    public void TurnAround(int colliderChoice)
    {
        Debug.Log("Turning around");
        StartCoroutine("JustTurned");
    //    aimDir = Vector2.Scale(aimDir, new Vector2(-1, 1));
        //  Debug.Log("aimDir = " + aimDir);
        isFacingRight = !isFacingRight;
        if (!GetComponents<BoxCollider2D>()[0].enabled)
        {
            GetComponents<BoxCollider2D>()[0].enabled = true;
        }
        float xMin = GetComponents<BoxCollider2D>()[0].bounds.min.x;
        Debug.Log("xMin = " + xMin);
        float xMax = GetComponents<BoxCollider2D>()[2].bounds.max.x;
        Debug.Log("xMax = " + xMax);
        float xAverage = (xMin + xMax) / 2;
        float yCenter = GetComponents<BoxCollider2D>()[3].bounds.center.y;
        Debug.Log("Turn pivot = " + xAverage);
        if (colliderChoice == 3)
        {
            transform.RotateAround(new Vector3(xAverage, yCenter, 0), Vector3.up, 180.0f);
        }
        else
        {
            transform.RotateAround(GetComponents<BoxCollider2D>()
                [colliderChoice].bounds.center, Vector3.up, 180.0f);
        }
        // transform.RotateAround(transform.position, Vector3.up, 180.0f);
        //     GetComponent<BoxCollider2D>().bounds
        //   transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        //      transform.RotateAround(GetComponents<BoxCollider2D>()
        //      [colliderChoice].bounds.center, Vector3.up, 180.0f);

    } 

    public void FallOffSwing()
    {
        chain.GetComponent<ChainController>().ChangeSwingAngle();
       
        if (state.Equals(swinging))
        {
            chain.GetComponent<ChainController>().SwingOff();
            
        }
        chain.GetComponent<ChainController>().ChainReturn();
        float speed = chain.GetComponent<ChainController>().GetSpeed() * 4;
        chain.GetComponent<ChainController>().SetSpeed(speed);
        transform.GetComponentInChildren<ChainDestroyer>().SetSpeed(speed * 1000000f);
        StartCoroutine("RotateAfterRetraction");

        //         transform.rotation = Quaternion.identity;
        SetFallState(true);
    }

    IEnumerator ShootPose()
    {

        anim.SetBool("Chain", true);
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("Chain", false);

    }
    IEnumerator StartFall()
    {
        yield return new WaitForSeconds(0.01f);
        startFall = false;
    } 

    IEnumerator RotateAfterRetraction()
    {
        yield return new WaitUntil(() => chain.GetComponent<ChainController>().GetState()
        .Equals("idle"));

        Debug.Log("Rotating back");
        if (isFacingRight)
        {
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.rotation = turnAngle;
        }
    }
   
    IEnumerator JustTurned()
    {
        justTurnedAround = true;
        yield return new WaitForSeconds(0.01f);
        justTurnedAround = false;
    }

    IEnumerator CrouchToggle()
    {
        Vector3 currPos = GetComponent<Transform>().position;

        if (isCrouching)
        {
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("ProneWalk") == true);
            Debug.Log("Idle Run Animation is " + anim.GetCurrentAnimatorStateInfo(0).IsName("Idle_Run_Anim"));
            GetComponent<Transform>().position = new Vector3(currPos.x, currPos.y - 0.31f, currPos.z);

            Debug.Log("ProneWalk Animation is " + anim.GetCurrentAnimatorStateInfo(0).IsName("ProneWalk"));
        }
        else
        {
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Idle_Run_Anim") == true);
            Debug.Log("Idle Run Animation is " + anim.GetCurrentAnimatorStateInfo(0).IsName("Idle_Run_Anim"));
            GetComponent<Transform>().position = new Vector3(currPos.x, currPos.y + 0.31f, currPos.z);
            Debug.Log("ProneWalk Animation is " + anim.GetCurrentAnimatorStateInfo(0).IsName("ProneWalk"));

        }
        GetComponents<BoxCollider2D>()[0].enabled = !GetComponents<BoxCollider2D>()[0].enabled;
        GetComponents<BoxCollider2D>()[1].enabled = !GetComponents<BoxCollider2D>()[1].enabled;
        GetComponents<CircleCollider2D>()[0].enabled = !GetComponents<CircleCollider2D>()[0].enabled;
        GetComponents<BoxCollider2D>()[2].enabled = !GetComponents<BoxCollider2D>()[2].enabled;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        // Makes Bouncless collisions possible!! 

        Debug.Log("Collision with  " + collision.gameObject.name);

        string layer = collision.gameObject.layer.ToString();
        string collisionName = collision.gameObject.name;
        Debug.Log("Adding Layer " + layer + " to CollideMap");

        if (CollideMap.ContainsKey(collisionName))
        {
            Debug.Log("Key " + collisionName + " is already in CollideMap");
        }
        else
        {
            CollideMap.Add(collisionName, 1);
        }
        
        if (CollideMap.ContainsKey(layer))
        {
            int layerCount;
            CollideMap.TryGetValue(layer, out layerCount);
            
            layerCount++;
            CollideMap.Remove(layer);
            CollideMap.Add(layer, layerCount);
        } 

        else
        {
            CollideMap.Add(layer, 1);
        }


        BoxCollider2D feetCollider = GetComponents<BoxCollider2D>()[0];

        Debug.Log("Max Player Collider 0 = " + GetComponents<BoxCollider2D>()[0].bounds.max.x);
        Debug.Log("Max Player Collider 1 = " + GetComponents<BoxCollider2D>()[1].bounds.max.x);
        Debug.Log("Max Player Collider 2 = " + GetComponents<BoxCollider2D>()[2].bounds.max.x);
        Debug.Log("Max Player Collider 3 = " + GetComponents<BoxCollider2D>()[3].bounds.max.x);

        Debug.Log("Min Surface Collider = " + collision.collider.bounds.min.x);

        Collider2D collider = collision.collider;
        if (collision.gameObject.layer == 12  || collision.gameObject.layer == 11)
        {
            
            if (state.Equals(swinging))
            {
                FallOffSwing();
          //      SetState(standing);
            }

            BoxCollider2D surfaceCollider = (BoxCollider2D) collision.collider;
            if (feetCollider.bounds.min.y >= surfaceCollider.bounds.max.y && GetState().Equals(falling))
            {
               
                SetState(standing);
                anim.SetBool("IsFalling", false);
                Debug.Log("Collision with ground, setting state to Standing");
                Debug.Log("Collision happened at Y = " + gameObject.transform.position.y);
            }
            else
            {
                if (!(substate.Equals(chaining) || state.Equals(climbing)) &&
                    collision.gameObject.layer == 12)
                {

                    if ((IsTravelingHoriz() == 1 && feetCollider.bounds.max.x <= collider.bounds.min.x) ||
                        (IsTravelingHoriz() == -1 && feetCollider.bounds.min.x >= collider.bounds.max.x))
                    {
                        Debug.Log("Setting state to fall");
                        int layerCount;
                        CollideMap.TryGetValue(layer, out layerCount);
                        layerCount--;
                        CollideMap.Remove(collisionName);
                        CollideMap.Remove(layer);
                        CollideMap.Add(layer, layerCount);
                   //     SetFallState(false);
                    }
                }
            }
        }
        //Debug.Log("Arthur is touching something");
        //    Debug.Log(collision.gameObject.tag);
        //if (collision.gameObject.CompareTag("Chainable"))
        //{
        //    isRidingPlatform = true;
        //}
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            Debug.Log("Taking Damage!!");
            Destroy(collision.gameObject);
            GetComponent<PlayerHealthScript>().DamageOrHealth(-1);
            GetComponent<WowController>().Impact();
        }
        else if (collision.gameObject.CompareTag("CircleEnemy"))
        {
            Debug.Log("Taking Damage!!");
            GetComponent<PlayerHealthScript>().DamageOrHealth(-1);
            GetComponent<WowController>().Impact();
        }
        else if (collision.gameObject.CompareTag("Chain"))
        {
        //    Destroy(collision.gameObject);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        //Debug.Log("Arthur is touching something");
     //   Debug.Log(collision.gameObject.tag);
     //   Debug.Log("Layer = " + collision.gameObject.layer);

        string tag = collision.gameObject.tag;
        BoxCollider2D feetCollider = GetComponents<BoxCollider2D>()[0];
        BoxCollider2D surfaceCollider = (BoxCollider2D)collision.collider;

        if (collision.gameObject.layer == 12 && feetCollider.bounds.min.y >= surfaceCollider.bounds.max.y 
            && GetState().Equals(falling))
        {

            SetState(standing);
            anim.SetBool("IsFalling", false);
            Debug.Log("Collision with ground, setting state to Standing");
            Debug.Log("Collision happened at Y = " + gameObject.transform.position.y);
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //    Debug.Log("Left Collision");
        Debug.Log("Left Collision with " + collision.gameObject.name);
        //    Debug.Log("Layer = " + collision.gameObject.layer);


        string layer = collision.gameObject.layer.ToString();
        string collisionName = collision.gameObject.name;
        if (CollideMap.ContainsKey(collisionName))
        {
            CollideMap.Remove(collisionName);

            if (CollideMap.ContainsKey(layer))
            {
                Debug.Log("Removing layer " + layer + " from CollideMap");
                int layerCount;
                CollideMap.TryGetValue(layer, out layerCount);
                if (layerCount > 0)
                {

                    layerCount--;
                    CollideMap.Remove(layer);
                    CollideMap.Add(layer, layerCount);
                }
            }
            else
            {
                CollideMap.Add(layer, 0);
            }
        }
        


        if (collision.gameObject.CompareTag("TopOfLedge"))
        {
            if (GetState().Equals(standing))
            {
                transform.parent = null;
            }
        }
        if (collision.gameObject.CompareTag("Chainable"))
        {
            Debug.Log("Leaving Collision with Chainable");
            GameObject obj = collision.gameObject;
          //  obj.GetComponent<ChainableController>().MovePlayer(false);
          //  isRidingPlatform = false;
            //SetState(standing);
        }
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 11 && !justTurnedAround)
        {
            BoxCollider2D feetCollider = GetComponents<BoxCollider2D>()[0];
            Debug.Log("Leaving Ground");
            if (state.Equals(standing) && !substate.Equals(chaining) && collision.gameObject.layer == 12)
            {

                if (feetCollider.bounds.min.y >= collision.collider.bounds.max.y)
                {
                    Debug.Log("Setting state to fall");
                    int layerCount;
                    CollideMap.TryGetValue(layer, out layerCount);
                    layerCount--;
                    CollideMap.Remove(collisionName);
                    CollideMap.Remove(layer);
                    CollideMap.Add(layer, layerCount);
                    SetFallState(false);
                }
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("TopOfLedge"))
        {

                    Debug.Log("Pull Up Zone detected");
            //     if (GetState().Equals(climbing) || GetState().Equals(chaining))
            if (GetState().Equals(climbing) || GetSubState().Equals(chaining))
            {
                pullUpZone = true;
                SetTopLedge(collision.gameObject);
                if (buttonPrompt == null)
                {

                    buttonPrompt = new GameObject();
                    buttonPrompt.name = "XButton";
                    var rend = buttonPrompt.AddComponent<SpriteRenderer>();
                    rend.sprite = Resources.Load<Sprite>("Sprites/XButton");
                    rend.sortingLayerName = "Sprites";
                    rend.sortingOrder = 5;
                    buttonPrompt.GetComponent<Transform>().localScale = new Vector3(0.25f, 0.25f);
                    buttonPrompt.GetComponent<Transform>().position = transform.position + new Vector3(0, 2.0f, 0.0f);
                    buttonPrompt.transform.parent = gameObject.transform;
                }
                //    ClimbLedge(collision.gameObject);
            }
            if (GetState().Equals(standing) || GetState().Equals(falling))
            {
        //        GetState().Equals(standing);
                //      Debug.Log("Standing on top of surface");
                if (collision.gameObject.transform.parent.transform.parent != null)
                {
                    //    transform.parent = collision.gameObject.transform.parent.transform;
                }
            }
        }
        else if (collision.gameObject.layer == 13)
        {
            Debug.Log("Hit by enemy");

            if (!GetComponent<PlayerHealthScript>().GetIsHitStunned())
            {
                string chainState = chain.GetComponent<ChainController>().GetState();
                if (state.Equals(swinging) || state.Equals(chaining))
                {
                    Debug.Log("Starting fall off swing");
                    FallOffSwing();
             
             

                }
                else
                {
                    KnockBack();
                }
                if (collision.CompareTag("Laser") || collision.CompareTag("CircleEnemy"))
                {
                    GetComponent<PlayerHealthScript>().DamageOrHealth(-1);
                    GetComponent<WowController>().Impact();
                }
            }
        }
        //}
        GameObject collider = collision.gameObject; 

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TopOfLedge"))
        {
            //if (GetState().Equals(standing) || GetState().Equals(falling))
            //{
            //    SetState(standing);
                isRidingPlatform = true;
            //    platformSpeed = collision.gameObject.transform.parent
            //        .GetComponent<ChainableController>().GetSpeed();
            //}

        }

        if (collision.gameObject.layer == 12)
        {
        //    SetState(standing);
         //   isColliding = true;
            //     GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        //    Debug.Log("Collision with ground, setting state to Standing");
        }


    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("TopOfLedge"))
        {
            Debug.Log("Left Pull Up Zone");
            pullUpZone = false;
            Destroy(buttonPrompt);
         //   isRidingPlatform = false;

        }

        if (collision.gameObject.layer == 12)
        {
            Debug.Log("Leaving collision with ground");
        //    isColliding = false;
         //   GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

       //     Debug.Log("Left collision with ground");
        }



        GameObject collider = collision.gameObject;

        if (collider.CompareTag("Chainable"))
        {
        //    Debug.Log("Player is touching chainable object");
            transform.parent = null;
          //  SetClimbing(true, gameObject);
          //  AimReset();
        }
    }
}
