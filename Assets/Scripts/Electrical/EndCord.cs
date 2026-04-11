using UnityEngine;

public class EndCord : MonoBehaviour
{
    public bool IsPluggedIn => isPluggedIn;
    private bool isPluggedIn;

    [SerializeField] private Transform inPlugTransform;
    [SerializeField] private Transform outPlugTransform;
    [SerializeField] private GameObject burnableObject;
    private PowerCord powerCord;



    private void Start()
    {
        isPluggedIn = true;
        powerCord = GetComponentInParent<PowerCord>();
    }
    private void Update()
    {
        if(isPluggedIn == true)
        {
            transform.localPosition = MovedPlugInTransform(inPlugTransform);
        }
        else
        {
            transform.localPosition = outPlugTransform.localPosition;
        }
    }

    public void PlugIn()
    {
        isPluggedIn = true;
        powerCord.FlashObject();
    }

    public void UnPlug()
    {
        isPluggedIn = false;
        powerCord.FlashObject();
    }

    public Vector3 MovedPlugInTransform(Transform target)
    {
        Vector3 position = Vector3.zero;
        if (powerCord.ZDirection == true)
        {
            if (powerCord.PosDirection == true)
            {
                position = new Vector3(target.localPosition.x, target.localPosition.y, target.localPosition.z + (target.localScale.z / 2));
                Debug.Log("TargetPos: " + target.localPosition + ". . . LocalScale: " + (target.localScale.z / 2) + "Position:" + position);
            }
            else
            {
                position = new Vector3(target.localPosition.x, target.localPosition.y, target.localPosition.z - (target.localScale.z / 2));
            }
        }
        else
        {
            if (powerCord.PosDirection == true)
            {
                position = new Vector3(target.localPosition.x + (target.localScale.x / 2), target.localPosition.y, target.localPosition.z);
            }
            else
            {
                position = new Vector3(target.localPosition.x - (target.localScale.x / 2), target.localPosition.y, target.localPosition.z);
            }
        }
        return position;

    }


}
