using System;
using System.Collections.Generic;

[Serializable]
public class SceneObject
{
    public string sceneName;
    public float initialVolume;
    public float fadeInTime;
    public float fadeOutTime;
    public List<LayerObject> layers;

    public string image;

    public SceneObject(string sceneName, float initialVolume, float fadeInTime, float fadeOutTime, string image, List<LayerObject> layers)
    {
        this.sceneName = sceneName;
        this.initialVolume = initialVolume;
        this.fadeInTime = fadeInTime;
        this.fadeOutTime = fadeOutTime;
        this.image = image;

        this.layers = layers;
    }
}
