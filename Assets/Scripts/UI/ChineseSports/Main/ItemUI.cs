using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ItemUI : MonoBehaviour
{
   public Image itemImage;
   public Text  itemNameText;
   public Color unrevealedColor;
   
   [SerializeField]
   [ContextMenuItem("SetName","Set")]
   private string itemName;
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


   private void Set()
   {
      itemImage.color   = _isRevealed ? Color.white : unrevealedColor;
      itemNameText.text = _isRevealed ? itemName : "???";
   }
}
