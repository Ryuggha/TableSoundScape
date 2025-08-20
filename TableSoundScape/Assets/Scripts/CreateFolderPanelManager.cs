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

    private UIManager ui;

    public void Initialize(UIManager ui)
    {
        this.ui = ui;

        nameInputField.text = string.Empty;
        hSlider.value = .1f;    
        sSlider.value = .36f;   
        vSlider.value = .87f;   
        OnColorChange();
    }
    
    public void OnCancelClick()
    {
        gameObject.SetActive(false);
    }

    public void OnCreateClick()
    {
        ui.AddFolder(nameInputField.text, Color.HSVToRGB(hSlider.value, sSlider.value, vSlider.value));
        gameObject.SetActive(false);
    }

    public void OnColorChange()
    {
        colorImage.color = Color.HSVToRGB(hSlider.value, sSlider.value, vSlider.value);
    }
}
