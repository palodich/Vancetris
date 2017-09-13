using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoPreview2 : MonoBehaviour
{
    public static MinoPreview2 instance;

    private void Awake()
    {
        instance = this;
    }
}