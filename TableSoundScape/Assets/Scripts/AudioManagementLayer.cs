using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioManagementLayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI layerNameField;
    [SerializeField] private Slider layerVolumeSlider;

    private AudioManagement manager;
    private int layerIndex;

    public void Initialize(AudioManagement manager, int layerIndex, string layerName, int initialVolume)
    {
        this.layerIndex = layerIndex;
        this.manager = manager;

        Debug.Log(initialVolume);

        layerVolumeSlider.value = initialVolume;
        layerNameField.text = layerName;
        layerVolumeSlider.onValueChanged.AddListener(updateSlider);
    }

    private void updateSlider(float value)
    {
        manager.sendVolume(value, layerIndex);
    }
}
