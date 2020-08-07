using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSubcollider : MonoBehaviour
{

    public bool isActivated;
    public GameObject owner;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isActivated && collision.gameObject.tag == "Player")
        {
            isActivated = true;
            owner = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && owner == collision.gameObject)
        {
            isActivated = false;
            owner = null;
        }
    }
}
