using UnityEngine;
using UnityEngine.Experimental.AI;

public class PowerCord : MonoBehaviour
{
    [Header("Cord General Direction")]
    [SerializeField] private bool zDirection;

    [Header("Two Points to Draw Cord")]
    [SerializeField] private GameObject ObjectPowerInlet;
    [SerializeField] private GameObject WallOutletInteractable;
    [Header("The cubes to connect points")]
    [SerializeField] private GameObject cubePoint1;
    [SerializeField] private GameObject cubeMid;
    [SerializeField] private GameObject cubePoint2;
    [SerializeField] private GameObject cubeVert1;
    [SerializeField] private GameObject cubeVert2;
    [Header("Cube Settings")]
    [SerializeField] private Color cordColor;

    //Base size of a cube is 0.1, 0.1, 0.1

    private void Update()
    {
        //CreateCordTest();
        SetCord();
    }

    private void SetCord()
    {
        if(zDirection == true)
        {
            Vector3 midPoint = MidPoint(ObjectPowerInlet.transform.localPosition, WallOutletInteractable.transform.localPosition);
            cubePoint1.transform.localPosition = new Vector3(ObjectPowerInlet.transform.localPosition.x, ObjectPowerInlet.transform.localPosition.y, MidPoint(ObjectPowerInlet.transform.localPosition, midPoint).z);
            cubePoint2.transform.localPosition = new Vector3(WallOutletInteractable.transform.localPosition.x, WallOutletInteractable.transform.localPosition.y, MidPoint(WallOutletInteractable.transform.localPosition, midPoint).z);
            float point1Distance = Distance(ObjectPowerInlet.transform.localPosition.z, midPoint.z);
            float point2Distance = Distance(WallOutletInteractable.transform.localPosition.z, midPoint.z);
            cubePoint1.transform.localScale = new Vector3(0.05f, 0.05f, point1Distance);
            cubePoint2.transform.localScale = new Vector3(0.05f, 0.05f, point2Distance);

            cubeMid.transform.localPosition = midPoint;
            float xDistance = Distance(ObjectPowerInlet.transform.localPosition.x, WallOutletInteractable.transform.localPosition.x);
            cubeMid.transform.localScale = new Vector3(xDistance, 0.05f, 0.05f);

            cubeVert1.transform.localPosition = new Vector3(cubePoint1.transform.localPosition.x, MidPoint(cubePoint1.transform.localPosition, midPoint).y, midPoint.z);
            cubeVert2.transform.localPosition = new Vector3(cubePoint2.transform.localPosition.x, MidPoint(cubePoint2.transform.localPosition, midPoint).y, midPoint.z);
            float yDistance1 = Distance(ObjectPowerInlet.transform.localPosition.y, midPoint.y);
            float yDistance2 = Distance(WallOutletInteractable.transform.localPosition.y, midPoint.y);
            cubeVert1.transform.localScale = new Vector3(0.05f, yDistance1, 0.05f);
            cubeVert2.transform.localScale = new Vector3(0.05f, yDistance2, 0.05f); 
        }
        else 
        {
            Vector3 midPoint = MidPoint(ObjectPowerInlet.transform.localPosition, WallOutletInteractable.transform.localPosition);
            cubePoint1.transform.localPosition = new Vector3(MidPoint(ObjectPowerInlet.transform.localPosition, midPoint).x, ObjectPowerInlet.transform.localPosition.y, ObjectPowerInlet.transform.localPosition.z);
            cubePoint2.transform.localPosition = new Vector3(MidPoint(WallOutletInteractable.transform.localPosition, midPoint).x, WallOutletInteractable.transform.localPosition.y, WallOutletInteractable.transform.localPosition.z);
            float point1Distance = Distance(ObjectPowerInlet.transform.localPosition.x, midPoint.x);
            float point2Distance = Distance(WallOutletInteractable.transform.localPosition.x, midPoint.x);
            cubePoint1.transform.localScale = new Vector3(point1Distance, 0.05f, 0.05f);
            cubePoint2.transform.localScale = new Vector3(point2Distance, 0.05f, 0.05f);

            cubeMid.transform.localPosition = midPoint;
            float zDistance = Distance(ObjectPowerInlet.transform.localPosition.z, WallOutletInteractable.transform.localPosition.z);
            cubeMid.transform.localScale = new Vector3(0.05f, 0.05f, zDistance);

            cubeVert1.transform.localPosition = new Vector3(midPoint.x, MidPoint(cubePoint1.transform.localPosition, midPoint).y, cubePoint1.transform.localPosition.z);
            cubeVert2.transform.localPosition = new Vector3(midPoint.x, MidPoint(cubePoint2.transform.localPosition, midPoint).y, cubePoint2.transform.localPosition.z);
            float yDistance1 = Distance(ObjectPowerInlet.transform.localPosition.y, midPoint.y);
            float yDistance2 = Distance(WallOutletInteractable.transform.localPosition.y, midPoint.y);
            cubeVert1.transform.localScale = new Vector3(0.05f, yDistance1, 0.05f);
            cubeVert2.transform.localScale = new Vector3(0.05f, yDistance2, 0.05f);
        }
        
    }

    //Works if the outlets are on the same Vector. 
    private void CreateCordTest()
    {
        Vector3 midPoint = MidPoint(ObjectPowerInlet.transform.localPosition, WallOutletInteractable.transform.localPosition);
        cubePoint1.transform.localPosition = midPoint;
        cubePoint1.transform.localScale = new Vector3(0.05f, 0.05f, Distance(WallOutletInteractable.transform.localPosition.z, WallOutletInteractable.transform.localPosition.z));
    }

    private Vector3 MidPoint(Vector3 point1, Vector3 point2)
    {
        Vector3 midPoint = (point1 + point2) / 2;
        return midPoint;
    }

    private float Distance(float x1, float x2)
    {
        float distance = x2 - x1;
        if(distance < 0)
                {
            distance = Mathf.Abs(distance);
        }
        return distance;
    }

    

    private GameObject SetCube(GameObject cube)
    {
        GameObject setCube = cube;
        setCube.GetComponent<MeshRenderer>().material.color = cordColor;

        return setCube;
    }  

}
