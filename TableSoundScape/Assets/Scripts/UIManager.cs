using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Globalization;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    [SerializeField] private AudioMixer mixer;
    [SerializeField] public AudioMixerGroup mixerGroup;

    [SerializeField] public GameObject SideViewHolder;
    [SerializeField] private GameObject GridViewContentHolder;
    [SerializeField] private Button SaveSceneButton;
    [SerializeField] private Button SaveSceneAsButton;
    [SerializeField] private Slider masterVolumeSlider;

    [SerializeField] private SceneEditor sceneEditor;

    [SerializeField] private GameObject scenePanelPrefab;


    private List<SceneContainer> sceneContainers;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        CultureInfo.CurrentCulture = new CultureInfo("en-US");
        CultureInfo.CurrentUICulture = new CultureInfo("en-US");
    }

    private void Start()
    {
        sceneContainers = new List<SceneContainer>();
        sceneContainers.Add(new SceneContainer(""));

        setMasterVolume(66);

        masterVolumeSlider.onValueChanged.AddListener(setMasterVolume);
    }

    private void setMasterVolume(float value)
    {
        mixer.SetFloat("MIXER_MASTER", Mathf.Log10(value / 100f + 0.0001f) * 20);
    }

    public void OnAddSceneClick()
    {
        sceneEditor.gameObject.SetActive(true);
        sceneEditor.InitializeCreateScene();
    }

    public void editScene(ScenePanel scene)
    {
        sceneEditor.gameObject.SetActive(true);
        sceneEditor.InitializeEditScene(scene);
    }

    public void OnLoadSceneClick()
    {

    }

    public void OnSaveSceneClick()
    {

    }

    public void OnSaveSceneAsClick()
    {

    }

    public void addScene(SceneObject scene)
    {
        var o = Instantiate(scenePanelPrefab, GridViewContentHolder.transform);
        var scenePanel = o.GetComponent<ScenePanel>();
        scenePanel.scene = scene;
        scenePanel.ButtonUpdate();
        sceneContainers[0].scenes.Add(scenePanel);
    }

    public void deleteScene(ScenePanel scene)
    {
        foreach (var container in sceneContainers)
        {
            if (container.scenes.Contains(scene))
            {
                container.scenes.Remove(scene);
                Destroy(scene.gameObject);
                return;
            }
        }
    }

    public void RefreshLayoutGroupsImmediateAndRecursive()
    {
        Invoke("RefreshLayoutGroupsImplementation", 0.001f);
    }

    private void RefreshLayoutGroupsImplementation()
    {
        foreach (var layoutGroup in SideViewHolder.GetComponentsInChildren<LayoutGroup>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }
    }

    public void changePositionOfScene()
    {

    }
}

public class SceneContainer
{
    public string containerName;
    public float r;
    public float g;
    public float b;
    public List<ScenePanel> scenes;

    public SceneContainer(string name, float r, float g, float b)
    {
        this.containerName = name;
        this.r = r;
        this.g = g;
        this.b = b;
        scenes = new List<ScenePanel>();
    }

    public SceneContainer(string name) : this(name, 0.5843138f, 0.5843138f, 0.5843138f) {}
}