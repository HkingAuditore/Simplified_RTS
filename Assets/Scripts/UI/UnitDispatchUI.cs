using System;
using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;
using UnityEngine.UI;

public class UnitDispatchUI : MonoBehaviour
{
    public Text unitNumberText;
    public Button unitRemoveButton;
    public GameObject unitSetIndicator;
    public Unit unit;
    public Player player;

    private int _unitDispatchNumber = 0;
    private Stack<bool> _unitSelectStack = new Stack<bool>();

    public void AddClick(bool isFoodAndWood)
    {
        // 尝试分配资源
        if (isFoodAndWood)
        {
            try
            {
                this.player.ChangeResource(Resource.Food,-unit.costFood);
                this.player.ChangeResource(Resource.Wood,-unit.costWood);
                
            }
            catch (ResourceRunOutException e)
            {
                if (e.resource == Resource.Wood)
                {
                    this.player.ChangeResource(Resource.Food,unit.costFood);
                }

                return;
            }

        }
        else
        {
            try
            {
                this.player.ChangeResource(Resource.Gold,-unit.costGold);
                
            }
            catch (ResourceRunOutException)
            {
                return;
            }
        }
        _unitDispatchNumber++;
        unitNumberText.gameObject.SetActive(true);
        unitNumberText.text = _unitDispatchNumber.ToString();
        unitSetIndicator.gameObject.SetActive(true);
        unitRemoveButton.gameObject.SetActive(true);
        
        _unitSelectStack.Push(isFoodAndWood);
    }

    public void RemoveClick()
    {
        _unitDispatchNumber--;
        unitNumberText.text = _unitDispatchNumber.ToString();

        if (_unitSelectStack.Pop())
        {
            this.player.ChangeResource(Resource.Food,unit.costFood);
            this.player.ChangeResource(Resource.Wood,unit.costWood);
        }
        else
        {
            this.player.ChangeResource(Resource.Gold,unit.costGold);
        }
        
        if (_unitDispatchNumber <= 0)
        {
            unitNumberText.gameObject.SetActive(false);
            unitSetIndicator.gameObject.SetActive(false);
            unitRemoveButton.gameObject.SetActive(false);
        }

    }

    public void SetClick(int rdInt)
    {
        Road rd = (Road) rdInt;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        Physics.Raycast(ray, out rayHit);
        
        Debug.DrawLine(ray.origin, rayHit.point);
        // Debug.Log(rayHit.point);
        Vector3 pos = new Vector3(rayHit.point.x,rayHit.point.y,rayHit.point.z+0.3f);
        // Debug.Log("HIT POINT:" + pos);
        
        
        player.SetUnits(pos,unit.gameObject,rd,_unitDispatchNumber);

        _unitDispatchNumber = 0;
        unitNumberText.gameObject.SetActive(false);
        unitSetIndicator.gameObject.SetActive(false);
        unitRemoveButton.gameObject.SetActive(false);

    }
}
