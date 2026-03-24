using UnityEngine;

public class EndCord : MonoBehaviour
{
    public bool isPluggedIn;
    [SerializeField] private Transform inPlugTransform;
    [SerializeField] private Transform outPlugTransform;

    private void Update()
    {
        if(isPluggedIn == true)
        {
            transform.localPosition = inPlugTransform.localPosition;
        }
        else
        {
            transform.localPosition = outPlugTransform.localPosition;
        }
    }


}
