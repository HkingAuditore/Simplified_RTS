using UnityEngine;
using UnityEngine.UI;

public class ScienceTreeNodeUI : MonoBehaviour
{
    public Button          nodeButton;
    public ScienceTreeUI   scienceTreeUI;
    public ScienceTreeNode scienceTreeNode;

    public ColorBlock selectedColor = ColorBlock.defaultColorBlock;

    private void Start()
    {
        CheckActivatable();
    }

    public void CheckActivatable()
    {
        nodeButton.interactable = scienceTreeNode.IsActivatable();
        Debug.Log("node name [" + gameObject.name + "] is " + scienceTreeNode.IsActivatable());
    }

    public void OnClick()
    {
        if (scienceTreeNode.IsActivatable())
            if (scienceTreeNode.Activate())
            {
                Debug.Log("Acitivate node [" + gameObject.name + "]");
                nodeButton.colors       = selectedColor;
                nodeButton.interactable = false;
                foreach (var afterwardNode in scienceTreeNode.afterwardNodes)
                    scienceTreeUI.FindNodeUI(afterwardNode).CheckActivatable();
            }
    }
}