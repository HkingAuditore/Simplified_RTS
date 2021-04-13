using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemContentUI : MonoBehaviour
{
    public                                     Image  itemImage;
    [SerializeField] private                   Sprite itemSprite;
    [SerializeField] private                   string itemName;
    public                                     Text   itemNameText;
    [TextArea(2, 10)] [SerializeField] private string itemContent;
    public                                     Text   itemContentText;

    public string ItemName
    {
        get => itemName;
        set
        {
            itemName          = value;
            itemNameText.name = ItemName;

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

    public void SetContent()
    {
        itemImage.sprite     = ItemSprite;
        itemNameText.name    = ItemName;
        itemContentText.text = ItemContent;
    }
}
