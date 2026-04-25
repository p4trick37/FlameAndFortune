using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Runtime.CompilerServices;
using UnityEditor.UIElements;
public class GameManager : MonoBehaviour
{
    [Header("Burns Completed")]
    [SerializeField] private List<BurnableObject> allBurnableObjects = new List<BurnableObject>();
    [SerializeField] private int numberOfBurns;
    [SerializeField] private int numberOfObjects;
    [SerializeField] private float percentBurned;
    [SerializeField] private TMP_Text percentBurnedText;
    [Header("Timer")]
    [SerializeField] private float burnTime;
    [SerializeField] private TMP_Text timerText;
    private float timer;
    private bool gameOver = false;
    private bool wentToGameOver = false;
    [Header("Player")]
    [SerializeField] private Transform playerSpawnPoint;
    private bool spawnPlayer = false;
    private bool dummybool = true;
    [Header("Money")]
    [SerializeField] private int maxAmountOfMoney;

    public float PercentBurnedValue => percentBurned;
    
    
    
    private void Awake()
    {
        BurnableObject[] objects = FindObjectsByType<BurnableObject>(FindObjectsSortMode.None);
        for(int i = 0; i < objects.Length; i++)
        {
            if (objects[i].gameObject.GetComponent<WoodPlank>() == null)
            {
                allBurnableObjects.Add(objects[i]);
            }
        }
        numberOfObjects = allBurnableObjects.Count;
        spawnPlayer = true;


    }

    private void OnEnable()
    {
        Player.instance.gameObject.transform.position = playerSpawnPoint.position;
    }

    private void Start()
    {
        timer = burnTime;
        Player.instance.FindObjectsInScene();
        FindUIElements();
    }

    private void Update()
    {
        if (spawnPlayer == true && dummybool == true)
        {
            
            dummybool = false;
        }
        Timer();
        if(gameOver == false)
        {
            CheckForBurns();
            percentBurned = PercentBurned(numberOfBurns, numberOfObjects);
        }
        else if(wentToGameOver == false)
        {
            GameOver();
            wentToGameOver = true;
        }
        percentBurnedText.text = percentBurned.ToString("F0") + "%";

        spawnPlayer = true;
    }

  private void CheckForBurns()
{
    for (int i = allBurnableObjects.Count - 1; i >= 0; i--)
    {
        if (allBurnableObjects[i].CurrentHealth <= 0)
        {
            numberOfBurns++;
            allBurnableObjects.RemoveAt(i);
        }
    }
}

    private float PercentBurned(int currentBurns, int numOfBurnObj)
    {
        float percent = ((float)currentBurns / numOfBurnObj) * 100;
        return percent;
    }

    private void Timer()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            gameOver = true;
            timer = 0;
        }
        timerText.text = timer.ToString("F2");
    }

    private void GameOver()
    {
        Debug.Log("GAME IS OVER DUDE");
        Player.instance.ClearInventory();
        Player.instance.gameObject.GetComponent<PlayerData>().AddMoney(MoneyMade());
    }

    private int MoneyMade()
    {
        int moneyMade = (int)Mathf.Lerp(0, maxAmountOfMoney, percentBurned / 100);
        return moneyMade;
    }

    private void FindUIElements()
    {
        percentBurnedText = Player.instance.PercentCompleteTxt;
        timerText = Player.instance.InGameTimerTxt;
    }

}
