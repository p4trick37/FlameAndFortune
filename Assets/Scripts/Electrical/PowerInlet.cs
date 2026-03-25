using UnityEngine;

public class PowerInlet : MonoBehaviour
{
    public GameObject wirePanel;
    public bool activated = false;
    [SerializeField] private EndCord endCord;
    private Player player;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>();
    }

    private void Update()
    {
        if(activated == true && endCord.isPluggedIn == false)
        {
            wirePanel.SetActive(true);
            player.freezePlayer = true;
        }
        activated = false;
    }
}
