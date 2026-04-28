using System.Threading;
using UnityEngine;

public class TorchFireTool : Item
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private IgniteProgressUI progressUI;
    [SerializeField] private GameObject litTorchVisual;

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

    [Header("Health")]
    [SerializeField] private float maxHealth;
    private static float changeMaxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float drainPerSecond;
    private float healthTimer;
    

    private BurnableObject currentHovered;
    private int hoveredLayer = -1;

    private string selectedMicDevice;
    private AudioClip micClip;
    private bool micReady = false;

    public bool TorchIsLit => torchIsLit;
    private int frameCount = 0;

    private void Start()
    {
        maxHealth += changeMaxHealth;
        currentHealth = maxHealth;
        changeMaxHealth = 0;

        healthTimer = 1;
        hoveredLayer = LayerMask.NameToLayer(hoveredLayerName);
        if (hoveredLayer < 0)
        {
            Debug.LogWarning("TorchFireTool: Hovered layer was not found. Create a layer named '" + hoveredLayerName + "'.");
        }

        UpdateTorchVisual();
        SetupMicrophone();

        progressUI = FindProgressBar();
    }

    private void Update()
    {
        if (frameCount < 5)
        {
            PlayerStart();
            frameCount++;
        }
        else
        {
            UpdateHover();
            CheckForSelection(); // From Item
            if (Input.GetKeyDown(relightKey))
            {
                TryRelightTorch();
            }

            if (Input.GetMouseButton(0) && currentlySelecting == true && Player.instance.StopOtherInteractions == false)
            {
                TryIgniteHeldTarget();
            }

            if (currentHealth <= 0)
            {
                torchIsLit = false;
                litTorchVisual.SetActive(false);
            }

            UpdateProgressUI();
        }
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
    BurnableObject hitBurnable = hit.collider.GetComponentInParent<BurnableObject>();

    if (hitBurnable != null && !hitBurnable.IsBurnedOut)
    {
        newHovered = hitBurnable;
    }
}

        if (newHovered != currentHovered)
        {
            if (currentHovered != null)
            {
                //Debug.Log("UNHOVER: " + currentHovered.name + " | Root Layer Before Reset: " + currentHovered.gameObject.layer);
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

        if (currentHovered.IsBurnedOut)
        {
            Debug.Log("Target is already burned out.");
            return;
        }

        // // Optional: block ignition while already burning
        // if (currentHovered.IsBurning)
        // {
        //     Debug.Log("Target is already burning.");
        //     return;
        // }

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
        UpdateHealth();
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

    if (currentHovered == null || currentHovered.IsBurnedOut)
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

    private void UpdateHealth()
    {
        healthTimer -= Time.deltaTime;
        if(healthTimer <= 0)
        {
            currentHealth -= drainPerSecond;
            healthTimer = 1;
            Debug.Log("health drain");
        }
        Debug.Log(healthTimer);
    }

    public static void AddHealth(int amount)
    {
        changeMaxHealth += amount;
    }

    private void PlayerStart()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private IgniteProgressUI FindProgressBar()
    {
        IgniteProgressUI progressbar = FindAnyObjectByType<IgniteProgressUI>();
        return progressbar;
    }
}