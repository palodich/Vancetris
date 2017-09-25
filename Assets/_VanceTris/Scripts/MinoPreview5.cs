using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoPreview5 : MonoBehaviour
{
    public static MinoPreview5 instance;

    private void Awake()
    {
        instance = this;
    }
}
