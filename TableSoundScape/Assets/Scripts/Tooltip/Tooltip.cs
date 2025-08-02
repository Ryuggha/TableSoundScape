using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.ShowTooltip(text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.HideTooltip();
    }

    public void SetTooltipText(string text)
    {
        this.text = text;
    }
}
