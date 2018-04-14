using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldMinoPreview : MonoBehaviour
{
    private static HoldMinoPreview _instance;
    public static HoldMinoPreview Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    private void Awake()
    {
        Instance = this;
    }
}
