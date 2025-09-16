using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AnotherFileBrowser.Windows;
using TMPro;

public class SceneEditor : MonoBehaviour
{
    public static SceneEditor instance;

    [SerializeField] private GameObject LayerHolder;

    [SerializeField] private GameObject LayerPrefab;

    [SerializeField] private TMP_InputField NameInputField;
    [SerializeField] private TMP_InputField FadeInInputField;
    [SerializeField] private TMP_InputField FadeOutInputField;
    [SerializeField] private Slider volumeSlider;

    private List<LayerEditor> layerEditorList;

    private string sceneImage;

    private bool editMode;
    private ScenePanel scene;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        layerEditorList = new List<LayerEditor>();
    }

    public void InitializeCreateScene()
    {
        editMode = false;
        NameInputField.text = "";
        FadeInInputField.text = "";
        FadeOutInputField.text = "4";
        volumeSlider.value = 50;
        sceneImage = "";

        for (int i = layerEditorList.Count - 1; i >= 0; i--) {
            layerEditorList[i].OnDeleteClick();
        }

        OnAddLayerClick();
    }

    public void InitializeEditScene(ScenePanel scene)
    {
        this.scene = scene;
        var editedScene = scene.scene;
        editMode = true;

        for (int i = layerEditorList.Count - 1; i >= 0; i--)
        {
            layerEditorList[i].OnDeleteClick();
        }

        NameInputField.text = editedScene.sceneName;
        FadeInInputField.text = editedScene.fadeInTime.ToString();
        FadeOutInputField.text = editedScene.fadeOutTime.ToString();
        volumeSlider.value = Mathf.FloorToInt(editedScene.initialVolume * 100);
        sceneImage = editedScene.image;

        for (int i = 0; i < editedScene.layers.Count; i++)
        {
            var layerObject = Instantiate(LayerPrefab, LayerHolder.transform);
            layerObject.transform.SetSiblingIndex(layerObject.transform.GetSiblingIndex() - 1);
            RefreshLayoutGroupsImmediateAndRecursive();

            var layer = layerObject.GetComponent<LayerEditor>();
            layerEditorList.Add(layer);
            layer.InitializeEdited(editedScene.layers[i]);
        }
    }

    public void OnSelectImageClick()
    {
        var browserProperties = new BrowserProperties();
        browserProperties.filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
        browserProperties.filterIndex = 0;

        var sceneImagePath = "";
        new FileBrowser().OpenFileBrowser(browserProperties, path =>
        {
            sceneImagePath = path;
        });

        if (sceneImagePath != "")
        {
            sceneImage = ImageManager.EncodeToJson(ImageManager.LoadTextureFromFile(sceneImagePath));
        }
        else sceneImage = "";
    }

    public void OnDeleteClick()
    {
        UIManager.instance.deleteScene(scene);
        OnCancelClick();
    }

    public void OnCancelClick()
    {
        gameObject.SetActive(false);
    }

    public void OnConfirmClick()
    {
        if (!editMode)
        {
            var scene = getSceneObject();

            UIManager.instance.addScene(scene);
        }
        else
        {
            editSceneObject();
            scene.ButtonUpdate();
        }
            
        OnCancelClick();
    }

    public void OnAddLayerClick()
    {
        var layerObject = Instantiate(LayerPrefab, LayerHolder.transform);
        layerObject.transform.SetSiblingIndex(layerObject.transform.GetSiblingIndex()-1);
        RefreshLayoutGroupsImmediateAndRecursive();

        var layer = layerObject.GetComponent<LayerEditor>();
        layerEditorList.Add(layer);
        layer.Initialize();
    }

    public void deleteLayer(LayerEditor layer)
    {
        layerEditorList.Remove(layer);
        Destroy(layer.gameObject);
        RefreshLayoutGroupsImmediateAndRecursive();
    }

    private SceneObject getSceneObject()
    {
        var layers = new List<LayerObject>();

        for (int i = 0; i < layerEditorList.Count; i++)
        {
            layers.Add(layerEditorList[i].getLayerObject());
        }

        float fin, fout;
        try { fin = float.Parse(FadeInInputField.text); } catch (System.Exception) { fin = 0; };
        try { fout = float.Parse(FadeOutInputField.text); } catch (System.Exception) { fout = 0; };

        return new SceneObject(
            NameInputField.text == "" ? "Untitled Scene" : NameInputField.text,
            volumeSlider.value / 100f,
            fin,
            fout,
            sceneImage,
            layers
        );
    }

    private void editSceneObject()
    {
        var layers = new List<LayerObject>();

        for (int i = 0; i < layerEditorList.Count; i++)
        {
            layers.Add(layerEditorList[i].getLayerObject());
        }

        float fin, fout;
        try { fin = float.Parse(FadeInInputField.text); } catch (System.Exception) { fin = 0; };
        try { fout = float.Parse(FadeOutInputField.text); } catch (System.Exception) { fout = 0; };

        var editedScene = scene.scene;
        editedScene.sceneName = NameInputField.text == "" ? "Untitled Scene" : NameInputField.text;
        editedScene.initialVolume = volumeSlider.value / 100f;
        editedScene.fadeInTime = fin;
        editedScene.fadeOutTime = fout;
        editedScene.image = sceneImage;
        editedScene.layers = layers;
    }

    public void RefreshLayoutGroupsImmediateAndRecursive()
    {
        Invoke("RefreshLayoutGroupsImplementation", 0.001f);
    }

    private void RefreshLayoutGroupsImplementation()
    {
        foreach (var layoutGroup in LayerHolder.GetComponentsInChildren<LayoutGroup>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }
    }
}