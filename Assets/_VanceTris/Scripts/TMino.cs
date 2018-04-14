using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMino : MonoBehaviour
{
    /* We need these GameObjects so we can better tell when a t-spin has occurred.
     * Each orientation has it's corners defined. The "front" of the T mino is
     * the block that extends outward */

    [SerializeField] private GameObject flatFrontLeftCorner;
    [SerializeField] private GameObject flatFrontRightCorner;
    [SerializeField] private GameObject flatBackLeftCorner;
    [SerializeField] private GameObject flatBackRightCorner;

    [SerializeField] private GameObject leftFrontLeftCorner;
    [SerializeField] private GameObject leftFrontRightCorner;
    [SerializeField] private GameObject leftBackLeftCorner;
    [SerializeField] private GameObject leftBackRightCorner;

    [SerializeField] private GameObject rightFrontLeftCorner;
    [SerializeField] private GameObject rightFrontRightCorner;
    [SerializeField] private GameObject rightBackLeftCorner;
    [SerializeField] private GameObject rightBackRightCorner;

    [SerializeField] private GameObject flippedFrontLeftCorner;
    [SerializeField] private GameObject flippedFrontRightCorner;
    [SerializeField] private GameObject flippedBackLeftCorner;
    [SerializeField] private GameObject flippedBackRightCorner;

    public void CheckTSpin(Orientation orientation)
    {
        Vector3 cornerPos;
        Vector3 overlapBoxSize = new Vector3(0.25f, 0.25f, 0.25f);
        GameObject[] corners = new GameObject[0];
        Collider[] cornerColliders = new Collider[0];

        switch (orientation)
        {
            case Orientation.flat:
                corners = new GameObject[] { flatFrontLeftCorner, flatFrontRightCorner, flatBackLeftCorner, flatBackRightCorner };
                break;

            case Orientation.left:
                corners = new GameObject[] { leftFrontLeftCorner, leftFrontRightCorner, leftBackLeftCorner, leftBackRightCorner };
                break;

            case Orientation.right:
                corners = new GameObject[] { rightFrontLeftCorner, rightFrontRightCorner, rightBackLeftCorner, rightBackRightCorner };
                break;

            case Orientation.flipped:
                corners = new GameObject[] { flippedFrontLeftCorner, flippedFrontRightCorner, flippedBackLeftCorner, flippedBackRightCorner };
                break;
        }


        for (int i = 0; i < corners.Length; i++)
        {
            cornerPos = corners[i].transform.position;

            cornerColliders = Physics.OverlapBox(cornerPos, overlapBoxSize, Quaternion.identity, GameManger.Instance.MinoBlockLayerMask);

            if (cornerColliders.Length > 0)
            {
                Debug.Log(corners[i].name + " collided");
            }
        }
        /*
        for (int i = 0; i < cornerColliders.Length; i++)
        {
            Debug.Log(cornerColliders[i].transform.name);
        }*/

    }
}
