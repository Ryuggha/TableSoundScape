using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class ScenePanel : MonoBehaviour
{
    [SerializeField] private Button sceneButton;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color transitionColor;
    [SerializeField] private Image imageHolder;
    [SerializeField] private Image frameHolder;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject AudioManagementPrefab;

    public SceneObject scene;

    public List<SceneLayerInterface> layers;

    public Color imaglessColor;

    public int index = -1;
    private AudioManagement manager;
    private int channelsPlaying;

    public float volume;
    private int panelState = 0; // 0->idle ; 1->FadeIn ; 2->Playing ; 3->FadeOut

    public void OnSceneButtonClick()
    {
        buttonStateUpdate();
    }

    public void OnSceneButtonRightClick()
    {
        if (panelState != 0) postRoutineStop();
        UIManager.instance.editScene(this);
    }

    private void buttonStateUpdate()
    {
        if (panelState == 0)
        {
            frameHolder.enabled = true;
            sceneButton.interactable = false;
            panelState = 1;
            playScene();
        }
        else if (panelState == 2)
        {
            sceneButton.interactable = false;
            panelState = 3;
            stopScene();
        }
    }

    public void ButtonUpdate()
    {
        var sources = GetComponents<AudioSource>();
        for (int i = sources.Length -1; i >= 0; i--)
        {
            Destroy(sources[i]);
        }

        layers = new List<SceneLayerInterface>();
        for (int i = 0; i < scene.layers.Count; i++)
        {
            layers.Add(new SceneLayerInterface(this, scene.layers[i]));
        }

        text.text = scene.sceneName;
        if (scene.imagePath == "")
        {
            imageHolder.color = imaglessColor;
        }
        else
        {
            imageHolder.color = Color.white;
            StartCoroutine(DownloadImage(scene.imagePath));
        } 
    }

    public void sliderUpdate(float value, int layerIndex)
    {
        if (layerIndex == -1)
        {
            volume = value / 100f;
            foreach (var layer in layers)
            {
                layer.updateVolume();
            }
        }
        else
        {
            layers[layerIndex].setVolume(value / 100f);
        }
    }

    private void playScene()
    {
        manager = Instantiate(AudioManagementPrefab, UIManager.instance.SideViewHolder.transform).GetComponent<AudioManagement>();
        manager.Initialize(this, scene.fadeInTime + .1f);
        volume = scene.initialVolume;

        foreach (var layer in layers)
        {
            layer.playLayer();
            channelsPlaying++;
        }
        StartCoroutine(FadeIn(scene.fadeInTime));
    }

    private void stopScene()
    {
        StartCoroutine(FadeOut(scene.fadeOutTime));
    }

    IEnumerator FadeIn(float totalTime)
    {
        float time = 0;
        float originalVolume = scene.initialVolume;

        while (time < totalTime && panelState != 0)
        {
            time += Time.deltaTime;
            if (time > totalTime) time = totalTime;

            float progress = time / totalTime;
            frameHolder.color = Color.Lerp(transitionColor, activeColor, progress);

            volume = originalVolume * progress;
            manager.sceneVolumeSlider.value = Mathf.FloorToInt(volume*100);
            updateVolume();

            yield return null;
        }

        postRoutineStart();
    }

    private void postRoutineStart()
    {
        if (panelState == 1)
        {
            volume = scene.initialVolume;
            updateVolume();
            if (manager != null) manager.sceneVolumeSlider.interactable = true;
            frameHolder.color = activeColor;
            panelState = 2;
            sceneButton.interactable = true;
        }
    }

    IEnumerator FadeOut(float time)
    {
        manager.sceneVolumeSlider.interactable = false;
        float totalTime = time;
        float originalVolume = volume;

        while (time > 0 && panelState != 0)
        {
            time -= Time.deltaTime;
            if (time < 0) time = 0;

            float progress = time / totalTime;
            frameHolder.color = Color.Lerp(transitionColor, activeColor, progress);

            volume = originalVolume * progress;
            manager.sceneVolumeSlider.value = Mathf.FloorToInt(volume * 100);
            updateVolume();

            yield return null;
        }

        postRoutineStop();
    }

    private void postRoutineStop()
    {
        foreach (var layer in layers)
        {
            layer.stopLayer();
        }

        if (manager != null)
        {
            Destroy(manager.gameObject);
            manager = null;
        }

        frameHolder.color = transitionColor;
        frameHolder.enabled = false;
        panelState = 0;
        sceneButton.interactable = true;
        channelsPlaying = 0;
    }

    public void channelFinished()
    {
        channelsPlaying--;
        if (channelsPlaying <= 0)
        {
            postRoutineStop();
        }
    }

    private void updateVolume()
    {
        foreach (var layer in layers)
        {
            layer.updateVolume();
        }
    }

    IEnumerator DownloadImage(string path)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture("file://" + path))
        {
            request.SendWebRequest();

            while (!request.isDone)
            {
                yield return null;
            }

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogWarning(request.result.ToString());
            }
            else
            {
                var tex = DownloadHandlerTexture.GetContent(request);
                imageHolder.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            }
        }
    }
}

public class SceneLayerInterface
{
    public float volume;
    public LayerObject layer;

    public AudioClip[] audioList;
    public AudioSource source;

    private ScenePanel scene;

    private int sequencePlaying;

    public SceneLayerInterface(ScenePanel scene, LayerObject o)
    {
        this.scene = scene;
        this.volume = o.initialVolume;
        this.layer = o;

        audioList = new AudioClip[o.sequences.Count];

        for (int i = 0; i < o.sequences.Count; i++)
        {
            if (o.sequences[i].soundPath == "" || o.sequences[i].soundPath == null) audioList[i] = null;
            else
            {
                scene.StartCoroutine(DownloadSound(o.sequences[i].soundPath, i));

            }
        }

        sequencePlaying = -1;
    }

    public void playLayer()
    {
        if (layer.sequences.Count < 1)
        {
            scene.channelFinished();
            return;
        }
        if (source == null)
        {
            source = scene.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            source.outputAudioMixerGroup = UIManager.instance.mixerGroup;
            source.playOnAwake = false;
            source.loop = (layer.loopState == 1 || ((layer.loopState == 0 || layer.loopState == 3) && layer.sequences.Count == 1)) ? true : false;
        }

        setNextClipIndex();
        if (sequencePlaying != -1)
        {
            setVolume(layer.initialVolume);
            if (audioList[sequencePlaying] != null) source.clip = audioList[sequencePlaying];
            source.Play();
        }
    }

    public void stopLayer()
    {
        if (source != null) source.Stop();
        sequencePlaying = -1;
    }

    public float getVolume()
    {
        if (sequencePlaying == -1) return 0;
        float r = 1;

        r *= scene.volume;
        r *= volume;
        r *= audioList[sequencePlaying] != null ? layer.sequences[sequencePlaying].volume : 0;

        return r;
    }

    public void setVolume(float v)
    {
        volume = v;
        updateVolume();
    }

    public void updateVolume()
    {
        if (source != null) source.volume = getVolume();
    }

    private void setNextClipIndex()
    {
        if (layer.sequences.Count == 0) scene.channelFinished();

        switch (layer.loopState)
        {
            case 0:
                sequencePlaying = randomWithWeight();
                if (audioList.Length > 1)
                {
                    float timer = layer.sequences[sequencePlaying].silenceTime;
                    if (audioList[sequencePlaying] != null) timer = audioList[sequencePlaying].length + 0.05f;
                    scene.StartCoroutine(changeOnFinish(timer));
                }
                break;
            case 1:
                sequencePlaying = randomWithWeight();
                break;
            case 2:
                sequencePlaying = randomWithWeight();
                if (audioList[sequencePlaying] == null) scene.channelFinished();
                else
                {
                    scene.StartCoroutine(stopOnFinish(audioList[sequencePlaying].length));
                }
                break;
            case 3:
                sequencePlaying++;
                if (sequencePlaying >= layer.sequences.Count)
                {
                    sequencePlaying = 0;
                }
                if (audioList.Length > 1)
                {
                    float timer = layer.sequences[sequencePlaying].silenceTime;
                    if (audioList[sequencePlaying] != null) timer = audioList[sequencePlaying].length + 0.05f;
                    scene.StartCoroutine(changeOnFinish(timer));
                }
                break;
            default:
                Debug.LogError("Invalid layer LoopState");
                break;
        }
    }

    private int randomWithWeight()
    {
        int totalWeight = 0;

        foreach (var sequence in layer.sequences)
        {
            totalWeight += sequence.randomWeight;
        }

        int r = Random.Range(0, totalWeight + 1);

        int count = 0;
        for (int i = 0; i < layer.sequences.Count; i++)
        {
            count += layer.sequences[i].randomWeight;
            if (count >= r) return i;
        }
        return 0;
    }

    IEnumerator stopOnFinish(float seconds)
    {
        float timer = seconds;

        while (timer > 0)
        {
            yield return null;
            if (sequencePlaying == -1) yield break;
            timer -= Time.deltaTime;
        }

        sequencePlaying = -1;
        scene.channelFinished();
    }

    IEnumerator changeOnFinish(float seconds)
    {
        float timer = seconds;

        while (timer > 0)
        {
            yield return null;
            if (sequencePlaying == -1) yield break;
            timer -= Time.deltaTime;
        }

        playLayer();
    }

    IEnumerator DownloadSound(string path, int index)
    {
        var split = path.Split(".");
        var format = split[split.Length - 1];
        AudioType audioType;

        switch (format.ToUpper())
        {
            case "OGG": audioType = AudioType.OGGVORBIS; break;
            case "WAV": audioType = AudioType.WAV; break;
            case "AIFF": audioType = AudioType.AIFF; break;
            default: audioType = AudioType.MPEG; break;
        }

        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType))
        {
            yield return request.SendWebRequest();

            ((DownloadHandlerAudioClip)request.downloadHandler).streamAudio = true;

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogWarning(request.result.ToString());
                audioList[index] = null;
            }
            else
            {
                var audioClip = DownloadHandlerAudioClip.GetContent(request);
                split = path.Split("/");
                audioClip.name = split[split.Length - 1];
                audioList[index] = audioClip;
            }
        }
    }
}