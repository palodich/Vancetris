using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    private Renderer currentRenderer;

    public bool IsRowFull()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, -(transform.right), 10f, GameManger.instance.rowLayerMask);

        if (hits.Length == 10)
        {
            return true;
        }
        else return false;
    }

    public void HighlightRow()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, -(transform.right), 10f, GameManger.instance.rowLayerMask);

        for (int i = 0; i < hits.Length; i++)
        {
            currentRenderer = hits[i].collider.GetComponent<Renderer>();
            currentRenderer.material.color = new Color(1, 1, 1, 1);
        }
    }

    public void DestroyRow()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, -(transform.right), 10f, GameManger.instance.rowLayerMask);

        for (int i = 0; i < hits.Length; i++)
        {
            Destroy(hits[i].collider.gameObject);
        }
    }

    public void MoveRowDown()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, -(transform.right), 10f, GameManger.instance.rowLayerMask);

        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].collider.transform.position = new Vector3(hits[i].transform.position.x, hits[i].transform.position.y - 1, hits[i].transform.position.z);
        }
    }
}