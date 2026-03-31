using NUnit.Framework;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class MoveAppliance : MonoBehaviour
{
    private Player player;
    [Header("Keep Track of Inlet Position")]
    [SerializeField] private Transform cordInlet;
    [SerializeField] private Transform inletOnAppliance;
    [Header("Movement Axis")]
    [SerializeField] private bool xAxis;
    [Header("Move Object at Position")]
    public bool moveObject;
    public Vector3 hitTransform;
    [Header("Stopped Transform")]
    public Transform stopTransformLessThan;
    public Transform stopTransformGreaterThan;
    public bool OutOfbounds => outOfBounds;
    private bool outOfBounds;
    

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        if(xAxis == true)
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
            if(xAxis == true)
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



        cordInlet.transform.position = inletOnAppliance.position;
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

}
