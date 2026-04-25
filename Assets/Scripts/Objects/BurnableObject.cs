using System.Collections;
using UnityEngine;

public class BurnableObject : MonoBehaviour
{
    public enum FireType
    {
        Combustible,   // red
        Electrical,    // blue
        Chemical       // green
    }

    [Header("Type")]
    [SerializeField] private FireType fireType = FireType.Combustible;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    [Tooltip("How much of max health must be lost before the object starts burning on its own. 0.10 = 10%")]
    [Range(0.01f, 0.95f)]
    [SerializeField] private float igniteDamagePercentToStartBurning = 0.10f;

    [Header("Burning")]
    [SerializeField] private float burnDamagePerSecond = 10f;
    [SerializeField] private bool isBurning = false;
    [SerializeField] private bool isBurnedOut = false;

    [Header("Gas")]
    [SerializeField] private bool isGasSoaked = false;
    [SerializeField] private float gasIgniteMultiplier = 2f;
    [SerializeField] private float gasBurnMultiplier = 2f;

    [Header("Spread")]
    [SerializeField] private bool canSpreadFire = true;
    [SerializeField] private float spreadRadius = .3f;
    [SerializeField] private float spreadInterval = 0.5f;
    [SerializeField] private float spreadIgniteDamagePerSecond = 3f;
    [SerializeField] private LayerMask spreadLayers = ~0;

    [Header("Visual Burn Darkening")]
    [SerializeField] private Renderer[] targetRenderers;
    [SerializeField] private Color burnedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    [SerializeField] private bool affectEmission = false;

    private MaterialPropertyBlock hoverPropertyBlock;
private static readonly int OutlineColorID = Shader.PropertyToID("_OutlineColor");

    [Header("Particles")]
    [SerializeField] private GameObject fireParticlePrefab;
    [SerializeField] private Transform particleSpawnPoint;
    [SerializeField] private Vector3 particleOffset = Vector3.zero;
    [SerializeField] private MeshFilter TargetMeshFilter;

    [Header("UI Anchor")]
    [SerializeField] private Transform uiAnchorOverride;
    [SerializeField] private Vector3 uiOffset = new Vector3(0f, 1.2f, 0f);

    private GameObject spawnedFireEffect;
    private Material[][] runtimeMaterials;
    private Color[][] originalColors;

    private Coroutine burnRoutine;
    private Coroutine spreadRoutine;

    private Transform[] cachedChildTransforms;
    private int[] cachedOriginalLayers;
    private bool layersCached = false;

    public FireType Type => fireType;
    public bool IsBurning => isBurning;
    public bool IsBurnedOut => isBurnedOut;
    public bool IsGasSoaked => isGasSoaked;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

private void Awake()
{
    currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

    if (TargetMeshFilter == null)
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        for (int i = 0; i < meshFilters.Length; i++)
        {
            if (meshFilters[i] != null && meshFilters[i].sharedMesh != null)
            {
                TargetMeshFilter = meshFilters[i];
                break;
            }
        }
    }

    if (targetRenderers == null || targetRenderers.Length == 0)
    {
        targetRenderers = GetComponentsInChildren<Renderer>();
    }

    CreateRuntimeMaterialInstances();
    UpdateBurnVisual();
    CacheLayers();
}

    private void CreateRuntimeMaterialInstances()
    {
        runtimeMaterials = new Material[targetRenderers.Length][];
        originalColors = new Color[targetRenderers.Length][];

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] == null)
            {
                continue;
            }

            Material[] mats = targetRenderers[i].materials;
            runtimeMaterials[i] = new Material[mats.Length];
            originalColors[i] = new Color[mats.Length];

            for (int j = 0; j < mats.Length; j++)
            {
                Material newMat = new Material(mats[j]);
                runtimeMaterials[i][j] = newMat;

                if (newMat.HasProperty("_BaseColor"))
                {
                    originalColors[i][j] = newMat.GetColor("_BaseColor");
                }
                else if (newMat.HasProperty("_Color"))
                {
                    originalColors[i][j] = newMat.GetColor("_Color");
                }
                else
                {
                    originalColors[i][j] = Color.white;
                }
            }

            targetRenderers[i].materials = runtimeMaterials[i];
        }
    }


    public void ApplyHoverOutlineColor()
{
    if (targetRenderers == null || targetRenderers.Length == 0)
    {
        targetRenderers = GetComponentsInChildren<Renderer>();
    }

    if (hoverPropertyBlock == null)
    {
        hoverPropertyBlock = new MaterialPropertyBlock();
    }

    Color hoverColor = GetHoverColor();

    for (int i = 0; i < targetRenderers.Length; i++)
    {
        if (targetRenderers[i] == null)
        {
            continue;
        }

targetRenderers[i].GetPropertyBlock(hoverPropertyBlock);

// ONLY set outline color, do not touch base color
hoverPropertyBlock.SetColor(OutlineColorID, hoverColor);

targetRenderers[i].SetPropertyBlock(hoverPropertyBlock);
    }
}

    private void CacheLayers()
    {
        cachedChildTransforms = GetComponentsInChildren<Transform>(true);
        cachedOriginalLayers = new int[cachedChildTransforms.Length];

        for (int i = 0; i < cachedChildTransforms.Length; i++)
        {
            cachedOriginalLayers[i] = cachedChildTransforms[i].gameObject.layer;
        }

        layersCached = true;
    }

public void SetHovered(bool hovered, int hoveredLayer)
{
    if (!layersCached)
    {
        CacheLayers();
    }

    if (hovered)
    {
        ApplyHoverOutlineColor();
    }

    for (int i = 0; i < cachedChildTransforms.Length; i++)
    {
        if (cachedChildTransforms[i] == null)
        {
            continue;
        }

        cachedChildTransforms[i].gameObject.layer = hovered ? hoveredLayer : cachedOriginalLayers[i];
    }
}

    public bool CanBeIgnitedByTorch()
    {
        return fireType == FireType.Combustible || fireType == FireType.Chemical;
    }

    public void ApplyGas()
    {
        if (isBurnedOut)
        {
            return;
        }

        isGasSoaked = true;
        Debug.Log("Gas is now on Object");
    }

    public void AddIgniteDamage(float amount)
    {
        if (isBurnedOut)
        {
            return;
        }

        if (isBurning)
        {
            return;
        }

        float finalAmount = amount;
        if (isGasSoaked)
        {
            finalAmount *= gasIgniteMultiplier;
        }

        currentHealth -= finalAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log("Updating burn visual on " + name);

        if (currentHealth <= GetIgnitionHealthThreshold())
        {
            StartBurning();
        }
        else if (currentHealth <= 0f)
        {
            BurnOut();
        }
    }

    public void IgniteImmediately()
    {
        if (isBurning || isBurnedOut)
        {
            return;
        }

        StartBurning();
    }

    private void StartBurning()
    {
        if (isBurning || isBurnedOut)
        {
            return;
        }

        isBurning = true;

        SpawnFireEffect();

        if (burnRoutine != null)
        {
            StopCoroutine(burnRoutine);
        }
        burnRoutine = StartCoroutine(BurnRoutine());

        if (canSpreadFire)
        {
            if (spreadRoutine != null)
            {
                StopCoroutine(spreadRoutine);
            }
            spreadRoutine = StartCoroutine(SpreadRoutine());
        }
    }

    public void Extinguish()
    {
        if (!isBurning)
        {
            return;
        }

        isBurning = false;

        if (burnRoutine != null)
        {
            StopCoroutine(burnRoutine);
            burnRoutine = null;
        }

        if (spreadRoutine != null)
        {
            StopCoroutine(spreadRoutine);
            spreadRoutine = null;
        }

        DestroyFireEffect();
    }

    private IEnumerator BurnRoutine()
    {
        while (isBurning && !isBurnedOut)
        {
            float damage = burnDamagePerSecond * Time.deltaTime;

            if (isGasSoaked)
            {
                damage *= gasBurnMultiplier;
            }

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

Debug.Log("Updating burn visual on " + name);
            if (currentHealth <= 0f)
            {
                BurnOut();
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator SpreadRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(spreadInterval);

        while (isBurning && !isBurnedOut)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, spreadRadius, spreadLayers);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] == null)
                {
                    continue;
                }

                BurnableObject other = hits[i].GetComponentInParent<BurnableObject>();

                if (other == null || other == this)
                {
                    continue;
                }

                if (other.IsBurnedOut || other.IsBurning)
                {
                    continue;
                }

                float igniteDamageThisTick = spreadIgniteDamagePerSecond * spreadInterval;
                other.AddIgniteDamage(igniteDamageThisTick);
            }

            yield return wait;
        }
    }

    private void BurnOut()
    {
        currentHealth = 0f;
        isBurning = false;
        isBurnedOut = true;

        if (burnRoutine != null)
        {
            StopCoroutine(burnRoutine);
            burnRoutine = null;
        }

        if (spreadRoutine != null)
        {
            StopCoroutine(spreadRoutine);
            spreadRoutine = null;
        }

        UpdateBurnVisual();
        DestroyFireEffect();
    }

private void SpawnFireEffect()
{
    if (fireParticlePrefab == null || spawnedFireEffect != null)
    {
        return;
    }

    Transform spawnTransform = particleSpawnPoint != null ? particleSpawnPoint : transform;
    Vector3 spawnPosition = spawnTransform.position + particleOffset;

    spawnedFireEffect = Instantiate(fireParticlePrefab, spawnPosition, Quaternion.identity, transform);

    PSMesh psMesh = spawnedFireEffect.GetComponent<PSMesh>();

    if (psMesh == null)
    {
        Debug.LogWarning("Spawned fire effect is missing a PSMesh component.", spawnedFireEffect);
        return;
    }

    if (TargetMeshFilter == null)
    {
        TargetMeshFilter = GetComponentInChildren<MeshFilter>();

        if (TargetMeshFilter == null)
        {
            Debug.LogWarning("No TargetMeshFilter assigned or found for " + gameObject.name, gameObject);
            return;
        }
    }

    if (TargetMeshFilter.sharedMesh == null)
    {
        Debug.LogWarning("TargetMeshFilter has no mesh assigned on " + TargetMeshFilter.name, TargetMeshFilter);
        return;
    }

    psMesh.SetMesh(TargetMeshFilter);
}

    private void DestroyFireEffect()
    {
        if (spawnedFireEffect != null)
        {
            Destroy(spawnedFireEffect);
            spawnedFireEffect = null;
        }
    }

    private void UpdateBurnVisual()
    {
        float healthPercent = maxHealth > 0f ? currentHealth / maxHealth : 0f;
        float burnAmount = 1f - healthPercent;

        for (int i = 0; i < runtimeMaterials.Length; i++)
        {
            if (runtimeMaterials[i] == null || originalColors[i] == null)
            {
                continue;
            }

            for (int j = 0; j < runtimeMaterials[i].Length; j++)
            {
                Material mat = runtimeMaterials[i][j];
                if (mat == null)
                {
                    continue;
                }

                Color startColor = originalColors[i][j];
                Color currentColor = Color.Lerp(startColor, burnedColor, burnAmount);

                if (mat.HasProperty("_BaseColor"))
                {
                    mat.SetColor("_BaseColor", currentColor);
                }
                else if (mat.HasProperty("_Color"))
                {
                    mat.SetColor("_Color", currentColor);
                }

                if (affectEmission && mat.HasProperty("_EmissionColor"))
                {
                    Color emission = Color.Lerp(Color.black, currentColor * 0.2f, 1f - burnAmount);
                    mat.SetColor("_EmissionColor", emission);
                }
            }
        }
    }

    public float GetIgnitionHealthThreshold()
    {
        return maxHealth * (1f - igniteDamagePercentToStartBurning);
    }

    public void Ignite()
{
    IgniteImmediately();
}

    public float GetIgnitionProgress01()
    {
        if (isBurning)
        {
            return 1f;
        }

        float threshold = GetIgnitionHealthThreshold();

        if (Mathf.Approximately(maxHealth, threshold))
        {
            return 0f;
        }

        float t = Mathf.InverseLerp(maxHealth, threshold, currentHealth);
        return Mathf.Clamp01(t);
    }

    public Vector3 GetUIWorldPosition()
    {
        if (uiAnchorOverride != null)
        {
            return uiAnchorOverride.position;
        }

        Bounds bounds = GetCombinedBounds();
        if (bounds.size != Vector3.zero)
        {
            return new Vector3(bounds.center.x, bounds.max.y, bounds.center.z) + uiOffset;
        }

        return transform.position + uiOffset;
    }

    private Bounds GetCombinedBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return new Bounds(transform.position, Vector3.zero);
        }

        Bounds combined = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            combined.Encapsulate(renderers[i].bounds);
        }

        return combined;
    }

public Color GetHoverColor()
{
    switch (fireType)
    {
        case FireType.Combustible:
            return Color.red;

        case FireType.Electrical:
            return Color.blue;

        case FireType.Chemical:
            return Color.green;

        default:
            return Color.white;
    }
}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spreadRadius);
    }
}