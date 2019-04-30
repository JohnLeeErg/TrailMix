using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DetachedPiece : MonoBehaviour
{
    [SerializeField] float regularGravity;
    public float healthIncreaseValue = 1;
    public bool collidedOnce;
    public List<Collider2D> collidedObjs;
    public float timeToCanCollideWithPlayer;
    public Collider2D playerCollider, worldCollider;
    public float gravity;
    [SerializeField] bool careAboutClipping = true;
    public bool hasLanded = true;
    Rigidbody2D myRigidbody;
    float horizontalTimer;
    float maxHoriTime;

    bool hasDoneClipCheck = false;
    // Automated Movement Code
    bool lookingForStoppingPos = false;
    [HideInInspector] public bool foundStoppingPos = false;
    List<Vector3> automatedMovementPositions = new List<Vector3>();
    //Vector3 stopPosition;
    //Vector3 midPosition;
    //Vector3 startPosition;
    //bool atMidPoint = false;
    float percent = 0;
    public float timeToReachTarget = 0.25f;
    public GameObject testGameObject;
    int autoMovementIndex = 1;
    int halfautoIndex = 0;

    public AnimationCurve horizontalEase;
    public AnimationCurve verticalRiseEase;
    public AnimationCurve verticalRunEase;
    IHealth healthInterface;
    public static int sortPeak;
    private void Awake()
    {
        collidedOnce = false;
        myRigidbody = GetComponent<Rigidbody2D>();

        //if (sortPeak == 0)
        //{

        //    sortPeak = 2147483640;
        //}
        foreach(SpriteRenderer eachSprite in GetComponentsInChildren<SpriteRenderer>())
        {
            eachSprite.sortingOrder = sortPeak + 1;
            sortPeak++;
        }
        
    }

    private void OnTriggerStay2D(Collider2D collided)
    {
        if (collided.gameObject.tag == "Player" || collided.gameObject.tag == "HeartHolder")
        {
            if (!collidedOnce)
            {
                myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
                healthInterface = collided.gameObject.GetComponentInParent<IHealth>();
                if (healthInterface)
                {
                    if (healthInterface.health < healthInterface.maxHealth)
                    {
                        healthInterface.OnHeartCollide(healthIncreaseValue, this);
                    }
                }
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasLanded && lookingForStoppingPos == false)
        {
            RestorePhysics();
        }

        //if (collision.gameObject.GetComponent<DetachedPiece>())
        //{
        //    if (collision.collider.OverlapPoint(transform.position))
        //    {
        //        //now you're inside eachother, ruh roh
        //        //no coll
        //        gameObject.layer = 16;
        //        print("fuck ");
        //    }

        //}
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        

    }

    void RestorePhysics()
    {
        myRigidbody.velocity = Vector2.zero;
        myRigidbody.gravityScale = regularGravity;
        hasLanded = true;
    }

    public void SetTimeFromSpeedAndDistance(float distance)
    {
        maxHoriTime = distance / Mathf.Abs(myRigidbody.velocity.x);
    }

    public void IgnoreColl(List<Collider2D> ignoredPlayerColliders, bool ignore)
    {
        collidedObjs = ignoredPlayerColliders;
        for (int i = 0; i < ignoredPlayerColliders.Count; i++)
        {
            Physics2D.IgnoreCollision(ignoredPlayerColliders[i], playerCollider, ignore);
        }
    }

    private void FixedUpdate()
    {
        //if (foundStoppingPos == false)
        //{
        if (!hasLanded && lookingForStoppingPos == false)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y - (gravity * Time.fixedDeltaTime));
            TrackDistance();
        }
        //}
        if (hasLanded)
        {

            //set the player collider to be on grid
            playerCollider.transform.position = Movement.RoundVectorToPoint5s(transform.position);
        }

        if (lookingForStoppingPos == true && hasLanded == false)
        {
            if (Vector3.Distance(transform.position, automatedMovementPositions[autoMovementIndex]) > 0.05f)
            {
                percent += Time.deltaTime / timeToReachTarget;

                float curvePercentX = 0;
                float curvePercentY = 0;
                float animatedxPos = 0;
                float animatedYPos = 0;

                // Calculate the curve transition of both the x and y movement 
                curvePercentX = horizontalEase.Evaluate(percent);
                animatedxPos = Mathf.Lerp(automatedMovementPositions[autoMovementIndex - 1].x, automatedMovementPositions[autoMovementIndex].x, curvePercentX);
                //print(animatedxPos);


                if (autoMovementIndex < halfautoIndex)
                {
                    //print("goodbye");
                    curvePercentY = verticalRiseEase.Evaluate(percent);
                    animatedYPos = Mathf.Lerp(automatedMovementPositions[autoMovementIndex - 1].y, automatedMovementPositions[autoMovementIndex].y, curvePercentY);
                }
                if (autoMovementIndex >= halfautoIndex)
                {
                    //print("hello");
                    curvePercentY = verticalRunEase.Evaluate(percent);
                    animatedYPos = Mathf.Lerp(automatedMovementPositions[autoMovementIndex - 1].y, automatedMovementPositions[autoMovementIndex].y, curvePercentY);
                }


                //transform.position Vector3.Lerp(automatedMovementPositions[autoMovementIndex - 1], automatedMovementPositions[autoMovementIndex], percent);
                transform.position = new Vector2(animatedxPos, animatedYPos);

                // When reach any point but the end of the animaiton
                if (Vector3.Distance(transform.position, automatedMovementPositions[autoMovementIndex]) <= 0.05f && autoMovementIndex < automatedMovementPositions.Count - 1)
                {
                    percent = 0;
                    transform.position = automatedMovementPositions[autoMovementIndex];
                    autoMovementIndex++;
                }
                // REACH END POINT
                else if (Vector3.Distance(transform.position, automatedMovementPositions[autoMovementIndex]) <= 0.05f && autoMovementIndex == automatedMovementPositions.Count - 1)
                {
                    // Figure out if the camper is dropped or not
                    int randomNumber = Random.Range(0, 20 + 1);

                    if (randomNumber > 0)   // STAYS AT SPOT
                    {
                        foundStoppingPos = true;

                        percent = 0;
                        myRigidbody.isKinematic = true;
                        myRigidbody.velocity = Vector3.zero;
                        transform.position = automatedMovementPositions[autoMovementIndex];

                        hasLanded = true;
                    }
                    else if (randomNumber == 0) // MISSES SPOT
                    {
                        foundStoppingPos = true;
                        hasLanded = true;

                        percent = 0;
                        //myRigidbody.velocity = Vector3.zero;
                        transform.position = automatedMovementPositions[autoMovementIndex];

                        RestorePhysics();

                        // Add a physics push to make the fall seem more believable
                        myRigidbody.AddForce((automatedMovementPositions[autoMovementIndex] - automatedMovementPositions[autoMovementIndex - 1]).normalized * 5, ForceMode2D.Impulse);
                        myRigidbody.AddTorque(2, ForceMode2D.Impulse);
                    }
                }
            }
            //if (atMidPoint == false)    // Beginning -> Midpoint
            //{
            //    percent += Time.deltaTime / timeToReachTarget;

            //    transform.position = Vector3.Lerp(startPosition, midPosition, percent);

            //    // Reached the mid Point
            //    if (Vector3.Distance(transform.position, midPosition) < 0.05f)
            //    {
            //        atMidPoint = true;
            //        percent = 0;
            //        transform.position = midPosition;
            //    }
            //}
            //else if (atMidPoint == true)    // Midpoint -> Endpoint
            //{
            //    percent += Time.deltaTime / timeToReachTarget;

            //    transform.position = Vector3.Lerp(midPosition, stopPosition, percent);

            //    // Reached the end Point
            //    if (Vector3.Distance(transform.position, stopPosition) < 0.05f)
            //    {
            //        int randomNumber = Random.Range(0, 10 + 1);

            //        if (randomNumber > 0)
            //        {
            //            foundStoppingPos = true;

            //            percent = 0;
            //            myRigidbody.isKinematic = true;
            //            myRigidbody.velocity = Vector3.zero;
            //            transform.position = stopPosition;

            //            hasLanded = true;
            //        }
            //        else if (randomNumber == 0)
            //        {
            //            foundStoppingPos = true;

            //            percent = 0;
            //            //myRigidbody.velocity = Vector3.zero;
            //            transform.position = stopPosition;

            //            RestorePhysics();

            //            // Add a physics push to make the fall seem more believable
            //            myRigidbody.AddForce((stopPosition - midPosition).normalized * 5, ForceMode2D.Impulse);
            //        }
            //    }
            //}

            //if (Vector3.Distance(transform.position, stopPosition) < 0.7f)
            //{
            //    foundStoppingPos = true;

            //    myRigidbody.isKinematic = true;
            //    myRigidbody.velocity = Vector3.zero;
            //    transform.position = stopPosition;

            //    hasLanded = true;
            //}
        }


        if (!hasDoneClipCheck)
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.NoFilter();
            Collider2D[] results = new Collider2D[10];
            worldCollider.OverlapCollider(filter, results);
            foreach(Collider2D eachCol in results){
                //if you are at all touching a boi on spawn, change layers
                if (eachCol && eachCol.GetComponentInParent<DetachedPiece>() && eachCol.transform.root!=transform.root)
                {
                    worldCollider.gameObject.layer = 16;
                    
                    //print("set to no clip layer");
                    break;
                }
            }
            hasDoneClipCheck = true;
        }
        else 
        {
            if (worldCollider.gameObject.layer == 16 && !worldCollider.IsTouchingLayers(Physics2D.AllLayers) )
            {

                worldCollider.gameObject.layer = LayerMask.NameToLayer("Heart");
                //print("returned to normal layer");
            }
        }
    }

    //public void SetStoppingDestination(Vector3 stoppingPosition, Vector3 midwayPosition, Vector3 startingPosition)
    public void SetStoppingDestination(Vector3[] movementPositions)
    {
        lookingForStoppingPos = true;
        hasLanded = false;
        //stopPosition = stoppingPosition;
        //midPosition = midwayPosition;
        //startPosition = startingPosition;
        myRigidbody.gravityScale = 0;
        percent = 0;
        //atMidPoint = false;

        halfautoIndex = Mathf.RoundToInt(movementPositions.Length / 2) + 1;
        //print(halfautoIndex);

        timeToReachTarget = Random.Range(timeToReachTarget - 0.1f, timeToReachTarget + 0.1f);

        for (int i = 0; i < movementPositions.Length; i++)
        {
            automatedMovementPositions.Add(movementPositions[i]);
        }

        Instantiate(testGameObject, movementPositions[1], Quaternion.identity);
        Instantiate(testGameObject, movementPositions[2], Quaternion.identity);

        // Disable the objects colliders
        //foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
        //{
        //    collider.enabled = false;
        //}
    }

    void TrackDistance()
    {
        if (myRigidbody.velocity.y == 0)
        {
            if (horizontalTimer >= maxHoriTime)
            {
                RestorePhysics();
            }
            else
            {
                //horizontals should count up to maxHoridistance
                horizontalTimer += Time.deltaTime;
            }
        }
    }
    public void RevertCollision()
    {
        StartCoroutine(ResetCollision());
    }

    IEnumerator ResetCollision()
    {
        yield return new WaitForSeconds(timeToCanCollideWithPlayer);
        IgnoreColl(collidedObjs, false);
    }
    
}
