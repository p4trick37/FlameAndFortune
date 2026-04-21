using UnityEngine;

public interface IPickupable 
{
    public void SetToHand();

    public void GetSlotNumber(int index);

    public GameObject OnPickup();

    public void ChangeRigidbodyState();

}
