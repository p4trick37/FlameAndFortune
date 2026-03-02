using System.Collections;
using UnityEngine;

public class WiresPanel : MonoBehaviour
{
    [SerializeField] private RectTransform clickWire;

    
    public void InteractClick()
    {
        Debug.Log("This Works");
    }

    //Check to see if the player has clicked the mouse button

    private void Update()
    {
        
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


    //Once the player presses and holds the button, a image spawns that will rotate along the cursor and stretches based on the length

    //Once the player lets go of the wire, if the wire is not on the designated spot, it disapears, if it is, it snaps into place.

}
