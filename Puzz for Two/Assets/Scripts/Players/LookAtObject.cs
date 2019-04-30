using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    [SerializeField] Transform pivotPoint;
    public Transform targetLook;
    [SerializeField] bool flipX = false;
    [SerializeField] float magnitude;
    float flipBuffer = .1f;
    private Vector2 dirToTarget;
    private Vector2 offset;
    SpriteRenderer spriteRenderComp;
    public bool randomizeFace = true, autofindLookTarget;
    // vv figure out a better way to do this later (scriptable objects)
    //public Sprite[] sprites;
    public FaceProfile possibleFaces;
    List<Sprite> runTimeSprites;
    Sprite randomSprite;
    public bool alreadyGotSpecialFace;
    private void Start()
    {
        spriteRenderComp = GetComponent<SpriteRenderer>();
        if (flipX)
        {
            GetComponentInParent<SpriteRenderer>().flipX = true;
        }
        if (randomizeFace)
        {
            if (possibleFaces && !alreadyGotSpecialFace)// dont generate a normal random face if they've already been given a dumb special face
            {
                spriteRenderComp.sprite = possibleFaces.GetRandomFace();
            }
        }
        if (autofindLookTarget)
        {
            //this logic can probably be whatever but ye make em look at a random player for now I guess?

            targetLook = GameObject.Find("Player " + Random.Range(1, 3)).transform;
}
    }

    private void Update()
    {
        CalculatePositionAndRotation();
        FlipSprite();
    }

    void CalculatePositionAndRotation()
    {
        Vector2 heading = targetLook.position - transform.position;
        float distance = Mathf.Abs(heading.x);
        if (distance >= flipBuffer)
        {
            dirToTarget = heading / distance;
            float angleOfRotational = Mathf.Atan2(dirToTarget.x, dirToTarget.y);
            Vector2 normalizedV = new Vector2(dirToTarget.x, dirToTarget.y).normalized;

            offset = Quaternion.AngleAxis(angleOfRotational, pivotPoint.transform.forward) * normalizedV * magnitude;

            if (!flipX)
            {
                transform.position = pivotPoint.position + (Vector3)offset;
            }
            else
            {
                transform.localPosition = new Vector3(-pivotPoint.localPosition.x, pivotPoint.localPosition.y, pivotPoint.localPosition.z);
                transform.position = transform.position + (Vector3)offset;
            }
        }
        else
        {
            if (!flipX)
            {
                transform.position = pivotPoint.position + (Vector3)offset;
            }
            else
            {
                transform.localPosition = new Vector3(-pivotPoint.localPosition.x, pivotPoint.localPosition.y, pivotPoint.localPosition.z);
                transform.position = transform.position + (Vector3)offset;
            }
        }
    }

    void FlipSprite()
    {
        Vector2 heading = targetLook.position - transform.position;
        float distance = Mathf.Abs(heading.x);
        if (distance > flipBuffer)
        {
            if (offset.x >= 0 && spriteRenderComp.flipX)
            {
                spriteRenderComp.flipX = false;
            }
            else if (offset.x < 0 && !spriteRenderComp.flipX)
            {
                spriteRenderComp.flipX = true;
            }
        }

    }

    public void SetRandomFace(FaceProfile faceProfileUsed)
    {
        if (!spriteRenderComp)
        {
            spriteRenderComp = GetComponent<SpriteRenderer>();
        }
        spriteRenderComp.sprite = faceProfileUsed.GetRandomFace();
        alreadyGotSpecialFace = true;
    }

    public void SetToPivotPosition()
    {
        transform.position = pivotPoint.position + (Vector3)offset;
    }

}
