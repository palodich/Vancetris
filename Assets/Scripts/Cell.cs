using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool IsFull()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;

        //Debug.DrawRay(transform.position, forward * 7, Color.green);

        if (Physics.Raycast(transform.position, forward, out hit, 100))
            return true;
        else return false;
    }
}