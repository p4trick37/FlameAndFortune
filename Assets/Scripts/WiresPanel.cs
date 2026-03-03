using UnityEngine;

public class WiresPanel : MonoBehaviour
{
    [SerializeField] private RectTransform wireImage;
    [SerializeField] private RectTransform wireStartPos;
    [SerializeField] private RectTransform wireEndPos;
    private bool usingWire;


    private void Start()
    {
        wireImage = LockWire(wireStartPos, wireEndPos);
        Instantiate(wireImage);
    }

    //Check to see if the player has clicked the mouse button

    //private void Update()
    //{
    //if (Input.GetMouseButton(0) && IsCursorInSpot(wireStartPos))
    //{
    //usingWire = true;
    //}
    //if(Input.GetMouseButtonUp(0))
    //{
    // usingWire = false;
    //}
    //}

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

    private void SpawnWire(RectTransform anchor)
    {
        Vector2 screenPos = Input.mousePosition;
        


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
