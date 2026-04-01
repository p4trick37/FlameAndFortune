using UnityEngine;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    [SerializeField] private List<BurnableObject> allBurnableObjects = new List<BurnableObject>();
    [SerializeField] private int numberOfBurns;
    [SerializeField] private int numberOfObjects;
    [SerializeField] private float percentBurned;
    private void Awake()
    {
        BurnableObject[] objects = FindObjectsByType<BurnableObject>(FindObjectsSortMode.None);
        for(int i = 0; i < objects.Length; i++)
        {
            allBurnableObjects.Add(objects[i]);
        }
        numberOfObjects = allBurnableObjects.Count;
    }

    private void Update()
    {
        CheckForBurns();
        percentBurned = PercentBurned(numberOfBurns, numberOfObjects);
        if(percentBurned > 0 )
        {
            Debug.Log("Object can be burned");
        }
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
}
