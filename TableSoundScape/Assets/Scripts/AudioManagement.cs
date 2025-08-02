using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AudioManagement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sceneNameField;
    [SerializeField] public Slider sceneVolumeSlider;

    [SerializeField] private GameObject AudioManagementLayerPrefab;

    public AudioManagementLayer[] layers {  get; private set; }

    private ScenePanel panel;

    public void Initialize(ScenePanel panel, float fadeInTime)
    {
        this.panel = panel;
        sceneNameField.text = panel.scene.sceneName;
        sceneVolumeSlider.value = Mathf.FloorToInt(panel.scene.initialVolume * 100);
        sceneVolumeSlider.onValueChanged.AddListener(updateSlider);

        layers = new AudioManagementLayer[panel.layers.Count];

        if (panel.layers.Count - panel.stingerLayers.Count > 1)
        {
            for (int i = 0; i < panel.layers.Count; i++)
            {
                if (panel.layers[i].layer.layerType == 0)
                {
                    var o = Instantiate(AudioManagementLayerPrefab, transform).GetComponent<AudioManagementLayer>();
                    layers[i] = o;
                    o.Initialize(this, i, panel.layers[i].layer.layerName, Mathf.FloorToInt(panel.layers[i].layer.initialVolume * 100));
                }
            }
        }

        UIManager.instance.RefreshLayoutGroupsImmediateAndRecursive();
    }

    public void sendVolume(float value, int layerIndex)
    {
        panel.sliderUpdate(value, layerIndex);
    }

    private void updateSlider(float value)
    {
        sendVolume(value, -1);
    }

    public void StartStopClick(int layerIndex)
    {
        panel.StartStopClick(layerIndex);
    }
}
