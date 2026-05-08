using UnityEngine;

public class LevelBurnVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Renderer[] targetRenderers;

    [Header("Visual Settings")]
    [SerializeField] private Color burnedColor = new Color(0.12f, 0.12f, 0.12f, 1f);
    [SerializeField] private bool affectEmission = false;

    [Tooltip("0 means start darkening immediately. 0.25 means wait until 25% burned.")]
    [Range(0f, 1f)]
    [SerializeField] private float darkeningStartPercent = 0f;

    [Tooltip("1 means reach full wall darkening at 100%. 0.75 means fully dark by 75%.")]
    [Range(0.01f, 1f)]
    [SerializeField] private float fullDarkPercent = 1f;

    private Material[][] runtimeMaterials;
    private Color[][] originalColors;

    private void Awake()
    {
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            targetRenderers = GetComponentsInChildren<Renderer>();
        }

        CreateRuntimeMaterialInstances();
        UpdateWallVisual(0f);
    }

    private void Update()
    {
        if (gameManager == null)
        {
            return;
        }

        float percent01 = Mathf.Clamp01((float)gameManager.PercentBurnedValue / 100f);
        Debug.Log("Debug gmess" + gameManager.PercentBurnedValue);
        UpdateWallVisual(percent01);
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

private void UpdateWallVisual(float levelBurnPercent01)
{
    float mappedBurn = RemapBurn(levelBurnPercent01);

    for (int i = 0; i < targetRenderers.Length; i++)
    {
        if (targetRenderers[i] == null)
        {
            continue;
        }

        Material[] mats = targetRenderers[i].materials;

        for (int j = 0; j < mats.Length; j++)
        {
            Material mat = mats[j];
            if (mat == null)
            {
                continue;
            }

            Color startColor = originalColors[i][j];
            Color currentColor = Color.Lerp(startColor, burnedColor, mappedBurn);
            //Debug.Log("Updating wall material on renderer: " + targetRenderers[i].name + " | Material: " + mat.name);

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
                Color emission = Color.Lerp(Color.black, currentColor * 0.2f, 1f - mappedBurn);
                mat.SetColor("_EmissionColor", emission);
            }
        }
    }
}

    private float RemapBurn(float rawBurn01)
    {
        // if (rawBurn01 <= darkeningStartPercent)
        // {
            
        //     return 0f;
        // }
//Debug.Log(rawBurn01);
        float range = Mathf.Max(0.0001f, fullDarkPercent - darkeningStartPercent);
        float mapped = (rawBurn01 - darkeningStartPercent) / range;
        return Mathf.Clamp01(mapped);
    }
}