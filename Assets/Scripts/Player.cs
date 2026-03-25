using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float mouseSens;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravity;

    private float rotation = 0;
    private Vector3 move;
    private float ySpeed;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float walkFOV;
    [SerializeField] private float sprintFOV;
    [Header("Character Controller")]
    [SerializeField] private CharacterController cc;

    public bool freezePlayer = false;
    


    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if(freezePlayer == true)
        {

            Movement(false);
            Interaction(false);
        }
        else
        {
            Movement(true);
            Interaction(true);
        }
    }

    private void Movement(bool shouldMove)
    {
        if(shouldMove == true)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * mouseSens, 0);
            float mouseInput = Input.GetAxis("Mouse Y") * mouseSens;
            rotation -= mouseInput;
            rotation = Mathf.Clamp(rotation, -90, 90);
            playerCamera.transform.localRotation = Quaternion.Euler(new Vector3(rotation, 0, 0));

            move = Vector3.zero;

            move += transform.right * Input.GetAxis("Horizontal") * walkSpeed;
            if(Input.GetKey(KeyCode.LeftShift))
            {
                move += transform.forward * Input.GetAxis("Vertical") * sprintSpeed;
                playerCamera.fieldOfView = sprintFOV;
            }
            else
            {
                move += transform.forward * Input.GetAxis("Vertical") * walkSpeed;
                playerCamera.fieldOfView = walkFOV;

            }

            if(cc.isGrounded == true)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    ySpeed = jumpHeight;
                }
            }
            else
            {
                ySpeed += gravity * Time.deltaTime;
            }
            move += new Vector3(0, ySpeed, 0);

            cc.Move(move * Time.deltaTime);
        }
    }

    private void Interaction(bool useInteraction)
    {
        if (useInteraction == true)
        {


            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit[] hits = Physics.RaycastAll(playerCamera.transform.position, playerCamera.transform.forward, 4);

                foreach (RaycastHit hit in hits)
                {
                    EndCord endCord = hit.transform.GetComponent<EndCord>();
                    PowerInlet inlet = hit.transform.GetComponent<PowerInlet>();
                    if (endCord != null)
                    {
                        if (endCord.isPluggedIn == true)
                        {
                            endCord.isPluggedIn = false;
                        }
                        else
                        {
                            endCord.isPluggedIn = true;
                        }
                    }
                    else if (inlet != null)
                    {
                        inlet.activated = true;
                    }
                }
            }
        }
    }
    private float VectorDistance(Vector3 camera, Vector3 endCord)
    {
        float distance = Mathf.Sqrt((endCord.x - camera.x) * (endCord.x - camera.x) + (endCord.y - camera.y) * (endCord.y - camera.y) + (endCord.z - camera.z) * (endCord.z - camera.z));
        return distance;
    }

    private Vector3 HoldObject(Vector3 position, Vector3 direction, float distance)
    {
        Vector3 objectTransform = position + (direction * distance);
        return objectTransform;

    }
    
}
