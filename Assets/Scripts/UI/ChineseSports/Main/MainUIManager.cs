using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    public GameObject itemUI;
    public ScrollUI scrollUI;

    public void ShowItemUI()
    {
        itemUI.SetActive(true);
    }

    public void CloseItemUI()
    {
        itemUI.SetActive(false);
    }
    
    public void ShowScrollUI()
    {
        scrollUI.gameObject.SetActive(true);
    }

    public void CloseScrollUI()
    {
        scrollUI.Close();
    }
}
