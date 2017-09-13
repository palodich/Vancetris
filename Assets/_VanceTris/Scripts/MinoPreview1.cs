using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoPreview1 : MonoBehaviour
{
    public static MinoPreview1 instance;

    private void Awake()
    {
        instance = this;
    }
}
