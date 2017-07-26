using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinoType
{
    iMino,
    jMino,
    lMino,
    oMino,
    sMino,
    tMino,
    zMino
}

public enum MinoOrientation
{
    left,
    flat,
    right,
    flipped
}

public class MinoBlock : MonoBehaviour
{
    public MinoType activeMinoType = MinoType.iMino;
    public MinoOrientation activeMinoOrientation = MinoOrientation.flat;

    public void SetMinoOrientation(MinoOrientation orientation)
    {

    }
}