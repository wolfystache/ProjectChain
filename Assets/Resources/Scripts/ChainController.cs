using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainController : Character {

    public GameObject nextLink;
    public GameObject ChainHead;
    public Transform ChainHeadSpawn;
    public Transform ClimbingAimSpawn;
    public GameObject ChainDestroyer;

    float speed;
    bool isReturning;
    private Animator anim;
    private Transform linkSpawn;
    Transform spawn;

    private Stack chainList;
    private bool isStuck;
    private bool isChaining;
    private Vector2 aimDir;
    private bool wasClimbing;
    private const string shooting = "shooting", retracting = "retracting",
        pulling = "pulling", swinging = "swinging";
    private Vector2 LastPos;
    private Vector2 PlatformSpeed;
    private int FixedCount = 0;
    private Vector2 LastPlayerPos = Vector2.zero;
    private Vector2 headPos = Vector2.zero;
    private List<float> lastPosList;
    private float startSwingTime;
    private float currSwingTime;
    private float theta;
    private float currTime;
    private float length;
    private float lastAngle;
    private float maxAngle;
    private float xInput;
    private AudioSource audio;
    private Vector2 playerOffset = Vector2.zero;

    private Physics playerPhysics; 

    private bool isPullingClimbable = false;

    public static ChainController chain;

    private static int linkCount;

    void Start () {
        chain = this;
        isReturning = false;
        SetState(idle);
        chainList = new Stack();
        speed = 0f;
        isStuck = false;
        isChaining = false;
        wasClimbing = false;
        LastPos = new Vector2();
        PlatformSpeed = new Vector2();
        rgdBdy = GetComponent<Rigidbody2D>();
        lastPosList = new List<float>();
        audio = GetComponent<AudioSource>();

    }

    private void Update ()
    {
        if (Input.GetButtonDown("Chain"))
        {
            if (GetState().Equals(swinging))
            {
                
            }
        }
    }

    public override void FixedUpdate()
    {
   //     rgdBdy.MovePosition(playerOffset);
        if (linkCount == 13)
        {
            Debug.Log("Chain Return");
            ChainReturn();

        }
        //    Debug.Log("Chain FixedCount = " + FixedCount);
        FixedCount++;

        Debug.Log("Chain State = " + state);
      //  Debug.Log("Speed = " + speed);

        Vector2 offset = new Vector2();
        
     
        //         Debug.Log("AimDir = " + aimDir);
        Vector2 pos = transform.position;
        Vector2 playerPos = ArtrobotController.player.transform.position;

        bool isFacingRight = ArtrobotController.player.GetComponent<ArtrobotController>().IsFacingRight();

       

        switch (state)
        {
            case idle:
                break;
            case shooting:
                offset += (speed * aimDir);
                break;
            case retracting:
                offset = offset - (speed * aimDir);
            //    Debug.Log()
             //   offset += player.GetComponent<Rigidbody2D>().position;

                break;
            case pulling:
                Vector2 playerOffset = speed * aimDir;
                ArtrobotController.player.AddOffset(playerOffset);
                if (!isPullingClimbable)
                {
                    float chainPlayerDistance = (transform.position - ArtrobotController.player.transform.position)
                        .magnitude;

                    if (chainPlayerDistance < 2.5f)
                    {
                        Swing();
                    }

                }

                break;
            case swinging:
                currSwingTime = Time.realtimeSinceStartup - startSwingTime;
                //           Debug.Log("HeadPos = " + headPos);
                Vector2 input = ArtrobotController.player.GetInput();

                int facingRightMult; 

                if (isFacingRight)
                {
                    facingRightMult = 1;
                }
                else
                {
                    facingRightMult = -1;
                }

                int HorizDirection = ArtrobotController.player.IsTravelingHoriz();
         //       Debug.Log("HorizDirection = " + HorizDirection);
             //   Debug.Log("Input = " + input.x);
                if ((HorizDirection == 1 && input.x > 0) || 
                    (HorizDirection == -1 && input.x < 0) && Mathf.Abs(maxAngle) < 70)
                {
                    maxAngle -= 0.1f * Mathf.Abs(input.x) * facingRightMult;
                } 

                else if (maxAngle < 0 )
                {
                    if (input.y < 0)
                    {
                        maxAngle -= input.y * 0.2f * facingRightMult;
                    }
                    else
                    {
                        maxAngle += 0.03f * facingRightMult;
                    }
                }
                //     
                theta =  Physics.PendulumDisplacement(maxAngle, 
                    length, currSwingTime, 3*9.81f);
         //       Debug.Log("Theta = " + theta);
         //       Debug.Log("MaxAngle = " + maxAngle);
                float round = (float)((int)(theta * 100)) / 100;
               
            //    Debug.Log("Round = " + round); 
                
                if ((theta < -12 && theta > -17 && HorizDirection == 1 && isFacingRight) 
                    || (theta > 12 && theta < 17 && HorizDirection == -1 && !isFacingRight)) {
                    Debug.Log("Playing woosh");
                    Debug.Log("Theta = " + theta);
                    if (!audio.isPlaying)
                    {
                        audio.volume = -(maxAngle * facingRightMult) / 100;
                        Debug.Log("Volume = " + audio.volume);
                        audio.Play();
                    }
                }

                //       Debug.Log("HeadPos = " + headPos);
                //       Debug.Log("Theta = " + theta);
                // transform.RotateAround(transform.position, Vector3.forward, theta - lastAngle);
                ArtrobotController.player.transform.RotateAround
                    (transform.position, Vector3.forward, theta - lastAngle);
                transform.RotateAround
                    (transform.position, Vector3.forward, theta - lastAngle);
                //      Debug.Log("lastAngle = " + lastAngle);
                //       Debug.Log("Rotating by " + (theta - lastAngle));
                lastAngle = theta;
                
                break;
            default:
                break;
        }
      

        offset += PlatformSpeed;

        if (transform.parent == null && !GetState().Equals(idle))
        {
            //         Debug.Log("playerOffset = " + playerOffset.x + "," + playerOffset.y);
          //  transform.position += new Vector3(offset.x, offset.y);
            rgdBdy.MovePosition(rgdBdy.position + offset);
        }
        else
        {
            
            transform.position += new Vector3(offset.x, offset.y);
        }
        playerOffset = Vector2.zero;

        if (isChaining)
        {
            //   
            
            
            //      Debug.Log("Speed = "  + aimDir.magnitude);
            if (!isReturning && !isStuck)
            {
            //    GetComponent<Transform>().position = pos + (speed * aimDir);
                   //     offset +=  (speed * aimDir );
            }
            else if (isReturning)
            {
             //     GetComponent<Transform>().position = pos - (speed * aimDir);
             //   offset -= (speed * aimDir);
            }
         

       //     offset += PlatformSpeed;
            float diff = transform.position.y - LastPos.y;
 
     //       }
        }

        //   Debug.Log("Platform Speed = " + PlatformSpeed.y);
        //  player.transform.position = player.transform.position + new Vector3(PlatformSpeed.x, PlatformSpeed.y, 0);
       
        
        
        
    }
    public override void LateUpdate()
    {
        LastPos = transform.position;
    }
    public void ShootChain()
    {
        transform.parent = ArtrobotController.player.transform;
        Debug.Log("Shooting Chain");
     //   player.GetComponent<SpriteRenderer>().enabled = false;
        //    isChaining = true;
        SetState(shooting);
        AudioClip audClip = (AudioClip) Resources.Load("SoundFX/steel-sheet-bang");

        // audClip.LoadAudioData()
        if (audClip != null)
        {
            audio.volume = 1.0f;
            audio.clip = audClip;
            audio.Play();
            StartCoroutine("AudioSequence");
        }
        else
        {
            Debug.Log("Couldn't find audio file");
        }


         speed = 0.2f;
        //   player.GetComponent<ArtrobotController>().GetAimAngle();
          aimDir = ArtrobotController.player.GetComponent<ArtrobotController>().GetAimDir();
   //     aimDir = new Vector2(1, 0);
        Debug.Log("AimDir = " + aimDir.x + "," + aimDir.y); 
        // speed = 7.3f;

        //    Debug.Log(player.GetComponent<ArtrobotController>().GetClimbing());
        if (ArtrobotController.player.GetComponent<ArtrobotController>().GetClimbing())
        {
            Debug.Log("Rotating Chain Head");
            spawn = ClimbingAimSpawn;
        }
        else
        {
            spawn = ChainHeadSpawn;
        }
        GameObject link = (GameObject)Instantiate(ChainHead, spawn.position, spawn.rotation);
        link.transform.parent = gameObject.transform;

        Vector3 head = GetComponentsInChildren<Transform>()[2].position;
        Vector3 offset = head - transform.position;
        transform.position = head;
        link.transform.position -= offset;
        LastPlayerPos = ArtrobotController.player.GetComponent<Rigidbody2D>().position;

        chainList.Push(link);
        anim = link.GetComponent<Animator>();
        linkSpawn = link.transform.GetChild(0).transform;
        float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
         StartCoroutine("SpawnNextLink", animLength);
        //SpawnNextLink(animLength);


    }
    public void StruckChainable(GameObject ledge, bool isClimable)
    {
        isPullingClimbable = isClimable;
        Debug.Log("Struck Chainable");
        SetState(pulling);
        
        if (playerPhysics == null)
        {
            playerPhysics = ArtrobotController.player.characPhysics;

        }
        playerPhysics.StopPhysics();
        ArtrobotController.player.SetState("standing");
        StopCoroutine("AudioSequence");
        AudioClip ChainStrike = (AudioClip) Resources.Load("SoundFX/axe-impact-3");
        if (ChainStrike!= null)
        {
            
            audio.clip = ChainStrike;
            audio.volume = 0.5f;
            audio.loop = false;
            audio.Play();
            StartCoroutine("AudioSequence");
            
        }
        else
        {
            Debug.Log("Couldn't find Audio Clip");
        }
        transform.parent = null;
        ArtrobotController.player.transform.parent = null;
        //  player.transform.parent = ledge.transform;
        ArtrobotController.player.SetIsRiding(true);
        ledge.GetComponent<ChainableController>().MovePlayer(true);
        //   isStuck = true;
         
       // Swing();
       // SetState(swinging);
     
        if (ArtrobotController.player.GetClimbing())
        {

            AimController.player.localTurnAround();
            wasClimbing = true;
        }
        ArtrobotController.player.SetGravity(false);
        //   isReturning = true; 
        ChainDestroyer.GetComponent<Collider2D>().enabled = true;
        GameObject furthest = (GameObject) chainList.Pop();
        Debug.Log("Popping " + furthest.name);
        // ChainDestroyer.GetComponent<ChainDestroyer>().OutsideDestroy(furthest);
        Destroy(furthest);
        ChainDestroyer.GetComponent<ChainDestroyer>().totalChainList(chainList);
        ChainDestroyer.GetComponent<ChainDestroyer>().SetSpeed(speed * 12);
        linkCount = 0;
    } 

    public void Swing()
    {
        StopCoroutine("AudioSequence");
        audio.Stop();
        audio.loop = false;

        startSwingTime = Time.realtimeSinceStartup;
        SetState(swinging);

        AudioClip Switch = (AudioClip)Resources.Load("SoundFX/" +
            "breaker-1");
        if (Switch != null)
        {
            Debug.Log("Playing Breaker sound");
            audio.clip = Switch;
            audio.volume = 1;
            audio.Play();
            StartCoroutine("AudioSequence");
        }
        else
        {
            Debug.Log("Couldn't find Audio File");
        }

        
        ChainDestroyer.GetComponent<Collider2D>().enabled = true;
        ArtrobotController.player.SetState("swinging");
        //   Debug.Log("Chain Group position = " + transform.position);   

        Vector2 pos = transform.position;
        transform.parent = null;
   //     transform.parent = ArtrobotController.player.transform;
        transform.position = pos;
        Vector2 head = GetComponentsInChildren<Transform>()[1].position;
        //      Debug.Log("Head name = " + GetComponentsInChildren<BoxCollider2D>()[1].name);
        Transform furtLinkPos = transform.GetChild(transform.childCount - 1).transform;
        furtLinkPos.GetComponent<Animator>().speed = 0;
   //     furtLinkPos.GetComponent<Animator>().SetFloat("Speed", speed * 1000000f);

        Debug.Log("Name = " + furtLinkPos.name);
        //   furtLinkPos.GetComponent<Animator>().SetFloat("Speed", 0);

        
        length = ((new Vector3(head.x, head.y)) - furtLinkPos.position).magnitude;
        
        lastAngle = -((Mathf.Atan(aimDir.x / aimDir.y) * 57.2958f));
        if (!ArtrobotController.player.IsFacingRight())
        {
            lastAngle *= 1;
        }
        Debug.Log("Starting Angle = " + lastAngle);
        maxAngle = lastAngle;

    }
    public void ChainReturn()
    {
        Debug.Log("Furthest chain: " + transform.localPosition.x);
        GameObject furtLinkPos = transform.GetChild(transform.childCount - 1).gameObject;
        Debug.Log("furthLinkPos = " + furtLinkPos.name);
        if (GetState().Equals(swinging))
        {
            float animSpeed = (int)(speed * 10.0f);
            animSpeed *= 2;
            //   Debug.Log("Chain Stack = " + )

            //   furtLinkPos.GetComponent<Animator>().SetFloat("Speed", animSpeed);
            furtLinkPos.GetComponent<Animator>().speed = 1;
            furtLinkPos.GetComponent<Animator>().SetFloat("Speed", speed * 1000000f);
            ChainDestroyer.GetComponent<ChainDestroyer>().StopAllCoroutines();
            Debug.Log("Destroying " + furtLinkPos.name);
            if (!chainList.Peek().Equals(furtLinkPos))
            {
                Destroy(furtLinkPos);
            }
            //  chainList.Pop();
            //      Debug.Log("Reanimating Chain " + furtLinkPos.name);
            Debug.Log("Speed = " + furtLinkPos.GetComponent<Animator>().speed);
        }
        else
        {
            if (chainList.Peek().Equals(furtLinkPos))
            {
                chainList.Pop();
            }
            ChainDestroyer.GetComponent<ChainDestroyer>().StopAllCoroutines();

            Destroy(furtLinkPos); 
            

        }
        SetState(retracting);
        ChainDestroyer.GetComponent<Collider2D>().enabled = true;
        ChainDestroyer.GetComponent<ChainDestroyer>().SetSpeed(speed * 10);
        ChainDestroyer.GetComponent<ChainDestroyer>().totalChainList(chainList);
     //   transform.parent = null;
      //  isReturning = true;
        Debug.Log("Chain Return");
        linkCount = 0;
       


    }
    public Stack getChainList()
    {
        return chainList;
    }

    public void setChainList(Stack chainList)
    {
        this.chainList = chainList;
    }

    public bool IsStruck()
    {
        return isStuck;
    }

    public void SetPlatformSpeed(Vector2 PlatformSpeed)
    {
        this.PlatformSpeed = PlatformSpeed;
    }


    public void SetOffset(Vector2 playerOffset)
    {
        this.playerOffset = playerOffset;
    }

    public void SwingOff()
    {
        Debug.Log("Length = " + length);
        transform.parent = ArtrobotController.player.transform;
        bool isFacingRight = ArtrobotController.player.IsFacingRight();
        int horizTravelDir = ArtrobotController.player.IsTravelingHoriz();
        float dismountAngle = 0;
        float thetaRad = theta / (180 / Mathf.PI);
        float maxAngleRad = maxAngle / (180 / Mathf.PI);
        float currTime = Time.realtimeSinceStartup;
        //    float dismountVel = Mathf.Abs(maxAngle * Mathf.Sqrt((-Physics.grav / length))
        //       * Mathf.Sin(Mathf.Sqrt(((-Physics.grav) / length) * currSwingTime))) * 0.2f;

        float dismountVel = Mathf.Sqrt(2 * -Physics.gravConstant * length *
            (Mathf.Cos(Mathf.Abs(thetaRad)) - Mathf.Cos(Mathf.Abs(maxAngleRad)))) * 1.0f;
        Debug.Log("Theta = " + Mathf.Abs(theta));
        Debug.Log("MaxAngle = " + (Mathf.Abs(maxAngle)));
        Debug.Log("Difference is " + (Mathf.Cos(Mathf.Abs(theta)) - Mathf.Cos(Mathf.Abs(maxAngle))));
        if (horizTravelDir == 1)
        {
            if (theta > 0)
            {
                dismountAngle = theta;
            }
            else 
            {
                dismountAngle = -theta + 270;
            }
        }
        else if (horizTravelDir == -1)
        {
            if (theta < 0)
            {
                dismountAngle = 90 + (90 +theta);
            }
            else
            {
                dismountAngle = 180 + theta;
            }
        }
   
  
            
        Debug.Log("DismountVel and Angle = " + dismountVel + " & "
            + dismountAngle);
        playerPhysics.StartPhysics(currTime, ArtrobotController.player.transform.position, dismountVel,
            dismountAngle, 1);
    }
    

    public void SetSpeed(float speed)
    {
        this.speed = speed; 
    } 
    public void ChangeSwingAngle()
    {
        Vector2 newAimDir = new Vector2(transform.position.x - ArtrobotController.player.transform.position.x,
            transform.position.y - ArtrobotController.player.transform.position.y);
        newAimDir /= (transform.position - ArtrobotController.player.transform.position).magnitude;
        aimDir = newAimDir;
    }

    public float GetSpeed()
    {
        return speed;
    }
    public void ResetChain()
    {
        // GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        speed = 0;
        if (!audio.clip.name.Equals("woosh"))
        {
            audio.Stop();
        }
        audio.loop = false;
        if (state.Equals(retracting))
        {
            //  player.GetComponent<ArtrobotController>().SetState("standing");
            ArtrobotController.player.AimReset();
        }
        if (wasClimbing)
        {
            wasClimbing = false;
       //     player.GetComponent<AimController>().childrenTurnAround();
        }
        transform.parent = null;
        SetState(idle);
        isStuck = false;
      //  GetComponent<Transform>().localPosition = new Vector2(0, 0f);
    //    Debug.Log(GetComponent<Transform>().position);
        isReturning = false;
        anim = ArtrobotController.player.GetComponent<Animator>();
        anim.SetBool("Chain", false);
        //  player.GetComponent<ArtrobotController>().SetChaining(false);
        ArtrobotController.player.GetComponentInChildren<AimArmController>().resetAim();
        ChainDestroyer.GetComponent<Collider2D>().enabled = false;
        isChaining = false;

    } 

    public void SetChainDestroyerSpeed (float speed)
    {

    }
    IEnumerator SpawnNextLink(float animLength)
    {
        // Debug.Log("Anim Time = " + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        while (GetState().Equals(shooting)) 
        {
            
           
            //   yield return new WaitForSeconds(animLength);
            float animProgress = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            //  Debug.Log("animName = " + anim.GetCurrentAnimatorStateInfo(0).tagHash);
       //     Debug.Log(isReturning);
            if (anim != null)
            {
                yield return new WaitForSeconds(0.074f);
                //yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            }
      //      Debug.Log("chain done");
            if (!GetState().Equals(shooting)) { break; }
            linkCount++;
            Vector3 playerDiff = ArtrobotController.player.GetComponent<Rigidbody2D>().position - LastPlayerPos;
  //          Debug.Log("LinkSpawnPosition = " + linkSpawn.position);
            GameObject link = (GameObject)Instantiate(nextLink, linkSpawn.position, linkSpawn.rotation);
            link.name = linkCount.ToString();
            chainList.Push(link);
            anim = link.GetComponent<Animator>();
            animLength = anim.GetCurrentAnimatorStateInfo(0).length;
            linkSpawn = link.transform.GetChild(0).transform;
            link.transform.parent = gameObject.transform;
            LastPlayerPos = ArtrobotController.player.GetComponent<Rigidbody2D>().position;
            
        }

    } 

    IEnumerator AudioSequence()
    {
        if (GetState().Equals(shooting)) {
            AudioClip ChainRelease = (AudioClip)Resources.Load("SoundFX/cartoon-arrow-08");
            if (ChainRelease != null)
            {
                yield return new WaitUntil(() => !audio.isPlaying);
                audio.clip = ChainRelease;
                audio.volume = 0.5f;
                audio.loop = true;
                audio.Play();

            }

            else
            {
                Debug.Log("Couldn't find Audio Clip");
            }
        }
        else if (GetState().Equals(pulling))
        {
            AudioClip ChainPull = (AudioClip)Resources.Load("SoundFX/pulling-chain");
            if (ChainPull != null)
            {
                yield return new WaitUntil(() => !audio.isPlaying);
                audio.clip = ChainPull;
                audio.volume = 0.1f;
                audio.loop = true;
                audio.Play();

            }
            else
            {
                Debug.Log("Couldn't find Audio Clip");
            }
        } 
        else if (GetState().Equals(swinging))
        {
            AudioClip Woosh = (AudioClip)Resources.Load("SoundFX/" +
                       "LongWoosh");
            Woosh.name = "woosh";
            if (Woosh != null)
            {
                yield return new WaitUntil(() => !audio.isPlaying);
                audio.volume = 0.1f;
                audio.clip = Woosh;
            }
            else
            {
                Debug.Log("Couldn't find Audio File");
            }
        }
    }


}
