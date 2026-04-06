using NUnit.Framework;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class MoveAppliance : MonoBehaviour
{
    private Player player;
    [Header("Move other Objects")]
    [SerializeField] private Transform[] moveObjectsWith;
    private Vector3[] startingRelPos;

    [Header("Movement Axis")]
    [SerializeField] private bool zAxis;
    [Header("Move Object at Position")]
    public bool moveObject;
    public Vector3 hitTransform;
    [Header("Stopped Transform")]
    private Transform stopTransformLessThan;
    private Transform stopTransformGreaterThan;
    [SerializeField] private Transform setStopTransform1;
    [SerializeField] private Transform setStopTransform2;

    private void Awake()
    {
        stopTransformLessThan = setStopTransform1;
        stopTransformGreaterThan = setStopTransform2;
    }

    private void Start()
    {
        startingRelPos = new Vector3[moveObjectsWith.Length];
        for (int i = 0; i < moveObjectsWith.Length; i++)
        {
            startingRelPos[i] = moveObjectsWith[i].position - transform.position;
        }

        player = FindAnyObjectByType<Player>();
        if(zAxis == false)
        {
            transform.localPosition = new Vector3(stopTransformLessThan.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, stopTransformLessThan.localPosition.z);
        }
    }

    private void Update()
    {
        if(moveObject == true)
        {
            ///*
            if(zAxis == false)
            {
                MoveOnXAxis(hitTransform);
                Debug.Log("Moving");
                if (transform.position.z < stopTransformLessThan.position.z)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, stopTransformLessThan.position.z);
                    Debug.Log("Position is bugged");
                }

                if(transform.position.z > stopTransformGreaterThan.position.z)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, stopTransformGreaterThan.position.z);
                    Debug.Log("Next Position is bugged");
                }
            }
            else
            {
                MoveOnZAxis(hitTransform);
                
                if(transform.localPosition.z < stopTransformLessThan.localPosition.z)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, stopTransformLessThan.localPosition.z);
                    
                }

                if(transform.localPosition.z > stopTransformGreaterThan.localPosition.z)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, stopTransformGreaterThan.localPosition.z);
                    
                }
            }
            //*/
            //MoveOnForwardVector(hitTransform);
            
        }

        for(int i = 0; i < moveObjectsWith.Length; i++)
        {
            moveObjectsWith[i].position = transform.position + startingRelPos[i];
        }
    }

    private void MoveOnXAxis(Vector3 hitTransform)
    {
        transform.position = new Vector3(hitTransform.x, transform.position.y, transform.position.z);
    }

    private void MoveOnZAxis(Vector3 hitTransform)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, hitTransform.z);
    }

    private void MoveOnForwardVector(Vector3 hitTransform)
    {
        Vector3 forwardVector = transform.forward.normalized;
        Debug.Log(forwardVector);
        transform.position = hitTransform.z * forwardVector;
    }

    public void CurrentlySelecting(RaycastHit hit)
    {
        moveObject = true;
        hitTransform = hit.transform.position;
    }

    public void ExitSelecting()
    {
        moveObject = false;
    }

    private void MoveOtherObjects(Vector3 hitTransform)
    {
        foreach(Transform objPos in moveObjectsWith)
        {
            Vector3 distance = objPos.position - transform.position;
            objPos.position = transform.position + distance;
        }
    }

}
