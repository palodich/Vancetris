using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoPreview4 : MonoBehaviour
{
    private static MinoPreview4 _instance;
    public static MinoPreview4 Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    private void Awake()
    {
        Instance = this;
    }
}
