using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class LayerEditor : MonoBehaviour
{
    [SerializeField] private GameObject SequencePrefab;

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TextMeshProUGUI layerTypeButtonText;
    [SerializeField] private Tooltip layerTypeButtonTooltip;
    [SerializeField] private Image layerBG;
    [SerializeField] private Image loopButtonImage;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Tooltip sequenceLoopTypeTooltip;

    [SerializeField] private Color layerColor;
    [SerializeField] private Color introColor;
    [SerializeField] private Color stingerColor;

    private int layerType; //0:Layer ; 1:Intro ; 2:Stinger
    private int loopState; //0:Shuffle ; 1:LoopRandom ; 2:OneShot ; 3:Sequential ; 4:SequentialLastLoop

    [SerializeField] private List<Sprite> loopStateSpriteSheet;

    private List<SequenceEditor> sequenceEditorList;

    public void Initialize()
    {
        sequenceEditorList = new List<SequenceEditor>();
        OnAddSequenceClick();

        SetLayerType(0);
        SetLoop(4);
    }

    public void InitializeEdited(LayerObject o)
    {
        sequenceEditorList = new List<SequenceEditor>();

        nameInput.text = o.layerName;
        
        volumeSlider.value = Mathf.FloorToInt(o.initialVolume * 100);

        for (int i = 0; i < o.sequences.Count; i++)
        {
            var sequenceObject = Instantiate(SequencePrefab, transform);
            sequenceObject.transform.SetSiblingIndex(sequenceObject.transform.GetSiblingIndex() - 1);
            SceneEditor.instance.RefreshLayoutGroupsImmediateAndRecursive();

            var sequence = sequenceObject.GetComponent<SequenceEditor>();
            sequence.InitializeEdit(this, o.sequences[i], sequenceEditorList.Count);
            sequenceEditorList.Add(sequence);
        }
        SetLayerType(o.layerType);
        SetLoop(o.loopState);
    }

    private void SetTooltip()
    {
        switch (loopState)
        {
            case 0: sequenceLoopTypeTooltip.SetTooltipText("Shuffle: Loop random sequence one after the other"); break;
            case 1: sequenceLoopTypeTooltip.SetTooltipText("Loop Random: Loop one random sequence"); break;
            case 2: sequenceLoopTypeTooltip.SetTooltipText("One Shot: Play one random track once"); break;
            case 3: sequenceLoopTypeTooltip.SetTooltipText("Sequential: Play each sequence in order and then loop"); break;
            case 4: sequenceLoopTypeTooltip.SetTooltipText("Intro + Loop: Play each sequence in order and then loop the last one"); break;
            default: sequenceLoopTypeTooltip.SetTooltipText("Invalid sequence type"); break;
        }

        loopButtonImage.sprite = loopStateSpriteSheet[loopState];
    }

    public void OnLayerTypeClick()
    {
        int layerType = this.layerType + 2;
        if (layerType > 2) layerType = 0;
        SetLayerType(layerType);
    }

    private void SetLayerType(int i)
    {
        layerType = i;

        if (layerType == 0)
        {
            layerTypeButtonText.text = "Layer";
            layerBG.color = layerColor;
            loopButtonImage.transform.parent.gameObject.SetActive(true);
            layerTypeButtonTooltip.SetTooltipText("Normal layer");
        }
        else
        {
            if (layerType == 1)
            {
                layerTypeButtonText.text = "Intro";
                layerBG.color = introColor;
                layerTypeButtonTooltip.SetTooltipText("Plays before other layers");
            }
            else if (layerType == 2)
            {
                layerTypeButtonText.text = "Stinger";
                layerBG.color = stingerColor;
                layerTypeButtonTooltip.SetTooltipText("Plays when scene ends");
            }
            loopButtonImage.transform.parent.gameObject.SetActive(false);
        }
    }

    public void OnLoopClick()
    {
        int loop = loopState + 1;
        if (loop > 4) loop = 0;

        SetLoop(loop);
    }

    public void SetLoop(int i)
    {
        loopState = i;

        foreach (var sequence in sequenceEditorList) { sequence.updateLoopState(loopState); }

        SetTooltip();
    }

    public void OnDeleteClick()
    {
        SceneEditor.instance.deleteLayer(this);
    }

    public void OnAddSequenceClick()
    {
        var sequenceObject = Instantiate(SequencePrefab, transform);
        sequenceObject.transform.SetSiblingIndex(sequenceObject.transform.GetSiblingIndex() - 1);
        SceneEditor.instance.RefreshLayoutGroupsImmediateAndRecursive();

        var sequence = sequenceObject.GetComponent<SequenceEditor>();
        sequence.Initialize(this, sequenceEditorList.Count);
        sequenceEditorList.Add(sequence);
        sequence.updateLoopState(loopState);
    }

    public void deleteSequence(SequenceEditor sequence)
    {
        sequenceEditorList.Remove(sequence);
        Destroy(sequence.gameObject);
        SceneEditor.instance.RefreshLayoutGroupsImmediateAndRecursive();
    }

    public LayerObject getLayerObject()
    {
        var sequences = new List<SequenceObject>();

        for (int i = 0; i < sequenceEditorList.Count; i++)
        {
            sequences.Add(sequenceEditorList[i].getSequenceObject());
        }

        return new LayerObject
        (
            nameInput.text == "" ? "Untitled Layer" : nameInput.text,
            volumeSlider.value / 100f,
            layerType,
            loopState,
            sequences
        );
    }

    public void changeSequenceOrder(bool increase, int index) //Returns the new Index
    {

        if (increase)
        {
            if (index < sequenceEditorList.Count - 1)
            {
                sequenceEditorList[index].transform.SetSiblingIndex(index + 2);
                sequenceEditorList[index].setIndex(index + 1);
                var aux = sequenceEditorList[index];
                sequenceEditorList[index] = sequenceEditorList[index + 1];
                sequenceEditorList[index + 1] = aux;
            }
        }
        else
        {
            if (index > 0)
            {
                sequenceEditorList[index].transform.SetSiblingIndex(index);
                sequenceEditorList[index].setIndex(index - 1);
                var aux = sequenceEditorList[index];
                sequenceEditorList[index] = sequenceEditorList[index - 1];
                sequenceEditorList[index - 1] = aux;
            }
        }
    }
}
