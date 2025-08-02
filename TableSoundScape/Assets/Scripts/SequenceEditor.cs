using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AnotherFileBrowser.Windows;

public class SequenceEditor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI audioPath;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider weightSlider;
    [SerializeField] private TMP_InputField silenceTimeInputField;
    [SerializeField] private GameObject VolumeSliderPanel;
    [SerializeField] private GameObject EmptyTimeSliderPanel;
    [SerializeField] private GameObject WeightPanel;
    [SerializeField] private GameObject OrderPanel;

    private LayerEditor layer;

    private string soundPath;
    private int index;

    public void Initialize(LayerEditor layer, int index)
    {
        setIndex(index);
        this.layer = layer;
    }

    public void InitializeEdit(LayerEditor layer, SequenceObject o, int index)
    {
        setIndex(index);
        this.layer = layer;

        soundPath = o.soundPath;
        setPathText(soundPath);
        volumeSlider.value = Mathf.FloorToInt(o.volume * 100);
        weightSlider.value = o.randomWeight;
        if (o.silenceTime > 0) silenceTimeInputField.text = o.silenceTime.ToString();
        VolumeSliderPanel.SetActive(!(soundPath == "" || soundPath == null));
        EmptyTimeSliderPanel.SetActive(soundPath == "" || soundPath == null);
    }

    public void updateLoopState(int loopState) 
    {
        if (loopState == 3 || loopState == 4)
        {
            OrderPanel.SetActive(true);
            WeightPanel.SetActive(false);
        }
        else
        {
            OrderPanel.SetActive(false);
            WeightPanel.SetActive(true);
        }
    }

    public void OnUpOrderClick()
    {
        layer.changeSequenceOrder(false, index);
    }

    public void OnDownOrderClick()
    {
        layer.changeSequenceOrder(true, index);
    }

    public void OnSearchSoundClick()
    {
        var browserProperties = new BrowserProperties();
        browserProperties.filter = "Sound files (*.wav; *.ogg; *.mp3; *.aiff) | *.wav; *.ogg; *.mp3; *.aiff";
        browserProperties.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(browserProperties, path =>
        {
            setPathText(path);
        });
    }

    private void setPathText (string s)
    {
        if (s == null || s == "") return;
        soundPath = s;
        var split = soundPath.Split("\\");
        audioPath.text = split[split.Length - 1];
        VolumeSliderPanel.SetActive(true);
        EmptyTimeSliderPanel.SetActive(false);
    }

    public void OnDestroyLayerClick()
    {
        layer.deleteSequence(this);
    }

    public SequenceObject getSequenceObject()
    {
        float silenceTime;
        try { silenceTime = float.Parse(silenceTimeInputField.text); } catch (System.Exception) { silenceTime = 0; };
        if (silenceTime < 0) silenceTime = 0;

        return new SequenceObject(
            soundPath,
            volumeSlider.value / 100f,
            Mathf.RoundToInt(weightSlider.value),
            silenceTime);
    }

    public void setIndex(int i)
    {
        this.index = i;
    }
}
