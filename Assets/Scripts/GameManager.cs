using UnityEngine;
using System.Collections.Generic;
using TMPro;
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
    [Header("Player")]
    [SerializeField] private Transform playerSpawnPoint;
    private Player player;
    private bool spawnPlayer = false;
    private bool dummybool = true;
    
    
    
    private void Awake()
    {
        player = Player.instance;
        player.gameObject.transform.position = playerSpawnPoint.position;
        Debug.Log("Setting player Position");


        BurnableObject[] objects = FindObjectsByType<BurnableObject>(FindObjectsSortMode.None);
        for(int i = 0; i < objects.Length; i++)
        {
            allBurnableObjects.Add(objects[i]);
        }
        numberOfObjects = allBurnableObjects.Count;
        spawnPlayer = true;
    }

    private void OnEnable()
    {
        
    }

    private void Start()
    {
        timer = burnTime;
        
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
        else
        {
            GameOver();
        }
        percentBurnedText.text = percentBurned.ToString("F0") + "%";

        spawnPlayer = true;
    }

    private void CheckForBurns()
    {
        for(int i = 0; i < allBurnableObjects.Count; i++)
        {
            if (allBurnableObjects[i].CurrentHealth <= 0)
            {
                numberOfBurns++;
                allBurnableObjects.Remove(allBurnableObjects[i]);
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
    }

 

}
