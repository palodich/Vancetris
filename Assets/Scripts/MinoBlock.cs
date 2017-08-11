using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinoType
{
    iMino, jMino, lMino, oMino, sMino, tMino, zMino
}

public enum MinoOrientation
{
    flat, left, right, flipped
}

public enum Direction
{
    left, right
}

public class MinoBlock : MonoBehaviour
{
    public MinoType activeMinoType;
    public MinoOrientation activeMinoOrientation;
    public GameObject[] flatPieces;
    public GameObject[] leftPieces;
    public GameObject[] rightPieces;
    public GameObject[] flippedPieces;
    private MinoBlock currentMinoBlock;
    private Vector3 vectorDirection;

    public bool CheckBelow()
    {
        GameObject[] minoPieces = null;
        currentMinoBlock = gameObject.GetComponent<MinoBlock>();

        switch (currentMinoBlock.activeMinoOrientation)
        {
            case MinoOrientation.flat:
                minoPieces = currentMinoBlock.flatPieces;
                break;
            case MinoOrientation.flipped:
                minoPieces = currentMinoBlock.flippedPieces;
                break;
            case MinoOrientation.left:
                minoPieces = currentMinoBlock.leftPieces;
                break;
            case MinoOrientation.right:
                minoPieces = currentMinoBlock.rightPieces;
                break;
        }

        foreach (GameObject child in minoPieces)
        {
            Vector3 forward = child.transform.TransformDirection(Vector3.forward);
            RaycastHit hit;

            int placedMinoLayer = 1 << 9;
            int borderLayer = 1 << 10;

            int layerMask = placedMinoLayer | borderLayer;

            if (Physics.Raycast(child.transform.position, forward, out hit, 1, layerMask))
            {
                return false;
                //GameManger.instance.ResetMino();
            }
            else return true;
            //Debug.DrawRay(child.transform.position, forward * 1, Color.green);
        }

        return true;
    }

    public bool CanMoveHorizontal(Direction dir)
    {
        GameObject[] minoPieces = null;
        currentMinoBlock = gameObject.GetComponent<MinoBlock>();

        switch (currentMinoBlock.activeMinoOrientation)
        {
            case MinoOrientation.flat:
                minoPieces = currentMinoBlock.flatPieces;
                break;
            case MinoOrientation.flipped:
                minoPieces = currentMinoBlock.flippedPieces;
                break;
            case MinoOrientation.left:
                minoPieces = currentMinoBlock.leftPieces;
                break;
            case MinoOrientation.right:
                minoPieces = currentMinoBlock.rightPieces;
                break;
        }

        foreach (GameObject child in minoPieces)
        {
            switch (dir)
            {
                case Direction.left:
                    vectorDirection = child.transform.TransformDirection(-Vector3.left);
                    break;
                case Direction.right:
                    vectorDirection = child.transform.TransformDirection(-Vector3.right);
                    break;
            }

            RaycastHit hit;

            int placedMinoLayer = 1 << 9;
            int borderLayer = 1 << 10;

            int layerMask = placedMinoLayer | borderLayer;

            if (Physics.Raycast(child.transform.position, vectorDirection, out hit, 1, layerMask))
            {
                //Debug.Log(child.name + " can see " + hit.transform.name);
                //GameManger.instance.ResetMino();
                return false;
            }
            //Debug.DrawRay(child.transform.position, left * 1, Color.green);
        }
        return true;
    }
}