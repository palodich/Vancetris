using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoPiece : MonoBehaviour
{
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != null)
        {
            StopMino(other);
        }
    }

    public static void StopMino(Collider other)
    {
        if (other.transform.parent.gameObject.layer == 8)
        {
            Rigidbody otherRb = other.GetComponentInParent<Rigidbody>();
            otherRb.velocity = Vector3.zero;
            //otherRb.isKinematic = true;
            otherRb.position = new Vector3(otherRb.position.x, (Mathf.Floor(otherRb.position.y + 1f)), otherRb.position.z);
            other.transform.parent.gameObject.layer = 9;
            GameManger.instance.activeMino = null;
        }
    }*/

}
