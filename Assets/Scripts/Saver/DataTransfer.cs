using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTransfer : MonoBehaviour
{
    public List<bool> itemRevealedList = new List<bool>();
    public List<bool> characterRevealedList = new List<bool>();
    public List<bool> levelRevealedList = new List<bool>();

    public XMLSaver  xmlSaver;
    public XMLReader xmlReader;
    
    public static DataTransfer GetDataTransfer { get; private set; }
    private void Awake()
    {
        GetDataTransfer = this;
    }
    
    private void Start()
    {
        xmlReader.LoadSaver();
        GameObject.DontDestroyOnLoad(gameObject);
    }
}
