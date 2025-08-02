using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioManagementLayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI layerNameField;
    [SerializeField] private Slider layerVolumeSlider;
    [SerializeField] private Image startStopButtonImage;

    [SerializeField] private Sprite pauseIcon, playIcon;

    public bool silencing;

    private AudioManagement manager;
    private int layerIndex;

    public void Initialize(AudioManagement manager, int layerIndex, string layerName, int initialVolume)
    {
        this.layerIndex = layerIndex;
        this.manager = manager;

        layerVolumeSlider.value = initialVolume;
        layerNameField.text = layerName;
    }

    private void updateSlider(float value)
    {
        if (value <= 0 || silencing) startStopButtonImage.sprite = pauseIcon;
        else startStopButtonImage.sprite = playIcon;

        manager.sendVolume(value, layerIndex);
    }

    public void OnUpdateSlider()
    {
        updateSlider(layerVolumeSlider.value);
    }

    public void SetSliderPosition(float v)
    {
        layerVolumeSlider.value = v;
    }

    public void OnStartStopClick()
    {
        manager.StartStopClick(layerIndex);
    }

    public void PauseIcon()
    {
        startStopButtonImage.sprite = pauseIcon;
    }
}
