using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataTransfer : MonoBehaviour
{
    public TutorialManager tutorialManager;

    public List<bool> itemRevealedList      = new List<bool>();
    public List<bool> characterRevealedList = new List<bool>();
    public List<bool> levelRevealedList     = new List<bool>();

    public XMLSaver  xmlSaver;
    public XMLReader xmlReader;

    public string nextLoadingSceneName;
    
    public static DataTransfer GetDataTransfer { get; private set; }
    private void Awake()
    {
        GetDataTransfer = this;
        GameObject.DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        xmlReader.LoadSaver(true);


    }

    public void LoadSceneInLoadingScene(string sceneName)
    {
        this.nextLoadingSceneName = sceneName;
        SceneManager.LoadSceneAsync("Loading");
    }
}
