using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Transform parent = other.transform.parent;

        if (parent != null)
        {
            switch (name)
            {
                case "LWall":
                    Debug.Log("Move right!");
                    break;
                case "RWall":
                    Debug.Log("Move left!");
                    break;
            }
        }


        //Debug.Log(name + " collided with " + other);
    }
}
