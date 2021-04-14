using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class XMLSaver : MonoBehaviour
{
    public  float        autoSaveTime;
    private DataTransfer _dataTransfer;
    private XmlDocument  _xmlDoc = new XmlDocument();
    



    private void Start()
    {
        _dataTransfer = DataTransfer.GetDataTransfer;
        StartCoroutine(WaitForSave());
    }

    public XmlDocument GetDataXmlDocument()
    {
        _xmlDoc.RemoveAll();
        var saver = _xmlDoc.CreateElement("Saver");
        saver.AppendChild(ConvertListToXmlElement<bool>(_dataTransfer.itemRevealedList, "ItemRevealedList", "Item"));
        saver.AppendChild(ConvertListToXmlElement<bool>(_dataTransfer.characterRevealedList, "CharacterRevealedList", "Character"));
        saver.AppendChild(ConvertListToXmlElement<bool>(_dataTransfer.levelRevealedList, "LevelRevealedList", "Level"));
        _xmlDoc.AppendChild(saver);
        return _xmlDoc;
    }

    private XmlElement ConvertListToXmlElement<T>(List<T> list, string elementName, string partName)
    {
        var element = _xmlDoc.CreateElement(elementName);
        for (var i = 0; i < list.Count; i++)
        {
            var b    = list[i];
            var bXml = _xmlDoc.CreateElement(partName);
            bXml.SetAttribute("Index",      i.ToString());
            bXml.SetAttribute("IsRevealed", b.ToString());
            element.AppendChild(bXml);
        }

        return element;
    }

    private void SaveDataFile(XmlDocument doc, string fileName)
    {
        var path = XMLDataBase.XMLPath + fileName + ".xml";
        doc.Save(path);
        AssetDatabase.Refresh();
    }

    public void SaveData(string fileName )
    {
        SaveDataFile(GetDataXmlDocument(),fileName);
    }
    
    public void SaveData()
    {

        SaveData(XMLDataBase.DataName);
        Debug.Log("Save!");
    }

    public void AutoSave()
    {
        SaveData();
        StartCoroutine(WaitForSave());
    }
    

    IEnumerator WaitForSave()
    {
        yield return new WaitForSeconds(autoSaveTime);
        AutoSave();
    }
}
