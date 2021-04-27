using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using StaticClasses;
using UnityEditor;
using UnityEngine;

public class XMLReader : MonoBehaviour
{
    private        DataTransfer _dataTransfer;

    
    private void Start()
    {
        _dataTransfer = DataTransfer.GetDataTransfer;
    }

    private XmlDocument LoadXml(string fileName)
    {
        var path = XMLDataBase.XMLPath + fileName + ".xml";
        if (File.Exists(path))
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            // AssetDatabase.Refresh();
            return xmlDoc;
        }

        throw new Exception(path + "文件不存在");
    }

    private List<bool> ConvertXmlElementToBoolList(XmlElement xmlElement,string partName)
    {
        XmlNodeList nodes    = xmlElement.SelectNodes(partName);
        List<bool> nodeList = nodes.Cast<XmlNode>()
                                   .OrderBy(node => int.Parse(node.Attributes?["Index"].Value ?? throw new InvalidOperationException()))
                                   .Select(node => bool.Parse(node.Attributes?["IsRevealed"].Value ?? throw new InvalidOperationException()))
                                   .ToList();
        return nodeList;
    }

 
    private T ConvertXmlElementToNumber<T>(XmlElement xmlElement) where T : IComparable<T>
    {
        T number =  xmlElement.Attributes["Content"].Value.ChangeType<T>();
        return number;
    }

    private void LoadSaver(string fileName)
    {
        XmlDocument doc = LoadXml(fileName);
        _dataTransfer.characterRevealedList = ConvertXmlElementToBoolList(doc.DocumentElement?["CharacterRevealedList"], "Character");
        _dataTransfer.itemRevealedList = ConvertXmlElementToBoolList(doc.DocumentElement?["ItemRevealedList"], "Item");
        _dataTransfer.levelRevealedList = ConvertXmlElementToBoolList(doc.DocumentElement?["LevelRevealedList"], "Level");
    }

    private void LoadSaverCompletely(string fileName)
    {
        XmlDocument doc = LoadXml(fileName);
        _dataTransfer.characterRevealedList = ConvertXmlElementToBoolList(doc.DocumentElement?["CharacterRevealedList"], "Character");
        _dataTransfer.itemRevealedList = ConvertXmlElementToBoolList(doc.DocumentElement?["ItemRevealedList"], "Item");
        _dataTransfer.levelRevealedList = ConvertXmlElementToBoolList(doc.DocumentElement?["LevelRevealedList"], "Level");
        _dataTransfer.tutorialManager.NextTutorialIndex =
            ConvertXmlElementToNumber<int>(doc.DocumentElement?["NextTutorialIndex"]);
    }

    public void LoadSaver(bool isCompletely)
    {
        if(isCompletely)
            LoadSaverCompletely(XMLDataBase.DataName);
        else
            LoadSaver(XMLDataBase.DataName);
    }
}
