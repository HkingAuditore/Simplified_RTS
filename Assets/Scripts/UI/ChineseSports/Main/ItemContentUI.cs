﻿using UnityEngine;
using UnityEngine.UI;

namespace UI.ChineseSports.Main
{
    public class ItemContentUI : MonoBehaviour
    {
        public                                     Image       itemImage;
        [SerializeField] private                   Sprite      itemSprite;
        [SerializeField] private                   string      itemName;
        public                                     Text        itemNameText;
        [TextArea(2, 10)] [SerializeField] private string      itemContent;
        public                                     Text        itemContentText;
        public                                     AudioSource logAudioSource;
        public                                     AudioClip   logAudio;

        public string ItemName
        {
            get => itemName;
            set
            {
                itemName          = value;
                itemNameText.text = ItemName;
            }
        }

        public string ItemContent
        {
            get => itemContent;
            set
            {
                itemContent          = value;
                itemContentText.text = ItemContent;
            }
        }

        public Sprite ItemSprite
        {
            get => itemSprite;
            set
            {
                itemSprite       = value;
                itemImage.sprite = ItemSprite;
            }
        }

        private void Start()
        {
            logAudioSource.playOnAwake = false;
        }

        private void OnEnable()
        {
            logAudioSource.clip = logAudio;
            if (logAudio != null) logAudioSource.Play();
        }

        public void SetContent()
        {
            itemImage.sprite     = ItemSprite;
            itemNameText.text    = ItemName;
            itemContentText.text = ItemContent;
        }

        public void Close()
        {
            gameObject.SetActive(false);
            logAudioSource.Stop();
        }
    }
}