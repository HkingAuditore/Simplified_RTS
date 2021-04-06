using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformer : MonoBehaviour
{
    private static Transformer transformer;

    public static Transformer getTransformer => transformer;

    public bool isSoundsActive;

    void Awake () {
        transformer = this;
    }
}
