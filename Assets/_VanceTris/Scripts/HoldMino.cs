using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldMino : MonoBehaviour
{
    public static HoldMino instance;

    private void Awake()
    {
        instance = this;
    }
}
