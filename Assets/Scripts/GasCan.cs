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
    [SerializeField] private float pourRange;
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
            //RaycastHit[] hits = Physics.RaycastAll(Player.instance.playerCamera.transform.position, Player.instance.playerCamera.transform.forward, 3);
            //Vector3 position = Player.instance.playerCamera.transform.position + Player.instance.playerCamera.transform.forward * 3;
            //foreach (RaycastHit hit in hits)
            //{
            //    if(Physics.Raycast(hit.point, Vector3.down, out RaycastHit hit2))
            //    {
            //        if (hit2.collider.CompareTag("Gas"))
            //        {
            //            break;
            //        }

            //        if (hit2.collider.gameObject.GetComponent<Player>())
            //        {
            //            continue;
            //        }

            //        if (hit2.collider == canCollider)
            //        {
            //            continue;
            //        }

            //        if (hit2.transform.gameObject.GetComponent<BurnableObject>())
            //        {
            //            BurnableObject obj = hit2.transform.gameObject.GetComponent<BurnableObject>();
            //            obj.ApplyGas();
            //        }

            //        Instantiate(gasSprite, new Vector3(hit.point.x, hit.point.y + 0.001f, hit.point.z), Quaternion.identity);
            //        health--;
            //        Debug.Log("Placed something because the player did hit something in initial raycast");
            //        break;
            //    }      
            //}

            //if (hits.Length == 0)
            //{
            //    if (Physics.Raycast(position, Vector3.down, out RaycastHit hitInfo))
            //    {
            //        if (hitInfo.collider.CompareTag("Gas"))
            //        {
            //            break;
            //        }

            //        if (hitInfo.collider.gameObject.GetComponent<Player>())
            //        {
            //            continue;
            //        }

            //        if (hitInfo.collider == canCollider)
            //        {
            //            continue;
            //        }

            //        if (hitInfo.transform.gameObject.GetComponent<BurnableObject>())
            //        {
            //            BurnableObject obj = hitInfo.transform.gameObject.GetComponent<BurnableObject>();
            //            obj.ApplyGas();
            //        }

            //        Instantiate(gasSprite, new Vector3(hitInfo.point.x, hitInfo.point.y + 0.001f, hitInfo.point.z), Quaternion.identity);
            //        health--;
            //        Debug.Log("Placed something because of the fact the initail raycast didn't hit anything");
            //    }
            //    Debug.Log("Didn't hit anything");
            //}

            //yield return new WaitForSeconds(spawnTimer);
            //Debug.Log("Next sequence");

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
