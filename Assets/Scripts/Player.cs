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
    public Camera playerCamera;
    [SerializeField] private float walkFOV;
    [SerializeField] private float sprintFOV;
    [Header("Character Controller")]
    [SerializeField] private CharacterController cc;

    public bool freezePlayer = false;

    private bool moveAppliance;
    


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if(freezePlayer == true)
        {

            Movement(false);
            EInteraction(false);
            RightClickInteraction(false);
        }
        else
        {
            Movement(true);
            EInteraction(true);
            RightClickInteraction(true);
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

    private void EInteraction(bool useInteraction)
    {
        if (useInteraction == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit[] hits = Physics.RaycastAll(playerCamera.transform.position, playerCamera.transform.forward, 4);

                foreach (RaycastHit hit in hits)
                {
                    TryForEndCord(hit);
                    TryForPowerInlet(hit);
                }
            }
        }
    }

    private void RightClickInteraction(bool shouldInteract)
    {
        if(shouldInteract == true)
        {
            if(Input.GetMouseButton(1))
            {
                if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 4))
                {
                    moveAppliance = true;
                    TryForMovableAppliance(hit);
                }
            }

            if(Input.GetMouseButtonUp(1))
            {
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 4))
                {
                    moveAppliance = false;
                    TryForMovableAppliance(hit);
                }
            }
        }
    }

    private void TryForEndCord(RaycastHit hit)
    {
        EndCord endCord = hit.transform.GetComponent<EndCord>();
        if(endCord != null)
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
    }

    private void TryForPowerInlet(RaycastHit hit)
    {
        PowerInlet inlet = hit.transform.GetComponent<PowerInlet>();
        if(inlet != null)
        {
            inlet.activated = true;
        }
    }

    private void TryForMovableAppliance(RaycastHit hit)
    {
        MoveAppliance appliance = hit.transform.GetComponent<MoveAppliance>();
        if(appliance != null)
        {
            if (moveAppliance == true)
            {
                appliance.moveObject = true;
                appliance.hitTransform = hit.point;
            }
            else
            {
                appliance.moveObject = false;
            }
        }
        
    }
    
    
}
