using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoSpawner : MonoBehaviour
{
    public static MinoSpawner instance;

    private void Awake()
    {
        instance = this;
    }
}
