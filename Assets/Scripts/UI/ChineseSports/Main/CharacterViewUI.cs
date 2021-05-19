using System;
using System.Collections.Generic;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.ChineseSports.Main
{
    public class CharacterViewUI : MonoBehaviour
    {
        public List<Sprite> characterSprites = new List<Sprite>();
        [Multiline(3)]
        public List<String>  characterDescriptions = new List<string>();

        [FormerlySerializedAs("MilitaryUnits")] [SerializeField]
        public List<Unit> militaryUnits = new List<Unit>();

        [FormerlySerializedAs("PropertyLines")]
        public List<PropertyLine> propertyLines = new List<PropertyLine>();

        [FormerlySerializedAs("CharacterAvater")]
        public Image characterAvater;

        public Text descriptionText;

        public int CurCharacterSpriteIndex { get; set; }

        private void Start()
        {
            Set(1);
        }

        public void Set(int index)
        {
            characterAvater.sprite = characterSprites[index];
            descriptionText.text   = characterDescriptions[index];
            propertyLines.ForEach(l =>
                                  {
                                      Debug.Log(index);
                                      Debug.Log((IMilitaryUnit) militaryUnits[index]);
                                      l.MilitaryUnit = (IMilitaryUnit) militaryUnits[index];
                                      // Debug.Log((IMilitaryUnit)militaryUnits[index]);
                                  });
        }
    }
}