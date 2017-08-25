using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// these should match the GameManager.instance.minoPrefabs[] array
public enum MinoType
{
    iMino = 0, jMino = 1, lMino = 2, oMino = 3, sMino = 4, tMino = 5, zMino = 6
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

    public void SnapToGrid()
    {
        transform.position = new Vector3((Mathf.Round(transform.position.x * 2))/2, (Mathf.Round(transform.position.y * 2)) / 2, (Mathf.Round(transform.position.z * 2)) / 2);
    }

    public bool CanMoveDown()
    {
        GameObject[] minoPieces = null;
        currentMinoBlock = gameObject.GetComponent<MinoBlock>();
        int counter = 0;

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

        foreach (GameObject piece in minoPieces)
        {
            Vector3 forward = piece.transform.TransformDirection(Vector3.forward);
            RaycastHit hit;

            if (Physics.Raycast(piece.transform.position, forward, out hit, 1, GameManger.instance.minoBlockLayerMask.value))
            {
                //Debug.Log(Time.time + " " + child.name + " cannot move down, " + hit.collider.name + " (" + hit.collider.gameObject.layer + ") is in the way.");
                counter++;
                //GameManger.instance.ResetMino();
            }

            //Debug.DrawRay(child.transform.position, forward * 1, Color.green);
        }

        // return false (can't move down) if any of the pieces see the floor, or other set pieces
        if (counter > 0)
        {
            return false;
        }
        else return true;

    }

    public bool CanMoveHorizontal(Direction dir, MinoOrientation orientation)
    {
        GameObject[] minoPieces = null;
        currentMinoBlock = gameObject.GetComponent<MinoBlock>();

        switch (orientation)
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

            if (Physics.Raycast(child.transform.position, vectorDirection, out hit, 1, GameManger.instance.minoBlockLayerMask.value))
            {
                //Debug.Log(Time.time + " " + child.name + " can see " + hit.collider.name);
                return false;
            }
        }
        return true;
    }
}