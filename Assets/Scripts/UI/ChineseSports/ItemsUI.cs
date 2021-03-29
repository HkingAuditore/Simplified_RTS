using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsUI : MonoBehaviour
{
    public GameObject itemUI;

    public void ShowItemUI()
    {
        itemUI.SetActive(true);
    }

    public void CloseItemUI()
    {
        itemUI.SetActive(false);
    }
}
