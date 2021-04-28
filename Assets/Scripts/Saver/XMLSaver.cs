using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Saver
{
    public class XMLSaver : MonoBehaviour
    {
        /// <summary>
        ///     自动保存时间
        /// </summary>
        public float autoSaveTime;

        private          DataTransfer _dataTransfer;
        private readonly XmlDocument  _xmlDoc = new XmlDocument();


        private void Start()
        {
            _dataTransfer = DataTransfer.GetDataTransfer;
            StartCoroutine(WaitForSave());
        }

        private XmlDocument GetDataXmlDocument()
        {
            _xmlDoc.RemoveAll();
            var saver = _xmlDoc.CreateElement("Saver");
            saver.AppendChild(ConvertListToXmlElement(_dataTransfer.itemRevealedList,      "ItemRevealedList",      "Item"));
            saver.AppendChild(ConvertListToXmlElement(_dataTransfer.characterRevealedList, "CharacterRevealedList", "Character"));
            saver.AppendChild(ConvertListToXmlElement(_dataTransfer.levelRevealedList,     "LevelRevealedList",     "Level"));
            saver.AppendChild(ConvertNumberToXmlElement(_dataTransfer.tutorialManager.NextTutorialIndex, "NextTutorialIndex"));
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

        private XmlElement ConvertNumberToXmlElement<T>(T number, string elementName) where T : IComparable<T>
        {
            var element = _xmlDoc.CreateElement(elementName);
            element.SetAttribute("Content", number.ToString());

            return element;
        }

        private void SaveDataFile(XmlDocument doc, string fileName)
        {
            var path = XMLDataBase.XMLPath + fileName + ".xml";
            doc.Save(path);
            // AssetDatabase.Refresh();
        }

        /// <summary>
        ///     存档
        /// </summary>
        /// <param name="fileName">文件名</param>
        public void SaveData(string fileName)
        {
            SaveDataFile(GetDataXmlDocument(), fileName);
        }

        /// <summary>
        ///     存档
        /// </summary>
        public void SaveData()
        {
            StartCoroutine(WaitForSaveDone());
            // SaveData(XMLDataBase.DataName);
            Debug.Log("Save!");
        }

        private IEnumerator WaitForSaveDone()
        {
            SaveData(XMLDataBase.DataName);
            yield return null;
        }

        /// <summary>
        ///     自动存档
        /// </summary>
        private void AutoSave()
        {
            SaveData();
            StartCoroutine(WaitForSave());
        }


        private IEnumerator WaitForSave()
        {
            yield return new WaitForSeconds(autoSaveTime);
            AutoSave();
        }
    }
}