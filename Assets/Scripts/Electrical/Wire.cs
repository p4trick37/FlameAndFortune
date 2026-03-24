using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Wire : MonoBehaviour
{
    [SerializeField] private RectTransform wire;
    [SerializeField] private RectTransform wireStartPos;
    [SerializeField] private RectTransform wireEndPos;
    [SerializeField] private RectTransform parentImage;
    private bool usingWire;
    private bool wireCompleted;
    private bool wireSelected;
    private int wireIndex;

    private void Start()
    {
        usingWire = false;
        wire.sizeDelta = new Vector2(0, wire.sizeDelta.y);
    }

    private void Update()
    {
        GameObject currentSelectedObj = HoveringObject();

        if(Input.GetMouseButtonDown(0) && IsCursorInSpot(wireStartPos) && wireCompleted == false)
        {
            usingWire = true;
            wireCompleted = false;
            wireSelected = true;
        }
        else if(Input.GetMouseButtonDown(0) && currentSelectedObj.CompareTag("Wire Image"))
        {
            WireImage wireImageFound = currentSelectedObj.GetComponent<WireImage>();
            if(wireImageFound != null)
            {
                if(wireImageFound.imageIndexed == wire.gameObject.GetComponent<WireImage>().imageIndexed)
                {
                    wire.sizeDelta = new Vector2(0, wire.rect.height);
                    wireSelected = false;
                    usingWire = false;
                    wireCompleted = false;
                }
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            usingWire = false;
            if(IsCursorInSpot(wireEndPos) && wireSelected == true)
            {
                WireComplete();
                wireCompleted = true;
                wireSelected = false;
            }
            else if(wireCompleted == false)
            {
                wire.sizeDelta = new Vector2(0, wire.rect.height);
                wireSelected = false;
            }
        }


        if(usingWire == true)
        {
            WireMovement();
        }
    }

    private bool IsCursorInSpot(RectTransform rectTransform)
    {
        Vector2 screenPos = Input.mousePosition;
        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPos, null, out localPoint))
        {
            float halfWidth = rectTransform.rect.width / 2;
            float halfHeight = rectTransform.rect.height / 2;
            if (localPoint.x >= -halfWidth && localPoint.x <= halfWidth && localPoint.y >= -halfHeight && localPoint.y <= halfHeight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void WireMovement()
    {
        Vector2 screenPos = Input.mousePosition;
        Vector2 localPoint;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(wireStartPos, screenPos, null, out localPoint))
        {
            wire.anchoredPosition = wireStartPos.anchoredPosition;
            wire.sizeDelta = new Vector2(WireWidth(Vector2.zero, localPoint), wire.rect.height);
            wire.rotation = Quaternion.Euler(0, 0, WireRotation(Vector2.zero, localPoint));
        }
    }

    private void WireComplete()
    {
        wire.anchoredPosition = wireStartPos.anchoredPosition;
        wire.sizeDelta = new Vector2(WireWidth(wireStartPos.anchoredPosition, wireEndPos.anchoredPosition), wire.rect.height);
        wire.rotation = Quaternion.Euler(0, 0, WireRotation(wireStartPos.anchoredPosition, wireEndPos.anchoredPosition));
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
        float angleDeg = Mathf.Atan(onCirclePoint.y / onCirclePoint.x) * Mathf.Rad2Deg;
        if (endPos.x < 0)
        {
            angleDeg = 180 + angleDeg;
        }
        else if (endPos.y < 0)
        {
            angleDeg = 360 + angleDeg;
        }
        return angleDeg;
    }

    private GameObject HoveringObject()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;

        List<RaycastResult> hitResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, hitResults);

        if(hitResults.Count > 0)
        {
            return hitResults[0].gameObject;
        }
        return null;    
    }
}
