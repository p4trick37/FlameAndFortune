using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements.Experimental;

public class PowerCord : MonoBehaviour
{
    [Header("Burnable Object")]
    [SerializeField] private BurnableObject burnableObject;

    [Header("Cord General Direction")]
    public bool ZDirection => zDirection;
    public bool PosDirection => posDirection;
    [SerializeField] private bool zDirection;
    [SerializeField] private bool posDirection;

    [Header("Two Points to Draw Cord")]
    [SerializeField] private GameObject ObjectPowerInlet;
    [SerializeField] private GameObject outletInteract;
    [Header("The cubes to connect points")]
    [SerializeField] private GameObject cubePoint1;
    [SerializeField] private GameObject cubeMid;
    [SerializeField] private GameObject cubePoint2;
    [SerializeField] private GameObject cubeVert1;
    [SerializeField] private GameObject cubeVert2;
    [Header("Cube Settings")]
    [SerializeField] private Color cordColor;
    [SerializeField] private float flashTime;
    [Header("EndCord")]
    [SerializeField] private EndCord endCord;
    [Header("PowerInlet")]
    [SerializeField] private PowerInlet powerInlet;
    [Header("Wires Panel")]
    [SerializeField] private GameObject wireCanvas;
    [SerializeField] private WirePanel wirePanel;

    private Player player;

    //Manager vairablers
    private bool wirePanelCompleted = false;
    private bool fireIgnited = false;

    public bool FlashColor => flashColor;
    private bool flashColor = true;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>();
        burnableObject = transform.parent.GetComponentInChildren<BurnableObject>();
        FlashObject();
    }


    //Base size of a cube is 0.1, 0.1, 0.1

    private void Update()
    {
        SetCord();

        if(flashColor == true)
        {
            if ((endCord.IsPluggedIn == true && fireIgnited == false) || (endCord.IsPluggedIn == false && wirePanelCompleted == true))
            {
                MeshRenderer endCordRenderer = endCord.gameObject.GetComponent<MeshRenderer>();
                StoppingCoroutines();
                StartCoroutine(FlashingRenderer(endCordRenderer));
            }

            if (endCord.IsPluggedIn == false && wirePanelCompleted == false)
            {
                MeshRenderer inletRenderer = powerInlet.gameObject.GetComponent<MeshRenderer>();
                StoppingCoroutines();
                StartCoroutine(FlashingRenderer(inletRenderer));
            }
            Debug.Log("Flashed");
            flashColor = false;
        }
        

        if(powerInlet.Activated == true && endCord.IsPluggedIn == false && wirePanel.WiresCompleted == false)
        {
            GoToWirePanel();
            powerInlet.Deactivate();
        }

        if(wirePanel.WiresCompleted == true && wirePanelCompleted == false)
        {
            ExitWirePanel();
            FlashObject();
            wirePanelCompleted = true;
        }

        if(endCord.IsPluggedIn == true && wirePanelCompleted == true && fireIgnited == false)
        {
            IgniteObject();
            fireIgnited = true;
            StoppingCoroutines();
        }
    }

    private void SetCord()
    {
        if(zDirection == true)
        {
            Vector3 midPoint = MidPoint(ObjectPowerInlet.transform.localPosition, outletInteract.transform.localPosition);
            cubePoint1.transform.localPosition = new Vector3(ObjectPowerInlet.transform.localPosition.x, ObjectPowerInlet.transform.localPosition.y, MidPoint(ObjectPowerInlet.transform.localPosition, midPoint).z);
            cubePoint2.transform.localPosition = new Vector3(outletInteract.transform.localPosition.x, outletInteract.transform.localPosition.y, MidPoint(outletInteract.transform.localPosition, midPoint).z);
            float point1Distance = Distance(ObjectPowerInlet.transform.localPosition.z, midPoint.z);
            float point2Distance = Distance(outletInteract.transform.localPosition.z, midPoint.z);
            cubePoint1.transform.localScale = new Vector3(0.05f, 0.05f, point1Distance);
            cubePoint2.transform.localScale = new Vector3(0.05f, 0.05f, point2Distance);

            cubeMid.transform.localPosition = midPoint;
            float xDistance = Distance(ObjectPowerInlet.transform.localPosition.x, outletInteract.transform.localPosition.x);
            cubeMid.transform.localScale = new Vector3(xDistance, 0.05f, 0.05f);

            cubeVert1.transform.localPosition = new Vector3(cubePoint1.transform.localPosition.x, MidPoint(cubePoint1.transform.localPosition, midPoint).y, midPoint.z);
            cubeVert2.transform.localPosition = new Vector3(cubePoint2.transform.localPosition.x, MidPoint(cubePoint2.transform.localPosition, midPoint).y, midPoint.z);
            float yDistance1 = Distance(ObjectPowerInlet.transform.localPosition.y, midPoint.y);
            float yDistance2 = Distance(outletInteract.transform.localPosition.y, midPoint.y);
            cubeVert1.transform.localScale = new Vector3(0.05f, yDistance1, 0.05f);
            cubeVert2.transform.localScale = new Vector3(0.05f, yDistance2, 0.05f); 
        }
        else 
        {
            Vector3 midPoint = MidPoint(ObjectPowerInlet.transform.localPosition, outletInteract.transform.localPosition);
            cubePoint1.transform.localPosition = new Vector3(MidPoint(ObjectPowerInlet.transform.localPosition, midPoint).x, ObjectPowerInlet.transform.localPosition.y, ObjectPowerInlet.transform.localPosition.z);
            cubePoint2.transform.localPosition = new Vector3(MidPoint(outletInteract.transform.localPosition, midPoint).x, outletInteract.transform.localPosition.y, outletInteract.transform.localPosition.z);
            float point1Distance = Distance(ObjectPowerInlet.transform.localPosition.x, midPoint.x);
            float point2Distance = Distance(outletInteract.transform.localPosition.x, midPoint.x);
            cubePoint1.transform.localScale = new Vector3(point1Distance, 0.05f, 0.05f);
            cubePoint2.transform.localScale = new Vector3(point2Distance, 0.05f, 0.05f);

            cubeMid.transform.localPosition = midPoint;
            float zDistance = Distance(ObjectPowerInlet.transform.localPosition.z, outletInteract.transform.localPosition.z);
            cubeMid.transform.localScale = new Vector3(0.05f, 0.05f, zDistance);

            cubeVert1.transform.localPosition = new Vector3(midPoint.x, MidPoint(cubePoint1.transform.localPosition, midPoint).y, cubePoint1.transform.localPosition.z);
            cubeVert2.transform.localPosition = new Vector3(midPoint.x, MidPoint(cubePoint2.transform.localPosition, midPoint).y, cubePoint2.transform.localPosition.z);
            float yDistance1 = Distance(ObjectPowerInlet.transform.localPosition.y, midPoint.y);
            float yDistance2 = Distance(outletInteract.transform.localPosition.y, midPoint.y);
            cubeVert1.transform.localScale = new Vector3(0.05f, yDistance1, 0.05f);
            cubeVert2.transform.localScale = new Vector3(0.05f, yDistance2, 0.05f);
        }
        
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


   
    private IEnumerator FlashingRenderer(MeshRenderer renderer)
    {
        while(true)
        {
            renderer.material.color = Color.white;
            yield return new WaitForSeconds(flashTime);
            renderer.material.color = Color.black;
            yield return new WaitForSeconds(flashTime);
        }
    }

    public void GoToWirePanel()
    {
        wireCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        player.freezePlayer = true;
    }

    private void IgniteObject()
    {
        burnableObject.Ignite();
    }

    public void ExitWirePanel()
    {
        wireCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        player.freezePlayer = false;

    }

    private void StoppingCoroutines()
    {
        StopAllCoroutines();
        endCord.gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
        powerInlet.gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
    }

    public void FlashObject()
    {
        flashColor = true;
    }

}
