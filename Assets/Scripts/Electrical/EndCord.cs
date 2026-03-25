using UnityEngine;

public class EndCord : MonoBehaviour
{
    public bool isPluggedIn;
    [SerializeField] private Transform inPlugTransform;
    [SerializeField] private Transform outPlugTransform;
    [SerializeField] private GameObject burnableObject;
    public bool pluginForFire = false;
    private bool objectOnFire = false;
    
    private void Update()
    {
        if(isPluggedIn == true)
        {
            if(pluginForFire == true && objectOnFire == false)
            {
                BurnableObject burnObject = burnableObject.GetComponent<BurnableObject>();
                //burnObject.isBurning = true;
                Debug.Log("Fridge Is Now On Fire");
                objectOnFire = true;
            }
            transform.localPosition = inPlugTransform.localPosition;
        }
        else
        {
            transform.localPosition = outPlugTransform.localPosition;
        }
    }


}
