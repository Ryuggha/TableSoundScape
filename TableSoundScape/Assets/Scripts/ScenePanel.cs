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
    public AudioManagement manager { get; private set; }
    private int channelsPlaying;

    public float volume;
    private int panelState = 0; // 0->idle ; 1->FadeIn ; 2->Playing ; 3->FadeOut

    public List<SceneLayerInterface> stingerLayers { get; private set; }
    private double stingerEndTime;

    public void OnSceneButtonClick()
    {
        buttonStateUpdate();
    }

    public void OnSceneButtonRightClick()
    {
        if (panelState != 0) postRoutineStop();
        UIManager.instance.editScene(this);
    }

    private void OnDestroy()
    {
        if (panelState != 0) postRoutineStop();
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
            layers.Add(new SceneLayerInterface(this, scene.layers[i], i));
        }

        text.text = scene.sceneName;
        if (scene.image == null || scene.image == "")
        {
            imageHolder.color = imaglessColor;
        }
        else
        {
            imageHolder.color = Color.white;
            imageHolder.sprite = ImageManager.DecodeFromJson(scene.image);
            //StartCoroutine(DownloadImage(scene.imagePath));
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

    public void StartStopClick(int layerIndex)
    {
        if (layerIndex >= 0 && layerIndex < layers.Count)
        {
            layers[layerIndex].StartStopClick();
        }
    }

    private void playScene()
    {
        var map = new Dictionary<int, List<SceneLayerInterface>>();

        for (int i = 0; i < 3; i++)
        {
            map.Add(i, new List<SceneLayerInterface>());
        }
        stingerLayers = map[2];

        manager = Instantiate(AudioManagementPrefab, UIManager.instance.SideViewHolder.transform).GetComponent<AudioManagement>();
        manager.Initialize(this, scene.fadeInTime + .1f);
        volume = scene.initialVolume;

        foreach (var layer in layers)
        {
            map[layer.layer.layerType].Add(layer);
        }

        foreach (var layer in map[0])
        {
            layer.playLayer();
            channelsPlaying++;
        }
        StartCoroutine(FadeIn(scene.fadeInTime));
    }

    private void stopScene()
    {
        double totalTime = 0;

        foreach (var layer in stingerLayers)
        {
            var time = layer.playLayer();
            if (time > totalTime) totalTime = time;
        }

        stingerEndTime = AudioSettings.dspTime + totalTime;

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

    IEnumerator FadeOut(float time, float delay = 0)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);

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

        while (stingerEndTime > AudioSettings.dspTime) yield return null;

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

    //IEnumerator DownloadImage(string path)
    //{
    //    using (UnityWebRequest request = UnityWebRequestTexture.GetTexture("file://" + path))
    //    {
    //        request.SendWebRequest();

    //        while (!request.isDone)
    //        {
    //            yield return null;
    //        }

    //        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    //        {
    //            Debug.LogWarning(request.result.ToString());
    //        }
    //        else
    //        {
    //            var tex = DownloadHandlerTexture.GetContent(request);
    //              = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
    //        }
    //    }
    //}
}

public class SceneLayerInterface
{
    private int layerIndex;
    private bool playing;
    public float volume;
    public LayerObject layer;

    public AudioClip[] audioList;
    public AudioSource[] sources;
    private bool sourceToggle;

    private ScenePanel scene;

    private int sequencePlaying;

    private Coroutine volumeUpDownCoroutine;
    private AudioManagementLayer manager;

    public SceneLayerInterface(ScenePanel scene, LayerObject o, int layerIndex)
    {
        this.layerIndex = layerIndex;
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

    private AudioManagementLayer GetManager()
    {
        if (manager == null)
        {
            manager = scene.manager.layers[layerIndex];
        }
        return manager;
    }

    public double playLayer(double time = -1)
    {
        playing = true;
        if (time < 0) time = AudioSettings.dspTime;
        if (layer.sequences.Count < 1)
        {
            scene.channelFinished();
            return 0;
        }
        if (sources == null)
        {
            sources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                sources[i] = scene.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
                sources[i].outputAudioMixerGroup = UIManager.instance.mixerGroup;
                sources[i].playOnAwake = false;
            }

            sources[0].loop = layer.layerType != 2 && (layer.loopState == 1 || ((layer.loopState == 0 || layer.loopState == 3 || layer.loopState == 4) && layer.sequences.Count == 1)) ? true : false;
            sources[1].loop = false;
        }

        setNextClipIndex();
        if (sequencePlaying != -1)
        {
            setVolume(layer.initialVolume);
            if (GetManager() != null) UpdateVolumeSlider();
            int i = sourceToggle ? 1 : 0;
            if (audioList[sequencePlaying] != null) sources[i].clip = audioList[sequencePlaying];
            sources[i].PlayScheduled(time);
            sourceToggle = !sourceToggle;

            if (layer.layerType == 2)
            {
                return ((double)audioList[sequencePlaying].samples / audioList[sequencePlaying].frequency);
            }
        }
        return 0;
    }

    public void stopLayer()
    {
        if (sources != null)
        {
            sources[0].Stop();
            sources[1].Stop();
        }
        playing = false;
        sequencePlaying = -1;
        sourceToggle = false;
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

    public void setVolume(float v, bool modifiedInCoroutine = false)
    {
        if (!modifiedInCoroutine && volumeUpDownCoroutine != null && Mathf.Abs(((int)(volume * 100)) / 100f - v) > .02f)
        {
            scene.StopCoroutine(volumeUpDownCoroutine);
            GetManager().silencing = false;
        }

        volume = v;
        updateVolume(true);
    }

    public void StartStopClick()
    {
        if (volumeUpDownCoroutine != null)
        {
            scene.StopCoroutine(volumeUpDownCoroutine);
            if (volume > 0 && !GetManager().silencing)
            {
                GetManager().silencing = true;
                volumeUpDownCoroutine = scene.StartCoroutine(TurnVolumeOnOrOff(0));
            }
            else
            {
                GetManager().silencing = false;
                volumeUpDownCoroutine = scene.StartCoroutine(TurnVolumeOnOrOff(layer.initialVolume));
            }
        }
        else
        {
            if (volume <= 0) volumeUpDownCoroutine = scene.StartCoroutine(TurnVolumeOnOrOff(layer.initialVolume != 0 ? layer.initialVolume : 1));
            else volumeUpDownCoroutine = scene.StartCoroutine(TurnVolumeOnOrOff(0));
        }
    }

    public void updateVolume(bool comeFromInside = false)
    { 
        if (sources != null && (layer.layerType != 2 || comeFromInside))
        {
            sources[0].volume = getVolume();
            sources[1].volume = getVolume();
        }
    }

    private void setNextClipIndex()
    {
        if (layer.sequences.Count == 0) scene.channelFinished();

        if (layer.layerType == 2)
        {
            sequencePlaying = randomWithWeight();
        }
        else
        {
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
                        double timer = layer.sequences[sequencePlaying].silenceTime + AudioSettings.dspTime;
                        if (audioList[sequencePlaying] != null) timer = ((double)audioList[sequencePlaying].samples / audioList[sequencePlaying].frequency) + AudioSettings.dspTime;
                        scene.StartCoroutine(changeOnFinish(timer));
                    }
                    break;
                case 4:
                    sequencePlaying++;
                    if (sequencePlaying >= layer.sequences.Count - 1)
                    {
                        sources[sourceToggle ? 1 : 0].loop = true;
                    }
                    else
                    {
                        double time = layer.sequences[sequencePlaying].silenceTime + AudioSettings.dspTime;
                        if (audioList[sequencePlaying] != null) time = ((double)audioList[sequencePlaying].samples / audioList[sequencePlaying].frequency) + AudioSettings.dspTime;
                        scene.StartCoroutine(changeOnFinish(time));
                    }
                    break;
                default:
                    Debug.LogError("Invalid layer LoopState");
                    break;
            }
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
        playing = false;
        sourceToggle = false;
        scene.channelFinished();
    }

    IEnumerator changeOnFinish(double time)
    {
        while (playing && AudioSettings.dspTime < time - 1)
        {
            yield return null;
        }

        if (playing) playLayer(time);
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

    private IEnumerator TurnVolumeOnOrOff(float target)
    {
        float originalTime = Time.time;
        float originalVolume = volume;
        float endTime = Time.time + 4;

        /*
        if (scene.scene.fadeOutTime > 0 && (target <= 0 || scene.scene.fadeInTime <= 0)) endTime = Time.time + scene.scene.fadeOutTime;
        else if (scene.scene.fadeOutTime > 0 && target > 0) endTime = Time.time + scene.scene.fadeInTime;
        */

        if (target < volume) GetManager().silencing = true;

        while (volume != target)
        {
            if (Time.time >= endTime) setVolume(target, true);
            else 
            {
                var val = Mathf.Lerp(originalVolume, target, (Time.time - originalTime) / (endTime - originalTime));
                setVolume(val, true);
            }
            UpdateVolumeSlider();
            yield return null;
        }

        GetManager().silencing = false;
    }

    private void UpdateVolumeSlider()
    {
        GetManager().SetSliderPosition(volume * 100);
    }
}