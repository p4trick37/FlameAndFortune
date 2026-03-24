using System.Collections;
using UnityEngine;

public class BurnableObject : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    [Header("Burning")]
    [SerializeField] private float burnDamagePerSecond = 10f;
    [SerializeField] private bool isBurning = false;
    [SerializeField] private bool isBurnedOut = false;

    [Header("Gas")]
    [SerializeField] private bool isGasSoaked = false;
    [SerializeField] private float gasBurnMultiplier = 2f;

    [Header("Spread")]
    [SerializeField] private bool canSpreadFire = true;
    [SerializeField] private float spreadRadius = 2f;
    [SerializeField] private float spreadInterval = 1f;
    [SerializeField] private LayerMask spreadLayers = ~0;

    [Header("Visual Burn Darkening")]
    [SerializeField] private Renderer[] targetRenderers;
    [SerializeField] private Color burnedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    [SerializeField] private bool affectEmission = false;

    [Header("Particles")]
    [SerializeField] private GameObject fireParticlePrefab;
    [SerializeField] private Transform particleSpawnPoint;
    [SerializeField] private Vector3 particleOffset = Vector3.zero;

    private GameObject spawnedFireEffect;
    private Material[][] runtimeMaterials;
    private Color[][] originalColors;

    private Coroutine burnRoutine;
    private Coroutine spreadRoutine;

    public bool IsBurning => isBurning;
    public bool IsBurnedOut => isBurnedOut;
    public bool IsGasSoaked => isGasSoaked;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            targetRenderers = GetComponentsInChildren<Renderer>();
        }

        CreateRuntimeMaterialInstances();
        UpdateBurnVisual();
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

            Material[] sharedMats = targetRenderers[i].materials;
            runtimeMaterials[i] = new Material[sharedMats.Length];
            originalColors[i] = new Color[sharedMats.Length];

            for (int j = 0; j < sharedMats.Length; j++)
            {
                Material newMat = new Material(sharedMats[j]);
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

    public void Ignite()
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

    public void ApplyGas()
    {
        if (isBurnedOut)
        {
            return;
        }

        isGasSoaked = true;
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
            float multiplier = isGasSoaked ? gasBurnMultiplier : 1f;
            float damage = burnDamagePerSecond * multiplier * Time.deltaTime;

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            UpdateBurnVisual();

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

                if (hits[i].gameObject == gameObject)
                {
                    continue;
                }

                BurnableObject other = hits[i].GetComponentInParent<BurnableObject>();

                if (other == null)
                {
                    continue;
                }

                if (other == this)
                {
                    continue;
                }

                if (!other.IsBurning && !other.IsBurnedOut)
                {
                    other.Ignite();
                }
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
        if (fireParticlePrefab == null)
        {
            return;
        }

        if (spawnedFireEffect != null)
        {
            return;
        }

        Transform spawnTransform = particleSpawnPoint != null ? particleSpawnPoint : transform;
        Vector3 spawnPosition = spawnTransform.position + particleOffset;

        spawnedFireEffect = Instantiate(fireParticlePrefab, spawnPosition, Quaternion.identity, transform);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spreadRadius);
    }
}