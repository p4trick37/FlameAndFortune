using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float closeRotation;
    [SerializeField] private float openRotation;
    public bool InOpenState => inOpenState;
    private bool inOpenState = false;
    public void OpenObject()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, openRotation, transform.rotation.z);
        inOpenState = true;
    }

    public void CloseObject()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, closeRotation, transform.rotation.z);
        inOpenState = false;
    }
}
