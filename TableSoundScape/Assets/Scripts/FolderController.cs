using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FolderController : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject buttonImage;
    [SerializeField] private TextMeshProUGUI text;

    private RectTransform parentTransform;

    public void Initialize(string name, Color color)
    {
        parentTransform = transform.parent.GetComponent<RectTransform>();

        if (name == "") name = "New Folder";

        text.text = name;
        GetComponent<Image>().color = color;
    }

    public void OnToggleFolderClick()
    {
        content.SetActive(!content.activeSelf);
        if (content.activeSelf) buttonImage.transform.rotation = Quaternion.Euler(0, 0, 180);
        else buttonImage.transform.rotation = Quaternion.Euler(0, 0, 90);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentTransform);
    }
}
