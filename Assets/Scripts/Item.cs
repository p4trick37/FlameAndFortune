using UnityEngine;

public class Item : MonoBehaviour, IPickupable
{
    [Header("Inventory Management")]
    public int SlotNumber => slotNumber;
    [SerializeField] private int slotNumber;
    [SerializeField] private GameObject playerHand;
    private bool inInventory = false;
    protected bool currentlySelecting = false;

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
        return gameObject;
    }

    protected void CheckForSelection()
    {
        if (gameObject.activeSelf == true && inInventory == true)
        {
            currentlySelecting = true;
        }
        else
        {
            currentlySelecting = false;
        }
    }

    public void ChangeGravity()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if(rb != null)
        {
            if(rb.useGravity == false)
            {
                rb.useGravity = true;
            }
            else
            {
                rb.useGravity = false;
            }
        }
    }
}
