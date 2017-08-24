using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoPreview3 : MonoBehaviour {
    public static MinoPreview3 instance;

    private void Awake()
    {
        instance = this;
    }
}
