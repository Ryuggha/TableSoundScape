using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu("Event/RightButtonEvent")]
public class RightButtonEvent : MonoBehaviour, IPointerClickHandler
{
	[System.Serializable] public class RightButton : UnityEvent { }
	public RightButton OnRightClick;

    public void OnPointerClick(PointerEventData eventData)
    {
		if (eventData.button == PointerEventData.InputButton.Right) OnRightClick.Invoke();
	}
}