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
    
    
    private void Awake()
    {
        BurnableObject[] objects = FindObjectsByType<BurnableObject>(FindObjectsSortMode.None);
        for(int i = 0; i < objects.Length; i++)
        {
            allBurnableObjects.Add(objects[i]);
        }
        numberOfObjects = allBurnableObjects.Count;
    }

    private void Start()
    {
        timer = burnTime;
    }

    private void Update()
    {
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
