using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Slot : MonoBehaviour
{
    [field: SerializeField] public int SlotCurrentValue { get; set; }
    [field: SerializeField] public Image SlotImage { get; private set; }
    [field: SerializeField] public RectTransform SlotRectTransform { get; private set; }
}