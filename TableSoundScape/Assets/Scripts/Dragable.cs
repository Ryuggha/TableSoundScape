using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Image image;
    public Transform parentAfterDrag { get; set; }
    public bool nonDraggeable;

    private int beforeDragIndex;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void Initiate()
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("a");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("e");
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
