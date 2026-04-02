using NUnit.Framework;
using System.Collections.Generic;
using Unity.Jobs;
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
    public Transform stopTransformLessThan;
    public Transform stopTransformGreaterThan;

    

    private void Start()
    {
        startingRelPos = new Vector3[moveObjectsWith.Length];
        for(int i = 0; i < moveObjectsWith.Length; i++)
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
            if(zAxis == false)
            {
                MoveOnXAxis(hitTransform);
                if(transform.localPosition.x < stopTransformLessThan.localPosition.x)
                {
                    transform.localPosition = new Vector3(stopTransformLessThan.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                }

                if(transform.localPosition.x > stopTransformGreaterThan.localPosition.x)
                {
                    transform.localPosition = new Vector3(stopTransformGreaterThan.localPosition.x, transform.localPosition.y, transform.localPosition.z);
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

    public void CurrentlySelecting(RaycastHit hit)
    {
        moveObject = true;
        hitTransform = hit.point;
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
