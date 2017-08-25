using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    private Renderer currentRenderer;

    public void CheckRow()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, -(transform.right), 10f, GameManger.instance.minoBlockLayerMask);

        if (hits.Length == 10)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                currentRenderer = hits[i].collider.GetComponent<Renderer>();

                currentRenderer.material.color = Color.white;
            }
        }
    }
}