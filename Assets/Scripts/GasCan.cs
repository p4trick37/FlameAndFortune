using System.Collections;
using UnityEngine;

public class GasCan : Item
{
    [SerializeField] private bool usingGasCan;
    [SerializeField] private GameObject gasSprite;
    [SerializeField] private float slowMoveSpeed;
    [SerializeField] private BoxCollider canCollider;
    [SerializeField] private float spawnTimer;
    [SerializeField] private float health;
    private bool changeSpeed;

    private void Awake()
    {
        canCollider = GetComponent<BoxCollider>();
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

    }


    private IEnumerator UseCan()
    {
        while (health > 0)
        {
            RaycastHit[] hits = Physics.RaycastAll(Player.instance.transform.position, Vector3.down);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Gas"))
                {
                    break;
                }

                if (hit.collider.gameObject.GetComponent<Player>())
                {
                    continue;
                }

                if (hit.collider == canCollider)
                {
                    continue;
                }

                if(hit.transform.gameObject.GetComponent<BurnableObject>())
                {
                    BurnableObject obj = hit.transform.gameObject.GetComponent<BurnableObject>();
                    obj.ApplyGas();
                }


                Instantiate(gasSprite, new Vector3(hit.point.x, hit.point.y + 0.001f, hit.point.z), Quaternion.identity);
                health--;
                break; 
            }
            yield return new WaitForSeconds(spawnTimer);
        }
    }

    private void UserInput()
    {
        if (Input.GetMouseButtonDown(0) && currentlySelecting == true)
        {
            changeSpeed = true;
            Debug.Log("Pressed");
        }

        if (Input.GetMouseButton(0) && currentlySelecting == true)
        {
            usingGasCan = true;
        }

        if (Input.GetMouseButtonUp(0) && currentlySelecting == true)
        {
            usingGasCan = false;
            changeSpeed = true;
            StopAllCoroutines();
        }
    }
}
