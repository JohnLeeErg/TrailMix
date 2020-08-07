using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Button : MonoBehaviour
{
    [HideInInspector] ButtonParent buttonCheckParent;
    public Color deactivatedColor;
    public Color activatedColor;
    public bool isActivated, requireExactBlock, sticky, playSound = true;
    public GameObject owner;
    SpriteRenderer spComponent;
    Collider2D myCol;
    float buttonOpacityBoost=.4f;
    public SpriteRenderer buttonHighlightSprite;

    // Create FMOD Sound Effect Variables
    [Header("FMOD Audio Events")]
    [FMODUnity.EventRef]
    public string buttonPressSound;
    FMOD.Studio.EventInstance fModButtonPressEvent;
    [SerializeField] bool autoColor,dontFlashColor;
    Tilemap[] gateRefs;
    void Awake()
    {
        buttonCheckParent = GetComponentInParent<ButtonParent>();
        buttonCheckParent.buttonsInPuzzle.Add(this);
        spComponent = GetComponent<SpriteRenderer>();
        spComponent.color = deactivatedColor;
        isActivated = false;
        myCol = GetComponent<Collider2D>();

        if (transform.childCount > 0)
        {
            GameObject child = transform.GetChild(0).gameObject;
            buttonHighlightSprite = child.GetComponentInChildren<SpriteRenderer>();
            if (buttonHighlightSprite.enabled == true)
            {
                buttonHighlightSprite.enabled = false;
            }
        }
        gateRefs = transform.parent.GetComponentsInChildren<Tilemap>();
        if (gateRefs.Length>0)
        {
            activatedColor = new Color(gateRefs[0].color.r, gateRefs[0].color.g, gateRefs[0].color.b, activatedColor.a+buttonOpacityBoost);
            deactivatedColor = new Color(gateRefs[0].color.r, gateRefs[0].color.g, gateRefs[0].color.b, deactivatedColor.a+buttonOpacityBoost);
        }

        if (buttonHighlightSprite != null)
            buttonHighlightSprite.color = new Color(deactivatedColor.r, deactivatedColor.g, deactivatedColor.b, buttonHighlightSprite.color.a+buttonOpacityBoost);
    }

    private void Start()
    {
        fModButtonPressEvent = FMODUnity.RuntimeManager.CreateInstance(buttonPressSound);
    }

    // Update is called once per frame
    void Update()
    {
        if (!sticky)
        {
            if (!myCol.IsTouchingLayers())
            {

                DeactivateButtonNoLayers();
            }
        }
    }
    public void FadeAway()
    {
        playSound = false;
        myCol.enabled = false;
        DisableSubColliders();
        StartCoroutine(FadeMe());
    }

    IEnumerator FadeMe()
    {
        Color startColor = spComponent.color;
        Color startColorH = buttonHighlightSprite.color;
        float timer = 0;
        while (startColor.a > 0 || startColorH.a > 0)
        {
            spComponent.color = Color.Lerp(startColor, Color.clear, timer / 1f);

            buttonHighlightSprite.color = Color.Lerp(startColorH, Color.clear, timer / 1f);

            timer += Time.deltaTime;
            yield return null;
        }
        yield return null;

    }
    IEnumerator FlashGate(Tilemap gate)
    {
        yield return null;
        Color startColor = new Color (activatedColor.r,activatedColor.g,activatedColor.b,1);
        Color targetColor= activatedColor+Color.gray*.5f;
        //print(Color.white * (buttonCheckParent.currentlyActiveButtons / buttonCheckParent.buttonsInPuzzle.Count));
        //print(buttonCheckParent.currentlyActiveButtons +" "+ buttonCheckParent.buttonsInPuzzle.Count);
        float timer = 0;
        while (gate.color !=targetColor)
        {
            gate.color = Color.Lerp(startColor, targetColor, timer / .1f);
            

            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        startColor= activatedColor + Color.gray * .2f;
        while (gate.color != startColor)
        {
            gate.color = Color.Lerp(targetColor, startColor, timer / .1f);


            timer += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator FlashGateOff(Tilemap gate)
    {

        float   timer = 0;
        Color startColor = new Color(activatedColor.r, activatedColor.g, activatedColor.b, 1); 
        Color currentColor=gate.color;
        while (gate.color != startColor)
        {
            gate.color = Color.Lerp(currentColor, startColor, timer / .1f);


            timer += Time.deltaTime;
            yield return null;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isActivated && collision.gameObject.tag == "Player" && buttonCheckParent.IsNewOwner(collision.gameObject) && (!requireExactBlock || SameRoundedPosition(collision.gameObject.transform.position)))
        {
            ActivateButton(collision.gameObject);
        }

    }

    /// <summary>
    /// make sure the player's grid position is the same as this buttons grid position
    /// </summary>
    /// <param name="playerPiecePos"></param>
    /// <returns></returns>
    bool SameRoundedPosition(Vector3 playerPiecePos)
    {

        return Movement.RoundVectorToPoint5s(transform.position) == Movement.RoundVectorToPoint5s(playerPiecePos);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && owner == collision.gameObject)
        {
            DeactivateButton();

        }
    }

    // <marty>
    // These functions are separate/public now so they can be called from other scripts
    // Sorry, I needed it for the level where the buttons stick to the screen

    public void ActivateButton(GameObject touchingGuy)
    {
        spComponent.color = activatedColor;
        if (buttonHighlightSprite != null)
        {
            buttonHighlightSprite.enabled = true;
        }
        isActivated = true;
        owner = touchingGuy;
        if (!dontFlashColor)
        {
            foreach (Tilemap eachGate in gateRefs)
            {
                StartCoroutine(FlashGate(eachGate));
            }
        }
        if (playSound)
            fModButtonPressEvent.start();
    }

    public void DeactivateButton()
    {
            spComponent.color = deactivatedColor;
            isActivated = false;
            owner = null;
            if (!dontFlashColor)
            {
                StopAllCoroutines();
                foreach (Tilemap eachGate in gateRefs)
                {
                    StartCoroutine(FlashGateOff(eachGate));
                }
            }
    }
    public void DeactivateButtonNoLayers()
    {
        owner = null;
        spComponent.color = deactivatedColor;
        if (buttonHighlightSprite != null)
        {
            buttonHighlightSprite.enabled = false;
        }
        isActivated = false;
    }
    
    void DisableSubColliders()
    {
        ButtonSubcollider[] subcolliders = GetComponentsInChildren<ButtonSubcollider>();
        if (subcolliders != null)
        {
            foreach (ButtonSubcollider colliderObject in subcolliders)
            {
                colliderObject.gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }
    }
    // </marty>
}
