using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    public float hoverScale = 1.1f;
    public float clickScale = 0.9f;

    [Header("Color Tint")]
    public Color hoverColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    public Color clickColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    private Image image;

    void Start()
    {
        originalScale = transform.localScale;
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScale;
        if (image != null) image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
        if (image != null) image.color = Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = originalScale * clickScale;
        if (image != null) image.color = clickColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScale;
        if (image != null) image.color = hoverColor;
    }
}