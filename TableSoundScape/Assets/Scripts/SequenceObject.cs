using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceObject
{
    public string soundPath;
    public float volume;
    public int randomWeight;
    public float silenceTime;

    public SequenceObject(string soundPath, float volume, int randomWeight, float silenceTime)
    {
        this.soundPath = soundPath;
        this.volume = volume;
        this.randomWeight = randomWeight;
        this.silenceTime = silenceTime;
    }
}
