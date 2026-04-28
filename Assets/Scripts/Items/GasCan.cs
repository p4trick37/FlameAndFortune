using System.Collections;
using UnityEngine;

public class GasCan : Item
{
    [SerializeField] private bool usingGasCan;
    [SerializeField] private GameObject gasSprite;
    [SerializeField] private float slowMoveSpeed;
    [SerializeField] private BoxCollider canCollider;
    [SerializeField] private float spawnTimer;
    private static float addHealth;
    [SerializeField] private float health;
    [SerializeField] private float pourRange;
    private bool changeSpeed;

    private void Awake()
    {
        canCollider = GetComponent<BoxCollider>();
        health += addHealth;
        addHealth = 0;

    }

    private void OnDisable()
    {
        Player.instance.SetMovementSpeed(Player.instance.WalkSpeed);
    }

    private void Update()
    {
        CheckForSelection();
        UserInput();

        if (usingGasCan == true && health > 0)
        {
            if (changeSpeed == true)
            {
                Player.instance.SetMovementSpeed(slowMoveSpeed);
                Player.instance.LockSprint(true);
                changeSpeed = false;
                StartCoroutine(UseCan());
            }
        }
        else
        {
            if (changeSpeed == true)
            {
                Player.instance.SetMovementSpeed(Player.instance.DefaultWalkSpeed);
                Player.instance.LockSprint(false);
                changeSpeed = false;
            }
        }
        Debug.Log("Stopping interacfg " + Player.instance.StopOtherInteractions);
        Debug.Log("currently selc " + currentlySelecting);

    }


    private IEnumerator UseCan()
    {
        
        while (health > 0)
        {
            Vector3 targetPoint = Vector3.zero;
            if(Physics.Raycast(Player.instance.playerCamera.transform.position, Player.instance.playerCamera.transform.forward, out RaycastHit hit, pourRange))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = Player.instance.playerCamera.transform.position + Player.instance.playerCamera.transform.forward * pourRange;
            }

            if(Physics.Raycast(targetPoint, Vector3.down, out RaycastHit hitInfo))
            {
                bool invalidHit = false;
                if(hitInfo.collider.CompareTag("Gas"))
                {
                    invalidHit = true;
                }

                if(hitInfo.collider.gameObject.GetComponent<Player>() != null)
                {
                    invalidHit = true;
                }

                if(invalidHit == false)
                {
                    Instantiate(gasSprite, new Vector3(hitInfo.point.x, hitInfo.point.y + 0.001f, hitInfo.point.z), Quaternion.identity);
                    health--;
                }
            }
            yield return new WaitForSeconds(spawnTimer);
        }
    }

    private void UserInput()
    {
        if (Input.GetMouseButtonDown(0) && currentlySelecting == true && Player.instance.StopOtherInteractions == false)
        {
            changeSpeed = true;
        }

        if (Input.GetMouseButton(0) && currentlySelecting == true && Player.instance.StopOtherInteractions == false) 
        {
            usingGasCan = true;
        }

        if (Input.GetMouseButtonUp(0) && currentlySelecting == true && Player.instance.StopOtherInteractions == false)
        {
            usingGasCan = false;
            changeSpeed = true;
            StopAllCoroutines();
        }
    }

    public static void AddHealth(int amount)
    {
        addHealth += amount;
    }

  
}
