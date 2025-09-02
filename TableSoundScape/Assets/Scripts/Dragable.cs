using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private RectTransform graphics;

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
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (graphics != null)
        {
            graphics.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (graphics != null)
        {
            graphics.position = transform.position;
        }

        var prevFolder = GetComponentInParent<FolderController>();

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("ScenePanel") && result.gameObject != gameObject)
            {
                Transform targetParent = result.gameObject.transform.parent;
                int targetIndex = result.gameObject.transform.GetSiblingIndex();
                transform.SetParent(targetParent, false);
                transform.SetSiblingIndex(targetIndex);
                break;
            }

            else if (result.gameObject.CompareTag("Folder"))
            {
                if (prevFolder != null && prevFolder.gameObject == result.gameObject) return;
                Transform targetParent = result.gameObject.transform;
                transform.SetParent(targetParent.GetChild(targetParent.childCount - 1), false);
                transform.SetAsLastSibling();
                break;
            }

            else if (result.gameObject.CompareTag("UnsortedFolder"))
            {
                if (prevFolder == null) return;
                Transform targetParent = UIManager.instance.unsortedContentHolder.transform;
                transform.SetParent(targetParent, false);
                transform.SetAsLastSibling();
                break;
            }
        }

        var newFolder = GetComponentInParent<FolderController>();

        if (prevFolder != newFolder)
        {
            prevFolder?.Refresh();
            newFolder?.Refresh();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
