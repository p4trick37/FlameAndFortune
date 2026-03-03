using UnityEngine;

public class WiresPanel : MonoBehaviour
{
    [SerializeField] private RectTransform wire;
    [SerializeField] private RectTransform wireStartPos;
    [SerializeField] private RectTransform wireEndPos;
    private bool usingWire;


    private void Start()
    {
        //WireComplete();
        usingWire = false;
    }

    //Check to see if the player has clicked the mouse button

    

    private void WireComplete()
    {
        wire.anchoredPosition = wireStartPos.anchoredPosition;
        wire.sizeDelta = new Vector2(WireWidth(wireStartPos.anchoredPosition, wireEndPos.anchoredPosition), wire.rect.height);
        wire.rotation = Quaternion.Euler(0, 0, WireRotation(wireStartPos.anchoredPosition, wireEndPos.anchoredPosition));
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && IsCursorInSpot(wireStartPos))
        {
            usingWire = true;
            Debug.Log("Happened");
        }
        if(Input.GetMouseButtonUp(0))
        {
            usingWire = false;
        }

        if(usingWire == true)
        {
            WireMovement();
        }
    }

    //Also Check if the cursor is in a certain area of the screen
    private bool IsCursorInSpot(RectTransform rectTransform)
    {
        Vector2 screenPos = Input.mousePosition;
        float leftSide = rectTransform.position.x - rectTransform.rect.width / 2;
        float rightSide = rectTransform.position.x + rectTransform.rect.width / 2;
        float bottomSide = rectTransform.position.y - rectTransform.rect.height / 2;
        float upSide = rectTransform.position.y + rectTransform.rect.height / 2;
        if(screenPos.x >= leftSide && screenPos.x <= rightSide && screenPos.y >= bottomSide && screenPos.y <= upSide)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void WireMovement()
    {
        Vector2 screenPos = Input.mousePosition;
        Vector2 mouseRelWireStart = (Vector2)wireStartPos.position - screenPos;
        wire.anchoredPosition = wireStartPos.anchoredPosition;
        wire.sizeDelta = new Vector2(WireWidth(wireStartPos.anchoredPosition, mouseRelWireStart), wire.rect.height);
        wire.rotation = Quaternion.Euler(0, 0, WireRotation(wireStartPos.anchoredPosition, mouseRelWireStart));
    }

    private float WireWidth(Vector2 startPos, Vector2 endPos)
    {
        float y = (endPos.y - startPos.y) * (endPos.y - startPos.y);
        float x = (endPos.x - startPos.x) * (endPos.x - startPos.x);
        float width = Mathf.Sqrt(x + y);
        return width;
    }

    private float WireRotation(Vector2 startPos, Vector2 endPos)
    {
        Vector3 onCirclePoint = endPos - startPos;
        float angleDeg = Mathf.Atan(endPos.y / endPos.x) * Mathf.Rad2Deg;
        if (endPos.x < 0)
        {
            angleDeg = 180 + angleDeg;
        }
        else if (endPos.y < 0)
        {
            angleDeg = 360 + angleDeg;
        }
        return angleDeg + 180;
    }


    private RectTransform LockWire(RectTransform startPos, RectTransform endPos)
    {
        RectTransform wire = new RectTransform();
        wire.position = startPos.position;
        wire.sizeDelta = new Vector2(endPos.position.x - startPos.position.x, wire.sizeDelta.y);
        Vector3 onCirclePoint = endPos.position - startPos.position;
        float angleDeg = Mathf.Atan(endPos.position.y / endPos.position.x) * Mathf.Rad2Deg;
        if(endPos.position.x < 0)
        {
            angleDeg = 180 + angleDeg;
        }
        else if(endPos.position.y < 0)
        {
            angleDeg = 360 + angleDeg;
        }
        wire.rotation = Quaternion.Euler(wire.rotation.x, wire.rotation.y, angleDeg);
        return wire;
    }


    //Once the player presses and holds the button, a image spawns that will rotate along the cursor and stretches based on the length

    //Once the player lets go of the wire, if the wire is not on the designated spot, it disapears, if it is, it snaps into place.

}
