using System;
using System.Collections;
using System.Linq;
using Units;
using UnityEngine;

public class UnitDispatchManagerUI : MonoBehaviour
{
    public UnitDispatchUI[]  UnitDispatchUIs;
    public Player.Player     player;
    public ShootCalculatorUI shootCalculatorUI;
    public bool              IsInDispatching { get; set; } = false;

    public void SetClick(int rdInt)
    {
        var        rd  = (Road) rdInt;
        var        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        var        layerMask = 1 << 11;
        Physics.Raycast(ray, out rayHit, 50f, layerMask);

        // Debug.DrawLine(ray.origin, rayHit.point);
        // Debug.Log(rayHit.point);
        var pos = new Vector3(rayHit.point.x, rayHit.point.y, rayHit.point.z + 0.3f);

        foreach (var unitDispatchUI in UnitDispatchUIs)
        {
            player.SetUnits(pos, unitDispatchUI.unitNumber, rd, unitDispatchUI.UnitDispatchNumber);
            //Debug.Log("NUMBER:" + unitDispatchUI.UnitDispatchNumber);

            unitDispatchUI.UnitDispatchNumber = 0;
            unitDispatchUI.unitNumberText.gameObject.SetActive(false);
            unitDispatchUI.unitSetIndicator.gameObject.SetActive(false);
            unitDispatchUI.unitRemoveButton.gameObject.SetActive(false);
        }
    }

    public void SetClickWithVelocity(int rdInt)
    {
        var        rd  = (Road) rdInt;
        var        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        var        layerMask = 1 << 11;
        Physics.Raycast(ray, out rayHit, 50f, layerMask);

        //初始位置
        var pos       = new Vector3(rayHit.point.x, rayHit.point.y, rayHit.point.z);
        var screenPos = Input.mousePosition;
        
        // Debug.Log("[Dispatch test]ori pos: " + pos);
        shootCalculatorUI.gameObject.SetActive(true);
        shootCalculatorUI.oriPos       = pos;
        shootCalculatorUI.oriScreenPos = screenPos;

        shootCalculatorUI.shootRigidbody = (from dispatchUI in UnitDispatchUIs
                                            where dispatchUI.unitNumber > 0
                                            select dispatchUI.unit.unitRigidbody).First();
        Debug.Log(shootCalculatorUI.shootRigidbody);
        StartCoroutine(WaitForVelocity(pos,screenPos, rdInt));
    }

    private IEnumerator WaitForVelocity(Vector3 oriPos,Vector3 oriScreenPos, int rdInt)
    {
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        var     rd          = (Road) rdInt;
        var     newPos      = Input.mousePosition;
        Vector3 oriVelocity = -(newPos - oriScreenPos) * .1f;
        oriVelocity = new Vector3(oriVelocity.x, 0, oriVelocity.y);
        // Debug.DrawLine(oriPos, oriPos+oriVelocity,Color.cyan,60f);
        foreach (var unitDispatchUI in UnitDispatchUIs)
        {
            try
            {
                player.SetUnits(oriPos, unitDispatchUI.unitNumber, rd, unitDispatchUI.UnitDispatchNumber, oriVelocity);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                unitDispatchUI.UnitDispatchNumber = 0;
                unitDispatchUI.unitNumberText?.gameObject.SetActive(false);
                unitDispatchUI.unitSetIndicator.gameObject.SetActive(false);
                unitDispatchUI.unitRemoveButton.gameObject.SetActive(false);
                unitDispatchUI.UnitSelectStack.Clear();
                unitDispatchUI.spriteIndicatorUI.gameObject.SetActive(false);

            }
        }

        this.IsInDispatching = false;
        
        shootCalculatorUI.gameObject.SetActive(false);
    }
}