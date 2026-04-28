using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private int gasPrice;
    [SerializeField] private int torchPrice;
    [SerializeField] private int woodPrice;
    public void ChangeGasCan(int amount)
    {
        if(CheckForMoney(gasPrice) == true)
        {
            GasCan.AddHealth(amount);
            Player.instance.gameObject.GetComponent<PlayerData>().RemoveMoney(gasPrice);
        }
    }

    public void ChangeWoodPlank(int amount)
    {
        if(CheckForMoney(woodPrice) == true)
        {
            WoodPlank.AddHealth(amount);
            Player.instance.gameObject.GetComponent<PlayerData>().RemoveMoney(woodPrice);
        }
    }

    public void ChangeTorch(int amount)
    {
        if (CheckForMoney(torchPrice) == true)
        {
            TorchFireTool.AddHealth(amount);
            Player.instance.gameObject.GetComponent<PlayerData>().RemoveMoney(torchPrice);
        }
    }

    public void GoToNextLevel()
    {
        SceneManager.LoadScene("Level1");
    }

    private void Start()
    {
        Player.instance.SetPlayer("Upgrade");
    }

    private void Update()
    {
        SetPlayerPos();
        UpdateMoneyText();
    }

    private void SetPlayerPos()
    {
        if (Player.instance != null)
        {
            Player.instance.gameObject.transform.position = new Vector3(0, -200, 0);
        }
    }

    private bool CheckForMoney(int amount)
    {
        if(Player.instance != null)
        {
            if(amount < Player.instance.gameObject.GetComponent<PlayerData>().Money)
            {
                return true;
            }
        }
        
        return false;
    }

    private void UpdateMoneyText()
    {
        if (Player.instance != null)
        {
            string text = "$" + Player.instance.gameObject.GetComponent<PlayerData>().Money.ToString("N0");
            moneyText.text = text;
        }
    }
}
