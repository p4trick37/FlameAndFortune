using UnityEngine;

public class FireTool : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Range")]
    [SerializeField] private float useDistance = 8f;

    [Header("Layers")]
    [SerializeField] private LayerMask hitLayers = ~0;

    [Header("Debug")]
    [SerializeField] private bool drawDebugRay = true;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryIgnite();
        }

        if (Input.GetMouseButtonDown(1))
        {
            TryApplyGas();
        }
    }

    private void TryIgnite()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("FireTool: No player camera assigned.");
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (drawDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * useDistance, Color.red, 1f);
        }

        if (Physics.Raycast(ray, out RaycastHit hit, useDistance, hitLayers))
        {
            BurnableObject burnable = hit.collider.GetComponentInParent<BurnableObject>();

            if (burnable != null)
            {
                burnable.Ignite();
            }
        }
    }

    private void TryApplyGas()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("FireTool: No player camera assigned.");
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (drawDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * useDistance, Color.yellow, 1f);
        }

        if (Physics.Raycast(ray, out RaycastHit hit, useDistance, hitLayers))
        {
            BurnableObject burnable = hit.collider.GetComponentInParent<BurnableObject>();

            if (burnable != null)
            {
                burnable.ApplyGas();
            }
        }
    }
}