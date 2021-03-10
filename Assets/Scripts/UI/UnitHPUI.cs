using Units;
using UnityEngine;
using UnityEngine.UI;

public class UnitHPUI : MonoBehaviour
{
    public  Unit  unit;
    private Image _fr;

    private float _hpFull;

    private void Awake()
    {
        _hpFull = unit.HP;
        _fr     = transform.Find("FR").GetComponent<Image>();
    }

    private void Update()
    {
        _fr.fillAmount = Mathf.Clamp(unit.HP / _hpFull, 0f, 100f);
    }
}