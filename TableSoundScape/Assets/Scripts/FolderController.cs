using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FolderController : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject buttonImage;
    [SerializeField] private TextMeshProUGUI text;

    private RectTransform parentTransform;

    public void Initialize()
    {
        parentTransform = transform.parent.GetComponent<RectTransform>();
    }

    public void OnToggleFolderClick()
    {
        content.SetActive(!content.activeSelf);
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentTransform);
    }
}
