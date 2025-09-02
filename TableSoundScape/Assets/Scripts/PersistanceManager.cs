using System.Collections.Generic;
using System.IO;
using AnotherFileBrowser.Windows;
using UnityEngine;

public class PersistanceManager : MonoBehaviour
{
    private string filePath = "";

    public void SaveData()
    {
        if (filePath == "") return;
        SaveAllData();
    }

    public void SaveDataAs()
    {
        var browserProperties = new BrowserProperties();
        browserProperties.filter = "Soundscape file (*.tss) | *.tss";
        browserProperties.filterIndex = 0;

        string p = "";

        new FileBrowser().SaveFileBrowser(browserProperties, "TableSoundScapeFile", ".tss", path => { p = path; });
        if (p == "") return;
        filePath = p;

        SaveData();
    }

    public void LoadData()
    {
        var browserProperties = new BrowserProperties();
        browserProperties.filter = "Soundscape file (*.tss) | *.tss";
        browserProperties.filterIndex = 0;

        string p = "";
        new FileBrowser().OpenFileBrowser(browserProperties, path => { p = path; });
        if (p == "") return;
        filePath = p;

        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) { return; }
        UIManager.instance.EnableSaveButton();
        string data = File.ReadAllText(filePath);
        PersistanceContainer container = JsonUtility.FromJson<PersistanceContainer>(data);

        UIManager.instance.ClearAll();

        foreach (var scene in container.unsorted) { UIManager.instance.addScene(scene); }
        foreach (var folder in container.folders)
        {
            var folderController = UIManager.instance.AddFolder(folder.name, new Color(folder.R, folder.G, folder.B));
            foreach (var scene in folder.scenes) { folderController.AddScene(scene); }
        }
    }

    private void SaveAllData()
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) { return; }
        UIManager.instance.EnableSaveButton();

        PersistanceContainer container = new PersistanceContainer();

        var scenes = UIManager.instance.unsortedContentHolder.GetComponentsInChildren<ScenePanel>(true);
        foreach (var scene in scenes) { container.unsorted.Add(scene.scene); }

        var folders = UIManager.instance.GridViewContentHolder.GetComponentsInChildren<FolderController>(true);
        foreach (var folder in folders)
        {
            FolderObject folderObject = new FolderObject();
            folderObject.name = folder.text.text;
            folderObject.R = folder.colorImage.color.r;
            folderObject.G = folder.colorImage.color.g;
            folderObject.B = folder.colorImage.color.b;
            var folderScenes = folder.content.GetComponentsInChildren<ScenePanel>(true);
            foreach (var scene in folderScenes) { folderObject.scenes.Add(scene.scene); }
            container.folders.Add(folderObject);
        }

        string data = JsonUtility.ToJson(container, true);
        File.WriteAllText(filePath, data);
    }


    [System.Serializable]
    public class PersistanceContainer
    {
        public List<SceneObject> unsorted;
        public List<FolderObject> folders;

        public PersistanceContainer()
        {
            unsorted = new List<SceneObject>();
            folders = new List<FolderObject>();
        }
    }

    [System.Serializable]
    public class FolderObject
    {
        public string name;
        public float R;
        public float G;
        public float B;
        public List<SceneObject> scenes;

        public FolderObject()
        {
            scenes = new List<SceneObject>();
        }
    }

}

