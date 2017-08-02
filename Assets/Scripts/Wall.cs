using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private MinoMovement activeMinoMovement;
 
    private void OnTriggerEnter(Collider other)
    {
        Transform parent = other.transform.parent;
        activeMinoMovement = GameManger.instance.activeMino.GetComponent<MinoMovement>();

        if (parent != null)
        {
            switch (name)
            {
                case "LWall":
                    activeMinoMovement.MoveHorizontal(Direction.right, 1);
                    break;
                case "RWall":
                    activeMinoMovement.MoveHorizontal(Direction.left, 1);
                    break;
            }
        }
    }
}
