using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoPiece : MonoBehaviour
{
    public bool isColliding = false;
    public Collider collidingObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9 || other.gameObject.layer == 10)
        {
            isColliding = true;
            collidingObject = other;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 9 || other.gameObject.layer == 10)
        {
            isColliding = true;
            collidingObject = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9 || other.gameObject.layer == 10)
        {
            isColliding = false;
            collidingObject = null;
        }
    }
}