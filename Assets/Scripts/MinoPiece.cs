using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoPiece : MonoBehaviour
{
    private MinoMovement activeMinoMovement;
    private MeshRenderer currentMeshRenderer;
    private MeshRenderer otherMeshRenderer;

    public bool isColliding = false;
    public Collider collidingObject;

    private void OnTriggerEnter(Collider other)
    {
        isColliding = true;
        collidingObject = other;
    }

    private void OnTriggerStay(Collider other)
    {
        currentMeshRenderer = gameObject.GetComponent<MeshRenderer>();
        otherMeshRenderer = other.gameObject.GetComponent<MeshRenderer>();

        //Debug.Log(gameObject.name + " (MR " + currentMeshRenderer.enabled + ") collided with " + other.name + " (MR " + otherMeshRenderer.enabled + ")");
        /*
        if (currentMeshRenderer.enabled)
        {
            Debug.Log(gameObject.name + " (MR " + currentMeshRenderer.enabled + ") collided with " + other.name + " (MR " + otherMeshRenderer.enabled + ")");
        }

        /*Transform parent = other.transform.parent;
        activeMinoMovement = GameManger.instance.activeMino.GetComponent<MinoMovement>();

        if (parent != null)
        {
            if (parent.gameObject.layer == 8)
            {

            }
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        isColliding = false;
        collidingObject = null;
    }
}
