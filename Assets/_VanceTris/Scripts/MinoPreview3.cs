using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoPreview3 : MonoBehaviour
{
    private static MinoPreview3 _instance;
    public static MinoPreview3 Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    private void Awake()
    {
        Instance = this;
    }
}
