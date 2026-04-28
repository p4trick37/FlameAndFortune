using UnityEngine;
using System.Collections;

public class WoodPlank : Item
{
    [SerializeField] private float timeInHand;
    private float timer;
    private BurnableObject burnObj;
    private static int amountChange;

    private void Awake()
    {
        burnObj = gameObject.GetComponent<BurnableObject>();
        burnObj.SetAddedHealth(amountChange);
        amountChange = 0;
        timer = timeInHand;
        
    }

    private void Update()
    {
        if(inInventory == true && burnObj.IsBurning == true)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                Player.instance.DestroyCertainItem(gameObject);
            }
        }
        else
        {
            SetTimer();
        }
    }

    private void SetTimer()
    {
        timer = timeInHand;
    }

    public override void OnDrop()
    {
        inInventory = false;
        ChangeLayerMask(gameObject, "Burnable");
    }

    public static void AddHealth(int amount)
    {
        amountChange += amount;
    }

}
