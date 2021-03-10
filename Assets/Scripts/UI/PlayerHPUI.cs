using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    public  Player player;
    private Image  _fr;

    private float _hpFull;

    private void Awake()
    {
        _hpFull = player.HP;
        _fr     = transform.Find("FR").GetComponent<Image>();
    }

    private void Update()
    {
        _fr.fillAmount = Mathf.Clamp(player.HP / _hpFull, 0f, 100f);
    }
}