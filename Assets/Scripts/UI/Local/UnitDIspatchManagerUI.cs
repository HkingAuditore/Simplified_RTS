using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;
using Photon.Pun;

public class UnitDIspatchManagerUI : MonoBehaviourPun
{
     public UnitDispatchUI[] UnitDispatchUIs;
     public Player player;

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

          foreach (var unitDispatchUI in UnitDispatchUIs)
          {
               player.SetUnits(pos,unitDispatchUI.unit.gameObject,rd,unitDispatchUI.UnitDispatchNumber);
               Debug.Log("NUMBER:" + unitDispatchUI.UnitDispatchNumber);

               unitDispatchUI.UnitDispatchNumber = 0;
               unitDispatchUI.unitNumberText.gameObject.SetActive(false);
               unitDispatchUI.unitSetIndicator.gameObject.SetActive(false);
               unitDispatchUI.unitRemoveButton.gameObject.SetActive(false);

          }
        

     }

    
    
}
