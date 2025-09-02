using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreateFolderPanelManager : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public Image colorImage;
    public Slider hSlider;
    public Slider sSlider;
    public Slider vSlider;

    private bool updatingFolder;
    private FolderController updatingFolderController;

    public void Initialize(FolderController folder = null)
    {
        updatingFolder = folder != null;

        if (updatingFolder)
        {
            updatingFolderController = folder;

            nameInputField.text = folder.text.text;

            Color.RGBToHSV(folder.colorImage.color, out float h, out float s, out float v);
            hSlider.value = h;
            sSlider.value = s;
            vSlider.value = v;
        }
        else
        {
            nameInputField.text = string.Empty;

            hSlider.value = .1f;
            sSlider.value = .36f;
            vSlider.value = .87f;
        }
            
        OnColorChange();
    }
    
    public void OnCancelClick()
    {
        gameObject.SetActive(false);
    }

    public void OnCreateClick()
    {
        if (updatingFolder)
        {
            updatingFolderController.UpdateFolder(nameInputField.text, Color.HSVToRGB(hSlider.value, sSlider.value, vSlider.value));
        }
        else
        {
            UIManager.instance.AddFolder(nameInputField.text, Color.HSVToRGB(hSlider.value, sSlider.value, vSlider.value));
        }

        gameObject.SetActive(false);
    }

    public void OnColorChange()
    {
        colorImage.color = Color.HSVToRGB(hSlider.value, sSlider.value, vSlider.value);
    }
}
