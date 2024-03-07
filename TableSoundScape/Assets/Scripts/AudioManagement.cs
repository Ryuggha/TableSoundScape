using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioManagement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sceneNameField;
    [SerializeField] public Slider sceneVolumeSlider;

    [SerializeField] private GameObject AudioManagementLayerPrefab;

    private ScenePanel panel;

    public void Initialize(ScenePanel panel, float fadeInTime)
    {
        this.panel = panel;
        sceneNameField.text = panel.scene.sceneName;
        sceneVolumeSlider.value = Mathf.FloorToInt(panel.scene.initialVolume * 100);
        sceneVolumeSlider.onValueChanged.AddListener(updateSlider);

        if (panel.layers.Count > 1)
        {
            for (int i = 0; i < panel.layers.Count; i++)
            {
                Instantiate(AudioManagementLayerPrefab, transform).GetComponent<AudioManagementLayer>()
                    .Initialize(this, i, panel.layers[i].layer.layerName, Mathf.FloorToInt(panel.layers[i].layer.initialVolume * 100));
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
}
