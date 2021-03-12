using Units;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class SpriteRendererPlus : MonoBehaviour
{
    public  bool isUnit;
    private int  _isAside;
    private Unit _parentUnit;

    private void Start()
    {
        if (isUnit)
        {
            _parentUnit = transform.parent.GetComponent<Unit>();
            _isAside    = LayerMask.LayerToName(transform.parent.gameObject.layer) == "ASide" ? 1 : -1;
        }

        GetComponent<SpriteRenderer>().shadowCastingMode = ShadowCastingMode.On;
    }

    private void Update()
    {
        if (isUnit)
        {
            var isForward = _parentUnit.unitRigidbody.velocity.x < 0 ? 1 : -1;
            // Debug.Log(isForward);
            transform.eulerAngles =
                new Vector3(0,
                            isForward * 180 * (_isAside == 1 ? 0 : -1) - transform.parent.transform.eulerAngles.y +
                            _isAside * 90f, 0);
        }
    }
}