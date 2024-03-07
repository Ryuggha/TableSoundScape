using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerObject
{
    public string layerName;
    public float initialVolume;
    public int loopState;

    public List<SequenceObject> sequences;

    public LayerObject(string layerName, float initialVolume, int loopState, List<SequenceObject> sequences)
    {
        this.layerName = layerName;
        this.initialVolume = initialVolume;
        this.loopState = loopState;
        this.sequences = sequences;
    }
}
