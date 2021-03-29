using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBarUI : MonoBehaviour
{
    public  Image         fillImage;
    public  RectTransform flag;
    public  RectTransform flagStartPos;
    public  RectTransform flagEndPos;
    private float         _fillAmount;

    public float FillAmount
    {
        get => _fillAmount;
        set => _fillAmount = value > 1 ? 1 : value;
    }

    [ContextMenu("SetFill")]
    public void SetFill()
    {
        fillImage.fillAmount  = FillAmount;
        flag.position = Vector3.Lerp(flagStartPos.position, flagEndPos.position, FillAmount);
    }


}
