using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LayerObject
{
    public string layerName;
    public float initialVolume;
    public int layerType;
    public int loopState;

    public List<SequenceObject> sequences;

    public LayerObject(string layerName, float initialVolume, int layerType, int loopState, List<SequenceObject> sequences)
    {
        this.layerName = layerName;
        this.initialVolume = initialVolume;
        this.layerType = layerType;
        this.loopState = loopState;
        this.sequences = sequences;
    }
}
