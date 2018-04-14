using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoSpawner : MonoBehaviour
{
    private static MinoSpawner _instance;
    public static MinoSpawner Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    private void Awake()
    {
        Instance = this;
    }
}
