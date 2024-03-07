using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class LayerEditor : MonoBehaviour
{
    [SerializeField] private GameObject SequencePrefab;

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Image loopButtonImage;
    [SerializeField] private Slider volumeSlider;

    private int loopState; //0:Shuffle ; 1:LoopRandom ; 2:OneShot ; 3:Sequential

    [SerializeField] private List<Sprite> loopStateSpriteSheet;

    private List<SequenceEditor> sequenceEditorList;

    public void Initialize()
    {
        sequenceEditorList = new List<SequenceEditor>();
        OnAddSequenceClick();
    }

    public void InitializeEdited(LayerObject o)
    {
        sequenceEditorList = new List<SequenceEditor>();

        nameInput.text = o.layerName;
        while (loopState != o.loopState) { OnLoopClick(); }
        volumeSlider.value = Mathf.FloorToInt(o.initialVolume * 100);

        for (int i = 0; i < o.sequences.Count; i++)
        {
            var sequenceObject = Instantiate(SequencePrefab, transform);
            sequenceObject.transform.SetSiblingIndex(sequenceObject.transform.GetSiblingIndex() - 1);
            SceneEditor.instance.RefreshLayoutGroupsImmediateAndRecursive();

            var sequence = sequenceObject.GetComponent<SequenceEditor>();
            sequence.InitializeEdit(this, o.sequences[i], sequenceEditorList.Count);
            sequenceEditorList.Add(sequence);
            sequence.updateLoopState(loopState);
        }
    }

    public void OnLoopClick()
    {
        loopState++;
        if (loopState > 3) loopState = 0;
        loopButtonImage.sprite = loopStateSpriteSheet[loopState];
        foreach (var sequence in sequenceEditorList) { sequence.updateLoopState(loopState); }
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
