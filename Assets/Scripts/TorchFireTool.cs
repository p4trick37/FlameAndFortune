using UnityEngine;

public class TorchFireTool : MonoBehaviour, IPickupable
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private IgniteProgressUI progressUI;
    [SerializeField] private GameObject litTorchVisual;
    [SerializeField] private Player player;
    private GameObject playerHand;

    [Header("Layers")]
    [SerializeField] private LayerMask interactLayers = ~0;
    [SerializeField] private string hoveredLayerName = "Hovered";

    [Header("Distance")]
    [SerializeField] private float hoverDistance = 6f;
    [SerializeField] private float igniteDistance = 3f;

    [Header("Torch")]
    [SerializeField] private bool torchIsLit = false;
    [SerializeField] private KeyCode relightKey = KeyCode.R;
    [SerializeField] private float relightRadius = 2f;
    [SerializeField] private LayerMask torchIgniteSourceLayers = ~0;

    [Header("Ignition")]
    [SerializeField] private float torchIgniteDamagePerSecond = 20f;

    [Header("Hairspray Boost")]
    [SerializeField] private float hairsprayIgniteMultiplier = 2.5f;
    [SerializeField] private KeyCode fallbackBoostKey = KeyCode.LeftShift;

    [Header("Microphone")]
    [SerializeField] private bool useMicrophone = true;
    [SerializeField] private int sampleWindow = 128;
    [SerializeField] private float micThreshold = 0.02f;

    [Header("Debug")]
    [SerializeField] private bool drawDebugRay = true;
    [SerializeField] private bool logHoverName = false;

    [Header("Inventory Management")]
    public int SlotNumber => slotNumber;
    [SerializeField] private int slotNumber;
    private bool selectingItem;

    private BurnableObject currentHovered;
    private int hoveredLayer = -1;

    private string selectedMicDevice;
    private AudioClip micClip;
    private bool micReady = false;

    public bool TorchIsLit => torchIsLit;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        hoveredLayer = LayerMask.NameToLayer(hoveredLayerName);
        if (hoveredLayer < 0)
        {
            Debug.LogWarning("TorchFireTool: Hovered layer was not found. Create a layer named '" + hoveredLayerName + "'.");
        }

        UpdateTorchVisual();
        SetupMicrophone();
        playerHand = FindHand();
    }

    private void Update()
    {
        UpdateHover();

        if (Input.GetKeyDown(relightKey))
        {
            TryRelightTorch();
        }

        if (Input.GetMouseButton(0))
        {
            TryIgniteHeldTarget();
        }

        UpdateProgressUI();
    }

    private void SetupMicrophone()
    {
        if (!useMicrophone)
        {
            return;
        }

        if (Microphone.devices == null || Microphone.devices.Length == 0)
        {
            micReady = false;
            return;
        }

        selectedMicDevice = Microphone.devices[0];
        micClip = Microphone.Start(selectedMicDevice, true, 1, 44100);
        micReady = true;
    }

    private void UpdateHover()
    {
        BurnableObject newHovered = null;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (drawDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * hoverDistance, Color.white);
        }

        if (Physics.Raycast(ray, out RaycastHit hit, hoverDistance, interactLayers))
        {
            // Debug.Log("RAY HIT: " + hit.collider.name + " | Parent Burnable: " + 
            //           (hit.collider.GetComponentInParent<BurnableObject>() != null 
            //           ? hit.collider.GetComponentInParent<BurnableObject>().name 
            //           : "None"));

            newHovered = hit.collider.GetComponentInParent<BurnableObject>();
        }

        if (newHovered != currentHovered)
        {
            if (currentHovered != null)
            {
                Debug.Log("UNHOVER: " + currentHovered.name + " | Root Layer Before Reset: " + currentHovered.gameObject.layer);
            }

            if (currentHovered != null && hoveredLayer >= 0)
            {
                currentHovered.SetHovered(false, hoveredLayer);
            }

            currentHovered = newHovered;

            if (currentHovered != null && hoveredLayer >= 0)
            {
                currentHovered.SetHovered(true, hoveredLayer);

                //Debug.Log("HOVER: " + currentHovered.name + " | Root Layer After Set: " + currentHovered.gameObject.layer);

                if (logHoverName)
                {
                    Debug.Log("Hovering: " + currentHovered.name);
                }
            }
        }
    }

private void TryIgniteHeldTarget()
{
    if (currentHovered == null)
    {
        Debug.Log("No hovered object.");
        return;
    }

    if (!torchIsLit)
    {
        Debug.Log("Torch is not lit.");
        return;
    }

    if (!currentHovered.CanBeIgnitedByTorch())
    {
        Debug.Log("Target cannot be ignited by torch. Type = " + currentHovered.Type);
        return;
    }

    float distanceToTarget = Vector3.Distance(playerCamera.transform.position, currentHovered.GetUIWorldPosition());
    if (distanceToTarget > igniteDistance)
    {
        Debug.Log("Too far away. Distance = " + distanceToTarget + " | Ignite Distance = " + igniteDistance);
        return;
    }

    float igniteDamage = torchIgniteDamagePerSecond * Time.deltaTime;

    if (IsBoostActive())
    {
        igniteDamage *= hairsprayIgniteMultiplier;
    }

    Debug.Log("Applying ignite damage: " + igniteDamage + " to " + currentHovered.name);
    currentHovered.AddIgniteDamage(igniteDamage);
}

    private bool IsBoostActive()
    {
        bool fallbackHeld = Input.GetKey(fallbackBoostKey);
        bool micBlowing = GetMicVolumeLevel() >= micThreshold;
        return fallbackHeld || micBlowing;
    }

    private float GetMicVolumeLevel()
    {
        if (!useMicrophone || !micReady || micClip == null)
        {
            return 0f;
        }

        int micPosition = Microphone.GetPosition(selectedMicDevice) - sampleWindow + 1;
        if (micPosition < 0)
        {
            return 0f;
        }

        float[] waveData = new float[sampleWindow];
        micClip.GetData(waveData, micPosition);

        float levelMax = 0f;
        for (int i = 0; i < sampleWindow; i++)
        {
            float wavePeak = Mathf.Abs(waveData[i]);
            if (wavePeak > levelMax)
            {
                levelMax = wavePeak;
            }
        }

        return levelMax;
    }

    public void SetTorchLit(bool lit)
    {
        torchIsLit = lit;
        UpdateTorchVisual();
    }

    
    private void TryRelightTorch()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, relightRadius, torchIgniteSourceLayers);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
            {
                continue;
            }

            BurnableObject burnable = hits[i].GetComponentInParent<BurnableObject>();

            if (burnable != null && burnable.IsBurning)
            {
                SetTorchLit(true);
                return;
            }

            // SimpleFireSource fireSource = hits[i].GetComponentInParent<SimpleFireSource>();
            // if (fireSource != null && fireSource.IsActive)
            // {
            //     SetTorchLit(true);
            //     return;
            // }
        }
    }

    private void UpdateTorchVisual()
    {
        if (litTorchVisual != null)
        {
            litTorchVisual.SetActive(torchIsLit);
        }
    }

    private void UpdateProgressUI()
    {
        if (progressUI == null)
        {
            return;
        }

        if (currentHovered == null)
        {
            progressUI.Hide();
            return;
        }

        progressUI.Show(currentHovered, playerCamera, currentHovered.GetHoverColor());

        bool canIgnite = torchIsLit && currentHovered.CanBeIgnitedByTorch();
        bool inRange = Vector3.Distance(playerCamera.transform.position, currentHovered.GetUIWorldPosition()) <= igniteDistance;
        bool isApplyingIgnition = Input.GetMouseButton(0) && canIgnite && inRange;

        progressUI.SetFill(currentHovered.GetIgnitionProgress01(), isApplyingIgnition);
    }

    private void OnDisable()
    {
        if (currentHovered != null && hoveredLayer >= 0)
        {
            currentHovered.SetHovered(false, hoveredLayer);
        }

        if (micReady && !string.IsNullOrEmpty(selectedMicDevice))
        {
            Microphone.End(selectedMicDevice);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = torchIsLit ? Color.yellow : Color.gray;
        Gizmos.DrawWireSphere(transform.position, relightRadius);
    }

    private GameObject FindHand()
    {
        GameObject hand = GameObject.Find("Hand");
        return hand;
    }

    public void SetToHand()
    {
        transform.SetParent(playerHand.transform);
    }



    public void GetSlotNumber(int index)
    {
        slotNumber = index;
    }

    public GameObject GetItem()
    {
        return gameObject;
    }
}