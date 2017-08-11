using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoPiece : MonoBehaviour
{
    private MinoMovement activeMinoMovement;

    /*
    private void OnTriggerEnter(Collider other)
    {
        Transform parent = other.transform.parent;
        activeMinoMovement = GameManger.instance.activeMino.GetComponent<MinoMovement>();

        if (parent != null)
        {
            if (parent.gameObject.layer == 8)
            {
                GameManger.instance.moveUpCounter++;
            }
        }
    }*/
}
