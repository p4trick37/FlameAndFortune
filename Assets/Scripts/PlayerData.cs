using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int Money => money;
    [SerializeField] private int money;

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void RemoveMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
        }
    }
}
