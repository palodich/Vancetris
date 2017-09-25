using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoPreview4 : MonoBehaviour
{
    public static MinoPreview4 instance;

    private void Awake()
    {
        instance = this;
    }
}
