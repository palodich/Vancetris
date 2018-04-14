﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoPreview2 : MonoBehaviour
{
    private static MinoPreview2 _instance;
    public static MinoPreview2 Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    private void Awake()
    {
        Instance = this;
    }
}