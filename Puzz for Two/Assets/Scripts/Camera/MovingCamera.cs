using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCamera : MonoBehaviour
{
    public Camera cam;
    public List<Transform> targets = new List<Transform>();
    private IEnumerator movementCoRou;
    public float minThreshold, minXThreshhold, yOffsetValue;
    public float moveSpeed;
    public float biasOffsetMag, ghostDistance;
    public float xArrowClamp, yArrowClamp;
    public bool biasCamera;
    //public bool lockYAxis = false;
    SpriteRenderer p1, p2, p1GhostSprite,p2GhostSprite;
    Transform p1Face, p2Face, p2GhostFace, p1GhostFace;
    [SerializeField] GameObject p1Arrow, p2Arrow, p1Ghost, p2Ghost;

    private void Awake()
    {
        if (targets.Count <= 0)
        {
            targets.Add(GameObject.Find("Player 1").transform);
            targets.Add(GameObject.Find("Player 2").transform);
        }
        p1 = targets[0].GetComponentInChildren<SpriteRenderer>();
        p2 = targets[1].GetComponentInChildren<SpriteRenderer>();
        p1Face = p1.GetComponentInChildren<LookAtObject>().transform;

        p2Face = p2.GetComponentInChildren<LookAtObject>().transform;
        p1GhostSprite = p1Ghost.GetComponentInChildren<SpriteRenderer>();

        p2GhostSprite = p2Ghost.GetComponentInChildren<SpriteRenderer>();

        p1GhostFace = p1GhostSprite.GetComponentInChildren<Animator>().transform;
        p2GhostFace = p2GhostSprite.GetComponentInChildren<Animator>().transform;

        //manually set to pos right away
        transform.position = new Vector3(AveragePosition().x, AveragePosition().y, cam.transform.position.z);
    }
    void FixedUpdate()
    {
        if (biasCamera)
        {
            if (GetDistance(cam.transform.position, AveragePosition()) > minThreshold)
            {
                if ((p2.transform.position - p1.transform.position).magnitude < biasOffsetMag)
                {
                    Vector3 targetPosition= new Vector3(AveragePosition().x, AveragePosition().y, cam.transform.position.z);
                    cam.transform.position = new Vector3(Mathf.Lerp(cam.transform.position.x, targetPosition.x, moveSpeed * Time.deltaTime), Mathf.Lerp(cam.transform.position.y, targetPosition.y, moveSpeed * Time.deltaTime), cam.transform.position.z);
                    
                    // cam.transform.position = new Vector3(cam.transform.position.x, AveragePosition().y, cam.transform.position.z);
                }
                else
                {
                    StopAllCoroutines();

                    Vector3 lastOnScreenPoint = (p1.transform.position + (p2.transform.position - p1.transform.position).normalized * biasOffsetMag);

                    Vector3 targetPosition = new Vector3(AveragePosition().x, AveragePosition().y, cam.transform.position.z)+lastOnScreenPoint;
                    cam.transform.position = new Vector3(Mathf.Lerp(cam.transform.position.x, targetPosition.x, moveSpeed * Time.deltaTime), Mathf.Lerp(cam.transform.position.y, targetPosition.y, moveSpeed * Time.deltaTime), cam.transform.position.z);

                }

            }
        }
        else
        {

            Vector3 targetPosition = new Vector3(AveragePosition().x, AveragePosition().y, cam.transform.position.z);
            cam.transform.position = new Vector3(Mathf.Lerp(cam.transform.position.x, targetPosition.x, moveSpeed * Time.deltaTime), Mathf.Lerp(cam.transform.position.y, targetPosition.y, moveSpeed * Time.deltaTime), cam.transform.position.z);
            //Alternate method that does not use hellish lerps:
            //cam.transform.position = new Vector3(Mathf.MoveTowards(cam.transform.position.x, targetPosition.x, moveSpeed * Mathf.Abs(targetPosition.x - cam.transform.position.x) * Time.deltaTime), Mathf.MoveTowards(cam.transform.position.y, targetPosition.y, moveSpeed * Mathf.Abs(targetPosition.y - cam.transform.position.y) * Time.deltaTime), cam.transform.position.z);

            // cam.transform.position = new Vector3(cam.transform.position.x, AveragePosition().y, cam.transform.position.z);

        }
        if (!p1.isVisible)
        {
            Vector3 direction = (p1.transform.position - p1Arrow.transform.position).normalized;
            p1Arrow.SetActive(true);
            p1Ghost.SetActive(true);
            p1Arrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            p1Arrow.transform.position = p1.transform.position;
            p1Arrow.transform.localPosition = new Vector3(Mathf.Clamp(p1Arrow.transform.localPosition.x, -xArrowClamp, xArrowClamp),
                                                        Mathf.Clamp(p1Arrow.transform.localPosition.y, -yArrowClamp, yArrowClamp),
                                                        p1Arrow.transform.localPosition.z);
            p1Ghost.transform.position = p1Arrow.transform.position - direction * ghostDistance;

            //animate the ghosty boi
            p1GhostSprite.sprite = p1.sprite;
            p1GhostFace.localPosition = p1Face.localPosition;
        }
        else
        {
            p1Arrow.SetActive(false);

            p1Ghost.SetActive(false);
        }
        if (!p2.isVisible)
        {
            Vector3 direction = (p2.transform.position - p2Arrow.transform.position).normalized;
            p2Arrow.SetActive(true);

            p2Ghost.SetActive(true);
            p2Arrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, p2.transform.position - p2Arrow.transform.position);
            p2Arrow.transform.position = p2.transform.position;
            p2Arrow.transform.localPosition = new Vector3(Mathf.Clamp(p2Arrow.transform.localPosition.x, -xArrowClamp, xArrowClamp),
                                                        Mathf.Clamp(p2Arrow.transform.localPosition.y, -yArrowClamp, yArrowClamp),
                                                        p2Arrow.transform.localPosition.z);
            p2Ghost.transform.position = p2Arrow.transform.position - direction * ghostDistance;


            //animate the ghosty boi
            p2GhostSprite.sprite = p2.sprite;
            p2GhostFace.localPosition = p2Face.localPosition;

        }
        else
        {
            p2Arrow.SetActive(false);

            p2Ghost.SetActive(false);
        }
    }

    public void RemoveAllTargets()
    {
        targets.RemoveRange(0, targets.Count);
    }

    public void RemoveTarget(GameObject targetToRemove)
    {
        if (targets.Contains(targetToRemove.transform))
        {
            int removeIndex = targets.IndexOf(targetToRemove.transform);
            targets.RemoveAt(removeIndex);
        }
    }

    public void AddTarget(GameObject newTarget)
    {
        targets.Add(newTarget.transform);
    }

    public Vector2 AveragePosition()
    {
        Vector2 averagePos = Vector2.zero;
        for (int i = 0; i < targets.Count; i++)
        {
            averagePos += (Vector2)targets[i].position;
        }
        averagePos /= targets.Count;

        if (yOffsetValue > 0)
        {
            averagePos.y += yOffsetValue;
        }

        return averagePos;
    }

    public float GetDistance(Vector2 origin, Vector2 destination)
    {
        float dist = Vector2.Distance(origin, destination);
        return dist;
    }

    //IEnumerator MoveToPosition(Vector3 targetPosition)
    //{
        
            
        
    //}
}
