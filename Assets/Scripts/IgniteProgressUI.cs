using UnityEngine;
using UnityEngine.UI;

public class IgniteProgressUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Vector3 extraOffset = Vector3.zero;

    private BurnableObject target;
    private Camera cam;

    private void Awake()
    {
        Hide();
    }

    private void LateUpdate()
    {
        if (target == null || cam == null)
        {
            return;
        }

        transform.position = target.GetUIWorldPosition() + extraOffset;

        Vector3 direction = transform.position - cam.transform.position;
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void Show(BurnableObject newTarget, Camera targetCamera, Color fillColor)
    {
        target = newTarget;
        cam = targetCamera;

        if (fillImage != null)
        {
            fillImage.color = fillColor;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void SetFill(float value, bool isActivelyIgniting)
    {
        if (slider != null)
        {
            slider.value = Mathf.Clamp01(value);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = isActivelyIgniting || value > 0f ? 1f : 0.65f;
        }
    }

    public void Hide()
    {
        target = null;
        cam = null;

        if (slider != null)
        {
            slider.value = 0f;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}