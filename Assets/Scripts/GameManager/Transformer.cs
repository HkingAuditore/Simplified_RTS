﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformer : MonoBehaviour
{
    public         TutorialManager tutorialManager;
    private static Transformer     _transformer;

    public static Transformer getTransformer => _transformer;

    public bool isSoundsActive;

    void Awake () {
        _transformer = this;
    }
}
