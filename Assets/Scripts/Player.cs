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


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Movement(true);
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
}
