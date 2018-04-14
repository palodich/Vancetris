using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoPreview1 : MonoBehaviour
{
    private static MinoPreview1 _instance;
    public static MinoPreview1 Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    private void Awake()
    {
        Instance = this;
    }
}
