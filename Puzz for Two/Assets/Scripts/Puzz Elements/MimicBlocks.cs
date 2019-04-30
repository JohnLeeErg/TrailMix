using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicBlocks : MonoBehaviour
{

    public enum SpatialStyle
    {
        SubtractShape,
        BecomeShape,
    }

    public SpatialStyle spatialStyle;
    public Button buttonActivated;
    private GameObject character; //the player block parent

    [Header("Add Shape Variables")]
    public GameObject spawnBlock;

    [Header("Subtract Shape Variables")]
    public LayerMask groundDestroyMask;
    private MeshRenderer meshRComp;
    private Collider2D coll2DComp;
    private List<GameObject> disabledBlocks = new List<GameObject>();

    private void Awake()
    {
        meshRComp = GetComponent<MeshRenderer>();
        coll2DComp = GetComponent<Collider2D>();
    }

    //find a way to determine the shape of the character and base it off of that... - EVEN if we change the shape of the characters
    public void ActivateShape(Button presser)
    {
        character = presser.owner.transform.root.GetComponent<PlayerHealth>().playerBlockParent.gameObject;
        switch (spatialStyle)
        {
            case SpatialStyle.BecomeShape:
                for (int i = 0; i < character.transform.childCount; i++)
                {
                    if (character.transform.GetChild(i).gameObject.activeInHierarchy) //if active
                    {
                        GameObject spawned = Instantiate(spawnBlock, transform);
                        spawned.transform.localPosition = character.transform.GetChild(i).localPosition;
                    }
                }
                break;
            case SpatialStyle.SubtractShape:
                disabledBlocks.Clear();
                for (int i = 0; i < character.transform.childCount; i++)
                {
                    if (character.transform.GetChild(i).gameObject.activeInHierarchy) //if active
                    {
                        //check localPosition
                        foreach (Collider2D ground in Physics2D.OverlapPointAll(transform.position + character.transform.GetChild(i).localPosition, groundDestroyMask))
                        {
                            ground.gameObject.SetActive(false);
                            disabledBlocks.Add(ground.gameObject);
                        }
                    }
                }
                //coll2DComp.enabled = false;
                //meshRComp.enabled = false;
                break;
        }
    }

    public void DeactivateShape()
    {
        switch (spatialStyle)
        {
            case SpatialStyle.BecomeShape:
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
                break;
            case SpatialStyle.SubtractShape:
                for (int i = 0; i < disabledBlocks.Count; i++)
                {
                    disabledBlocks[i].SetActive(true);
                }
                //coll2DComp.enabled = true;
                //meshRComp.enabled = true;
                break;
        }
    }
}
