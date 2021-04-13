using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
   
   public int    itemIndex;
   
   [SerializeField]
   [ContextMenuItem("SetName","Set")]
   private string itemName;
   
   [SerializeField]
   [TextArea(2,10)]
   private string itemContent;
   
   
   [SerializeField]
   [ContextMenuItem("SetReveal", "Set")]
   private bool   _isRevealed;

   
   public bool IsRevealed
   {
      get => _isRevealed;
      set
      {
         _isRevealed     = value;
         itemImage.color = value ? Color.white : unrevealedColor;
         if(unrevealedGameObject != null)unrevealedGameObject.SetActive(!value);
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

   private void Awake()
   {
      Set();
   }

[ContextMenu("Set")]
   public void Set()
   {
      itemImage.sprite = this.itemSprite;
      if (button != null)
      {
         button.interactable = _isRevealed;
      }
      itemImage.color   = _isRevealed ? Color.white : unrevealedColor;
      itemNameText.text = _isRevealed ? itemName : "???";
      if(unrevealedGameObject != null)unrevealedGameObject.SetActive(!IsRevealed);
   }

   public void ShowContent()
   {
      itemContentUI.ItemSprite  = this.itemSprite;
      itemContentUI.ItemName    = this.ItemName;
      itemContentUI.ItemContent = this.itemContent;
      itemContentUI.gameObject.SetActive(true);

   }
}
