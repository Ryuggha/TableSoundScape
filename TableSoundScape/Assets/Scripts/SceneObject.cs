using System;
using System.Collections.Generic;

[Serializable]
public class SceneObject
{
    public string sceneName;
    public float initialVolume;
    public float fadeInTime;
    public float fadeOutTime;
    public string imagePath;

    public List<LayerObject> layers;

    public SceneObject(string sceneName, float initialVolume, float fadeInTime, float fadeOutTime, string imagePath, List<LayerObject> layers)
    {
        this.sceneName = sceneName;
        this.initialVolume = initialVolume;
        this.fadeInTime = fadeInTime;
        this.fadeOutTime = fadeOutTime;
        this.imagePath = imagePath;

        this.layers = layers;
    }
}
