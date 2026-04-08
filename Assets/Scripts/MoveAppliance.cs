using NUnit.Framework;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Rendering;

public class MoveAppliance : MonoBehaviour
{
    private Player player;
    [Header("Move other Objects")]
    [SerializeField] private Transform[] moveObjectsWith;
    private Vector3[] startingRelPos;

    [Header("Movement Axis")]
    [SerializeField] private bool zAxis;
    [SerializeField] private bool posDirection;
    [Header("Move Object at Position")]
    public bool moveObject;
    public Vector3 hitTransform;
    [Header("Stopping Distance")]
    [SerializeField] private float stopDistance;
    private float startPosition;
    private float endPosition;
    //private Transform stopTransformLessThan;
    //private Transform stopTransformGreaterThan;
    //[SerializeField] private Transform setStopTransform1;
    //[SerializeField] private Transform setStopTransform2;


    private void Awake()
    {
        //stopTransformLessThan = setStopTransform1;
        //stopTransformGreaterThan = setStopTransform2;
    }

    private void Start()
    {
        startingRelPos = new Vector3[moveObjectsWith.Length];
        for (int i = 0; i < moveObjectsWith.Length; i++)
        {
            startingRelPos[i] = moveObjectsWith[i].position - transform.position;
        }

        player = FindAnyObjectByType<Player>();
        //if(zAxis == false)
        //  {
        //transform.localPosition = new Vector3(stopTransformLessThan.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        // }
        // else
        //  {
        //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, stopTransformLessThan.localPosition.z);
        // }
        if(zAxis == true)
        {
            startPosition = transform.position.z;
            if (posDirection == true)
            {
                endPosition = transform.position.z + stopDistance;
            }
            else
            {
                endPosition = transform.position.z - stopDistance;
            }
        }
        else
        {
            startPosition = transform.position.x;
            if (posDirection == true)
            {
                endPosition = transform.position.x + stopDistance;
            }
            else
            {
                endPosition = transform.position.x - stopDistance;
            }
        }
        

        
    }

    //Get rid of the stop transforms in the fridge child.
    //Set one of the stop transforms as its base position when the game starts
    //Set the other one at a distance that can be set in the inspector.
    //Determine if the object reaches beyond that distance
    //NOTE: I think you should use the global transform. detect x or z movements. 
    //NOTE: Depending rotation, have some sort of parameter crfeated that determines if it should be less than or greater than desired movement. 

    private void Update()
    {
        //Debug.Log(stopTransformLessThan.position);
        //Debug.Log(stopTransformGreaterThan.position);
        if(moveObject == true)
        {
            if(zAxis == true)
            {
                MoveOnZAxis(hitTransform);
            }
            else
            {
                MoveOnXAxis(hitTransform);
            }
            Bounds();
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
    
    private void Bounds()
    {
        if(zAxis == true)
        {
            if(posDirection == true)
            {
                if (transform.position.z < startPosition)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, startPosition);
                }

                if (transform.position.z > endPosition)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, endPosition);
                }
            }
            else
            {
                if (transform.position.z > startPosition)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, startPosition);
                }

                if (transform.position.z < endPosition)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, endPosition);
                }
            }
            
        }
        else
        {
            if(posDirection == true)
            {
                if (transform.position.x < startPosition)
                {
                    transform.position = new Vector3(startPosition, transform.position.y, transform.position.z);
                }

                if (transform.position.x > endPosition)
                {
                    transform.position = new Vector3(endPosition, transform.position.y, transform.position.z);
                }
            }
            else
            {
                if (transform.position.x > startPosition)
                {
                    transform.position = new Vector3(startPosition, transform.position.y, transform.position.z);
                }

                if (transform.position.x < endPosition)
                {
                    transform.position = new Vector3(endPosition, transform.position.y, transform.position.z);
                }
            }
            
        }
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

    private void OnDrawGizmos()
    {
        if(startPosition != 0)
        {
            Gizmos.DrawSphere(new Vector3(transform.localPosition.x, transform.localPosition.y, startPosition), 1);
            Gizmos.DrawSphere(new Vector3(transform.localPosition.x, transform.localPosition.y, endPosition), 1);
        }
    }
}
