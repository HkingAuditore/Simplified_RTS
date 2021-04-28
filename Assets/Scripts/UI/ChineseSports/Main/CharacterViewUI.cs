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

        [FormerlySerializedAs("MilitaryUnits")] [SerializeField]
        public List<Unit> militaryUnits = new List<Unit>();

        [FormerlySerializedAs("PropertyLines")]
        public List<PropertyLine> propertyLines = new List<PropertyLine>();

        [FormerlySerializedAs("CharacterAvater")]
        public Image characterAvater;

        public int CurCharacterSpriteIndex { get; set; }

        private void Start()
        {
            Set(1);
        }

        public void Set(int index)
        {
            characterAvater.sprite = characterSprites[index];
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