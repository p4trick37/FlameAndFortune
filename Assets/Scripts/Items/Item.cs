using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Item : MonoBehaviour, IPickupable
{
    [Header("Inventory Management")]
    public int SlotNumber => slotNumber;
    [SerializeField] private int slotNumber;
    [SerializeField] private GameObject playerHand;
    protected bool inInventory = false;
    protected bool currentlySelecting = false;
    [SerializeField] private Sprite image;

    private void OnEnable()
    {
        playerHand = FindHand();

    }

   
    public virtual GameObject FindHand()
    {
        GameObject hand = GameObject.Find("Hand");
        return hand;
    }

    public virtual void SetToHand()
    {
        transform.SetParent(playerHand.transform);
        transform.position = playerHand.transform.position;
        transform.rotation = playerHand.transform.rotation;
    }



    public virtual void GetSlotNumber(int index)
    {
        slotNumber = index;
    }

    public virtual GameObject OnPickup()
    {
        inInventory = true;
        ChangeLayerMask(gameObject, "MaskObject");
        return gameObject;
    }

    protected void CheckForSelection()
    {
        if (gameObject.activeInHierarchy == true && inInventory == true)
        {
            currentlySelecting = true;
        }
        else
        {
            currentlySelecting = false;
        }
    }

    public void ChangeRigidbodyState()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if(rb != null)
        {
            if(rb.useGravity == false)
            {
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.None;
            }
            else
            {
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }

    public virtual void OnDrop()
    {
        inInventory = false;
        ChangeLayerMask(gameObject, "Default");
    }

    public virtual Sprite GetImage()
    {
        return image;
    }

    protected virtual void ChangeLayerMask(GameObject obj, string maskName)
    {
        if(obj == null)
        {
            return;
        }

        obj.layer = LayerMask.NameToLayer(maskName);
        foreach(Transform childInObj in obj.transform)
        {
            ChangeLayerMask(childInObj.gameObject, maskName);
        }


    }

}
