using System.Runtime.CompilerServices;
using UnityEngine;

public class OpenWorldManager : MonoBehaviour
{
    [Header("Player Boundns")]
    [SerializeField] private float xMin;
    [SerializeField] private float xMax;
    [SerializeField] private float zMin;
    [SerializeField] private float zMax;
    [Header("Level Manager")]
    [SerializeField] private SceneSwitcher sceneSwitcher;
    [SerializeField] private GameObject level1Door;
    private bool playerEnterLevel;

    private void Start()
    {
        Player.instance.FindObjectsinScene();
    }

    private void Update()
    {
        if(playerEnterLevel == true)
        {
            sceneSwitcher.SwitchToScene("Level1");
        }
    }
    private void LateUpdate()
    {
        Vector3 playerPos = Player.instance.gameObject.transform.position;
        if(playerPos.x < xMin)
        {
            Player.instance.gameObject.transform.position = new Vector3(xMin, playerPos.y, playerPos.z);
        }
        else if(playerPos.x > xMax)
        {
            Player.instance.gameObject.transform.position = new Vector3(xMax, playerPos.y, playerPos.z);
        }
        else if(playerPos.z < zMin)
        {
            Player.instance.gameObject.transform.position = new Vector3(playerPos.x, playerPos.y, zMin);
        }
        else if(playerPos.z > zMax)
        {
            Player.instance.gameObject.transform.position = new Vector3(playerPos.x, playerPos.y, zMax);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(xMin, 1, zMin), new Vector3(xMax, 1, zMin));
        Gizmos.DrawLine(new Vector3(xMin, 1, zMin), new Vector3(xMin, 1, zMax));
        Gizmos.DrawLine(new Vector3(xMax, 1, zMax), new Vector3(xMin, 1, zMax));
        Gizmos.DrawLine(new Vector3(xMax, 1, zMax), new Vector3(xMax, 1, zMin));
    }

    public GameObject LevelCollider()
    {
        return level1Door;
    }

    public void PlayerEnterLevel()
    {
        playerEnterLevel = true;
    }
}
