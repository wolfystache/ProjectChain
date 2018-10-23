using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainDestroyer : MonoBehaviour {

    // Use this for initialization
    public GameObject player;
    public GameObject ChainGroup;


    private int chainSize;
    private Stack chainList;
    private GameObject mostRecentChain;
    private Animator anim;
    private float speed;

	void Start () {
        
    }
    void FixedUpdate()
    {
        chainList = ChainGroup.GetComponent<ChainController>().getChainList();

        if (ChainGroup.GetComponent<ChainController>().GetState().Equals("swinging"))
        {
            if (anim != null)
            {
                anim.speed = 0;
            }
        }
    }

    public void totalChainList(Stack chainList)
    {
        this.chainList = chainList;
        mostRecentChain = (GameObject)chainList.Peek();
        Debug.Log("Most Recent Chain tag is " + mostRecentChain.name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Chain Destroyer Collision is " + collision.name);
        Debug.Log("Speed = " + speed);

        GameObject gmeObj = collision.gameObject;
        anim = gmeObj.GetComponent<Animator>();



        // float speed = (int)(ChainGroup.GetComponent<ChainController>().GetSpeed() * 10.0f);
        // speed *= 5;
        //    Debug.Log("Speed = " + speed);
        //    Debug.Log("Raw Speed = " + ChainGroup.GetComponent<ChainController>().GetSpeed());



        //  if (isStuck)
        //  {
        //   Debug.Log("Name is " + collision.name);
        if (collision.CompareTag("Chain") || collision.CompareTag("ChainHead"))
        {

            anim.SetBool("End", true);
            // if ((chainList.Contains(gmeObj) && !(chainList.Contains(mostRecentChain) || gmeObj.Equals(mostRecentChain))) ||
            //     (gmeObj.Equals(mostRecentChain) && chainList.Contains(mostRecentChain)))
            if (chainList.Peek().Equals(gmeObj))
             {
                SeverLink(gmeObj);
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //     Debug.Log("Stay Chain Destroyer Collision is " + collision.name);
        GameObject gmeObj = collision.gameObject;
        anim = gmeObj.GetComponent<Animator>();



        // float speed = (int)(ChainGroup.GetComponent<ChainController>().GetSpeed() * 10.0f);
        // speed *= 5;
        //    Debug.Log("Speed = " + speed);
        //    Debug.Log("Raw Speed = " + ChainGroup.GetComponent<ChainController>().GetSpeed());




        //      Debug.Log("Stack = " + chainList.ToArray());

        //  if (isStuck)
        //  {
        //   Debug.Log("Name is " + collision.name);
        if (collision.CompareTag("Chain") || collision.CompareTag("ChainHead"))
        {

            anim.SetBool("End", true);
            // if ((chainList.Contains(gmeObj) && !(chainList.Contains(mostRecentChain) || gmeObj.Equals(mostRecentChain))) ||
            //     (gmeObj.Equals(mostRecentChain) && chainList.Contains(mostRecentChain)))
            if (chainList.ToArray().Length != 0)
            {
                if (chainList.Peek().Equals(gmeObj))
                {
                    SeverLink(gmeObj);
                }
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    //  } 

    public void SetSpeed (float speed)
    {
        this.speed = speed;
    }

    public void SeverLink(GameObject gmeObj)
    {
        Debug.Log("Destroying gameobject " + gmeObj.name);
        StartCoroutine(Destroy(gmeObj, anim));
        float length;

        bool isStuck = ChainGroup.GetComponent<ChainController>().IsStruck();
        length = anim.GetCurrentAnimatorStateInfo(0).length * (1 / speed);


        if (gmeObj.CompareTag("ChainHead") && isStuck)
        {
            length *= 4.0f;
            //      anim.SetFloat("Speed", speed);
            //        Destroy(gmeObj, length);
        }
        anim.SetFloat("Speed", speed);

        //else if (collision.gameObject.Equals(mostRecentChain))
        //{
        //    anim.SetFloat("Speed", speed);

        //    //     Debug.Log("Destroyed first member");

        //}
        //else
        //{
        //    anim.SetFloat("Speed", speed);
        //}
        Debug.Log("Curr Anim Speed = " + anim.speed);
        Debug.Log("Curr Anim Param Speed = " + anim.GetFloat("Speed"));

        Debug.Log("Anim Speed = " + ChainGroup.transform.
            GetChild(ChainGroup.transform.childCount - 1).
            GetComponent<Animator>().speed);
        Debug.Log("Norm Time = " + ChainGroup.transform.
            GetChild(ChainGroup.transform.childCount - 1).
            GetComponent<Animator>().speed);
        if (chainList.ToArray().Length != 0)
        {
            //    GameObject popped = (GameObject)chainList.Peek();

            //    Debug.Log("Gameobject " + popped.name + " is being popped");
            GameObject top = (GameObject)chainList.Pop();
            Debug.Log("ChainList size = " + chainList.ToArray().Length);
            Debug.Log("Popped chain = " + top.name);

            //    Destroy(popped);
        }
        if (chainList.ToArray().Length == 0)
        {
            Debug.Log("Chain List is empty!");
            StartCoroutine((ResetChain(length, gmeObj)));
        }
    }

    IEnumerator Destroy (GameObject gmeObject, Animator anim0)
    {
        Animator linkAnim = anim0;
             yield return new WaitUntil(() =>
             (gmeObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ChainLinkReverse")));
          yield return new WaitUntil(() =>
          gmeObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
     //   yield return new WaitForSeconds(0.3f);
           Debug.Log("Normalized Time for " + gmeObject.name + 
               " = " + gmeObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
        Destroy(gmeObject);
    }
    IEnumerator ResetChain(float animTime, GameObject ChainHead)
    {
        string chainState = ChainGroup.GetComponent<ChainController>().GetState();
        //  bool isClimbing = transform.parent.GetComponent<ArtrobotController>().GetClimbing();
        if (chainState.Equals("pulling"))
        {
            Debug.Log("Waiting for climb to be active");
            yield return new WaitUntil(() => player.GetComponent<ArtrobotController>()
            .GetState().Equals("climbing"));
            Debug.Log("Is Climbing Now");
            
        }
        else
        {
            yield return new WaitUntil(() =>
         anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }
        Debug.Log("Chain Reset");

        ChainGroup.GetComponent<ChainController>().setChainList(chainList);
        ChainGroup.GetComponent<ChainController>().ResetChain();

    }


}
