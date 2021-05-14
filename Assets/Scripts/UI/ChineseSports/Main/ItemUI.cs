using System;
using Saver;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ChineseSports.Main
{
    public enum ObjectType
    {
        Item,
        Character
    }

    [ExecuteInEditMode]
    public class ItemUI : MonoBehaviour
    {
        public Sprite        itemSprite;
        public Image         itemImage;
        public Text          itemNameText;
        public Color         unrevealedColor;
        public Button        button;
        public ItemContentUI itemContentUI;
        public GameObject    unrevealedGameObject;
        public ObjectType    objectType;
        public int           itemIndex;
        public AudioClip     logAudio;

        [SerializeField] [ContextMenuItem("SetName", "Set")]
        private string itemName;

        [SerializeField] [TextArea(2, 10)] private string itemContent;


        [SerializeField] [ContextMenuItem("SetReveal", "Set")]
        private bool _isRevealed;


        public bool IsRevealed
        {
            get => _isRevealed;
            set
            {
                _isRevealed     = value;
                itemImage.color = value ? Color.white : unrevealedColor;
                if (unrevealedGameObject != null) unrevealedGameObject.SetActive(!value);
                Set();
            }
        }

        public string ItemName
        {
            get => itemName;
            set
            {
                itemName          = value;
                itemNameText.text = _isRevealed ? value : "???";
            }
        }

        private void Start()
        {
            switch (objectType)
            {
                case ObjectType.Item:
                    IsRevealed = DataTransfer.GetDataTransfer.itemRevealedList[itemIndex];

                    break;
                case ObjectType.Character:
                    IsRevealed = DataTransfer.GetDataTransfer.characterUnlockedList[itemIndex];

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Set();
        }

        [ContextMenu("Set")]
        public void Set()
        {
            itemImage.sprite = itemSprite;
            if (button != null) button.interactable = _isRevealed;
            itemImage.color   = _isRevealed ? Color.white : unrevealedColor;
            itemNameText.text = _isRevealed ? itemName : "???";
            if (unrevealedGameObject != null) unrevealedGameObject.SetActive(!IsRevealed);
        }

        public void ShowContent()
        {
            itemContentUI.ItemSprite  = itemSprite;
            itemContentUI.ItemName    = ItemName;
            itemContentUI.logAudio    = logAudio;
            itemContentUI.ItemContent = itemContent;
            itemContentUI.gameObject.SetActive(true);
        }
    }
}