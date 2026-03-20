using UnityEngine;
using UnityEngine.EventSystems;

public class DragTesting : MonoBehaviour,IDragHandler
{
    public RectTransform rectTransform;
    public Canvas canvas;
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}
