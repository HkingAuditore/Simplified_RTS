using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScienceTreeNodeUI : MonoBehaviour
{
    public Button nodeButton;
    public ScienceTreeUI scienceTreeUI;
    public ScienceTreeNode scienceTreeNode;

    public ColorBlock selectedColor = ColorBlock.defaultColorBlock;

    private void Start()
    {
        CheckActivatable();
    }

    public void CheckActivatable()
    {
        nodeButton.interactable = scienceTreeNode.IsActivatable();
        Debug.Log("node name ["+this.gameObject.name+"] is " + scienceTreeNode.IsActivatable());
    }

    public void OnClick()
    {
        if (scienceTreeNode.IsActivatable())
        {
            if (scienceTreeNode.Activate())
            {
                Debug.Log("Acitivate node [" + this.gameObject.name + "]");
                this.nodeButton.colors = selectedColor;
                this.nodeButton.interactable = false;
                foreach (ScienceTreeNode afterwardNode in scienceTreeNode.afterwardNodes)
                {
                    scienceTreeUI.FindNodeUI(afterwardNode).CheckActivatable();
                }
            }
            
        }
    }
}
