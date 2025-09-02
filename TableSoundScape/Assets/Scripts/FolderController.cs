using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FolderController : MonoBehaviour
{
    [SerializeField] public GameObject content;
    [SerializeField] private GameObject buttonImage;
    [SerializeField] public TextMeshProUGUI text;

    public Image colorImage;
    private RectTransform parentTransform;

    public void Initialize(string name, Color color)
    {
        parentTransform = transform.parent.GetComponent<RectTransform>();

        if (name == "") name = "New Folder";

        text.text = name;
        colorImage = GetComponent<Image>();
        colorImage.color = color;
    }

    public void UpdateFolder(string name, Color color)
    {
        text.text = name;
        colorImage.color = color;
    }

    public void OnToggleFolderClick()
    {
        content.SetActive(!content.activeSelf);
        if (content.activeSelf) buttonImage.transform.rotation = Quaternion.Euler(0, 0, 180);
        else buttonImage.transform.rotation = Quaternion.Euler(0, 0, 270);

        Refresh();
    }

    public void Refresh()
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentTransform);
    }

    public void OnMoveFolderClick(int i)
    {
        int currentIndex = transform.GetSiblingIndex();
        int siblingCount = transform.parent.childCount;
        int maxIndex = Mathf.Max(0, siblingCount - 2); // never allow last index
        int newIndex = currentIndex;
        if (i > 0)
        {
            newIndex = Mathf.Max(0, currentIndex - 1);
        }
        else if (i < 0)
        {
            newIndex = Mathf.Min(maxIndex, currentIndex + 1);
        }
        newIndex = Mathf.Min(newIndex, maxIndex); // ensure never last index
        if (newIndex != currentIndex)
        {
            transform.SetSiblingIndex(newIndex);
            Refresh();
        }
    }

    public void OnEditFolderClick()
    {
        UIManager.instance.OnEditFolderClick(this);
    }
    
    public void OnDeleteFolderClick()
    {
        Destroy(gameObject);
    }

    public void AddScene(SceneObject scene)
    {
        UIManager.instance.addScene(scene, this);
        Refresh();
    }
}
