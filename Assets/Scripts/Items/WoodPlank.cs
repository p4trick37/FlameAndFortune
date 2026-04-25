using UnityEngine;
using System.Collections;

public class WoodPlank : Item
{
    [SerializeField] private float timeInHand;
    private float timer;
    private BurnableObject burnObj;

    private void Awake()
    {
        burnObj = gameObject.GetComponent<BurnableObject>();
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


}
