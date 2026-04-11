using UnityEngine;

public class PowerInlet : MonoBehaviour
{
    public bool Activated => activated;
    private bool activated = false;
    [SerializeField] private EndCord endCord;

   
    public void Activate()
    {
        activated = true;
    }

    public void Deactivate()
    {
        activated = false;
    }
}
