using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player instance;
    [Header("Player Movement")]
    [SerializeField] private float mouseSens;
    public float DefaultWalkSpeed => defaultWalkSpeed;
    public float DefaultSprintSpeed => defaultSprintSpeed;
    [SerializeField] private float defaultWalkSpeed;
    [SerializeField] private float defaultSprintSpeed;
    public float WalkSpeed => walkSpeed;
    public float SprintSpeed => sprintSpeed;
    private float walkSpeed;
    private float sprintSpeed;
    private bool shouldSprint;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravity;

    private float rotation = 0;
    private Vector3 move;
    private float ySpeed;
    private bool currentlyMoving;

    [Header("Camera")]
    public Camera playerCamera;
    [SerializeField] private float walkFOV;
    [SerializeField] private float sprintFOV;
    [Header("Character Controller")]
    [SerializeField] private CharacterController cc;
    [Header("Inventory")]
    [SerializeField] private int inventorySize;
    [SerializeField] private GameObject[] inventory;
    public int CurrentSlot => currentSlot;
    [SerializeField] private int currentSlot;

    [Header("Open World Manager")]
    [SerializeField] private OpenWorldManager openWorldManager;

    public bool freezePlayer = false;

    private bool moveAppliance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SetMovementSpeed(defaultWalkSpeed, defaultSprintSpeed);
        LockSprint(false);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inventory = SetInventory(inventorySize);
        currentSlot = 1;
    }

    void Update()
    {
        if (freezePlayer == true)
        {

            Movement(false);
            //EInteraction(false);
            LeftClickInteractioni(false);
            RightClickInteraction(false);
        }
        else
        {
            Movement(true);
            //EInteraction(true);
            LeftClickInteractioni(true);
            RightClickInteraction(true);
        }
        InventorySelect();
    }

    private void Movement(bool shouldMove)
    {
        if (shouldMove == true)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * mouseSens, 0);
            float mouseInput = Input.GetAxis("Mouse Y") * mouseSens;
            rotation -= mouseInput;
            rotation = Mathf.Clamp(rotation, -90, 90);
            playerCamera.transform.localRotation = Quaternion.Euler(new Vector3(rotation, 0, 0));

            move = Vector3.zero;

            move += transform.right * Input.GetAxis("Horizontal") * walkSpeed;

            if (Input.GetKey(KeyCode.LeftShift) && shouldSprint == true && currentlyMoving == true)
            {
                move += transform.forward * Input.GetAxis("Vertical") * sprintSpeed;
                playerCamera.fieldOfView = sprintFOV;
            }
            else
            {
                move += transform.forward * Input.GetAxis("Vertical") * walkSpeed;
                playerCamera.fieldOfView = walkFOV;

            }

            if(move.x != 0 || move.z != 0)
            {
                currentlyMoving = true;
            }
            else
            {
                currentlyMoving = false;
            }


            if (cc.isGrounded == true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
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

    public void SetMovementSpeed(float speed)
    {
        walkSpeed = speed;
    }

    public void SetMovementSpeed(float changeWalk, float changeSprint)
    {
        walkSpeed = changeWalk;
        sprintSpeed = changeSprint;
    }

    public void LockSprint(bool shouldLock)
    {
        if(shouldLock == true)
        {
            shouldSprint = false;
        }
        else
        {
            shouldSprint = true;
        }
    }

    private void EInteraction(bool useInteraction)
    {
        if (useInteraction == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //RaycastHit[] hits = Physics.RaycastAll(playerCamera.transform.position, playerCamera.transform.forward, 4);

                //foreach (RaycastHit hit in hits)
                //{
                //    TryForEndCord(hit);
                //    TryForPowerInlet(hit);
                //    TryForObjects(hit);
                //}

                if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 4))
                {
                    TryForEndCord(hit);
                    TryForPowerInlet(hit);
                    TryForObjects(hit);
                }
            }
        }
    }
    private void LeftClickInteractioni(bool shouldInteract)
    {
        if (shouldInteract == true)
        {
            if(Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 4))
                {
                    TryForEndCord(hit);
                    TryForPowerInlet(hit);
                    TryForObjects(hit);
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 4))
                {
                    moveAppliance = true;
                    
                    TryForMovableAppliance(hit);
                    TryForRotationObject(hit);
                }
            }


            if (Input.GetMouseButtonUp(0))
            {
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 4))
                {
                    moveAppliance = false;
                    //TryForMovableAppliance(hit);
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
                DropCurrentItem();
            }
        }
    }

    private void TryForEndCord(RaycastHit hit)
    {
        EndCord endCord = hit.transform.GetComponent<EndCord>();
        if(endCord != null)
        {
            if (endCord.IsPluggedIn == true)
            {
                endCord.UnPlug();
            }
            else
            {
                endCord.PlugIn();
            }
        }
    }

    private void TryForPowerInlet(RaycastHit hit)
    {
        PowerInlet inlet = hit.transform.GetComponent<PowerInlet>();
        if(inlet != null)
        {
            inlet.Activate();
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

    private void TryForObjects(RaycastHit hit)
    {
        if(hit.transform.TryGetComponent<IPickupable>(out IPickupable item))
        {
            Debug.Log("Ran");
            bool emptySlot = false;
            int emptyIndex = 0;
            for(int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] == null)
                {
                    emptySlot = true;
                    emptyIndex = i;
                    break;
                }
            }
            if(emptySlot == true)
            {
                item.SetToHand();
                inventory[emptyIndex] = item.OnPickup();
                item.GetSlotNumber(emptyIndex + 1);
                item.ChangeRigidbodyState();
            }
        }
    }

    private void TryForRotationObject(RaycastHit hit)
    {
        RotateObject rotateObject = hit.transform.gameObject.GetComponent<RotateObject>();
        if(rotateObject != null)
        {
            if(rotateObject.InOpenState == true)
            {
                rotateObject.CloseObject();
            }
            else
            {
                rotateObject.OpenObject();
            }
        }
    }


    private GameObject[] SetInventory(int size)
    {
        GameObject[] gameObjects = new GameObject[size];
        return gameObjects;
    }

    private int NumKeyboardPressedReturn()
    {
        int select = currentSlot;
        for (int i = 1; i <= inventory.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                select = i;
            }
        }
        return select;
    }



    private void InventorySelect()
    {
        currentSlot = NumKeyboardPressedReturn();
        
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null)
            {
                if (i == currentSlot - 1)
                {
                    inventory[i].SetActive(true);
                }
                else
                {
                    inventory[i].SetActive(false);
                }
            }
        }
    }

    private void DropCurrentItem()
    {
        if(inventory[currentSlot - 1] != null)
        {
            GameObject dropItem = inventory[currentSlot - 1];
            dropItem.transform.SetParent(null);
            if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 1f))
            {
                dropItem.transform.position = hit.point;
            }
            dropItem.GetComponent<Item>().ChangeRigidbodyState();
            inventory[currentSlot - 1] = null;
            
        }
    }
    
    public void DestroyCertainItem(GameObject item)
    {
        for(int i = 0; i < inventory.Length; i++)
        {
            if(inventory[i] == item)
            {
                inventory[i] = null;
                Destroy(item);
            }
        }
    }

    public void ClearInventory()
    {
        for(int i = 0; i < inventory.Length; i++)
        {
            Destroy(inventory[i]);
            inventory[i] = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(openWorldManager != null)
        {
            if(other.gameObject == openWorldManager.LevelCollider())
            {
                Debug.Log("WE HIT DOOR");
                openWorldManager.PlayerEnterLevel();
            }
        }
        Debug.Log("Something happe ned");
        
    }

    public void FindObjectsinScene()
    {
        openWorldManager = FindAnyObjectByType<OpenWorldManager>();
    }


}
