using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoPreview5 : MonoBehaviour
{
    private static MinoPreview5 _instance;
    public static MinoPreview5 Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    private void Awake()
    {
        Instance = this;
    }
}
