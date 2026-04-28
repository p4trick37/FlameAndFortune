using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class OpenWorldManager : MonoBehaviour
{
    [Header("Player Boundns")]
    [SerializeField] private float xMin;
    [SerializeField] private float xMax;
    [SerializeField] private float zMin;
    [SerializeField] private float zMax;
    [Header("Level Manager")]
    [SerializeField] private SceneSwitcher sceneSwitcher;
    [SerializeField] private GameObject level1Door;
    [SerializeField] private TMP_Text moneyText;
    private bool playerEnterLevel;
    [SerializeField] private Transform playerSpawnPoint;

    private void Start()
    {
        Player.instance.FindObjectsInScene();
        Player.instance.gameObject.transform.position = playerSpawnPoint.position;
        FindUIElements();
    }

    private void Update()
    {
        if(playerEnterLevel == true)
        {
            sceneSwitcher.SwitchToScene("Level1");
        }

        UpdateMoneyText();
    }
    private void LateUpdate()
    {
        Vector3 playerPos = Player.instance.gameObject.transform.position;
        if(playerPos.x < xMin)
        {
            Player.instance.gameObject.transform.position = new Vector3(xMin, playerPos.y, playerPos.z);
        }
        else if(playerPos.x > xMax)
        {
            Player.instance.gameObject.transform.position = new Vector3(xMax, playerPos.y, playerPos.z);
        }
        else if(playerPos.z < zMin)
        {
            Player.instance.gameObject.transform.position = new Vector3(playerPos.x, playerPos.y, zMin);
        }
        else if(playerPos.z > zMax)
        {
            Player.instance.gameObject.transform.position = new Vector3(playerPos.x, playerPos.y, zMax);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(xMin, 1, zMin), new Vector3(xMax, 1, zMin));
        Gizmos.DrawLine(new Vector3(xMin, 1, zMin), new Vector3(xMin, 1, zMax));
        Gizmos.DrawLine(new Vector3(xMax, 1, zMax), new Vector3(xMin, 1, zMax));
        Gizmos.DrawLine(new Vector3(xMax, 1, zMax), new Vector3(xMax, 1, zMin));
    }

    public GameObject LevelCollider()
    {
        return level1Door;
    }

    public void PlayerEnterLevel()
    {
        playerEnterLevel = true;
    }

    private void FindUIElements()
    {
        //moneyText = Player.instance.MoneyTxt;
    }

    private void UpdateMoneyText()
    {
        string text = "$" + Player.instance.gameObject.GetComponent<PlayerData>().Money.ToString("N0");
        moneyText.text = text;
    }
}
