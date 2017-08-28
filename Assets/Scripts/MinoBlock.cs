using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// these should match the GameManager.instance.minoPrefabs[] array
public enum MinoType
{
    iMino = 0, jMino = 1, lMino = 2, oMino = 3, sMino = 4, tMino = 5, zMino = 6
}

public enum Orientation
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
    public Orientation activeMinoOrientation;
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
        GameObject[] currentMinoPieces = null;
        currentMinoBlock = gameObject.GetComponent<MinoBlock>();
        int counter = 0;

        // get the visible/active mino pieces for our orientation
        switch (currentMinoBlock.activeMinoOrientation)
        {
            case Orientation.flat:
                currentMinoPieces = currentMinoBlock.flatPieces;
                break;
            case Orientation.flipped:
                currentMinoPieces = currentMinoBlock.flippedPieces;
                break;
            case Orientation.left:
                currentMinoPieces = currentMinoBlock.leftPieces;
                break;
            case Orientation.right:
                currentMinoPieces = currentMinoBlock.rightPieces;
                break;
        }

        for (int i = 0; i < currentMinoPieces.Length; i++)
        {
            Vector3 forward = currentMinoPieces[i].transform.TransformDirection(Vector3.forward);
            RaycastHit hit;

            if (Physics.Raycast(currentMinoPieces[i].transform.position, forward, out hit, 1, GameManger.instance.minoBlockLayerMask.value))
            {
                counter++;
            }
        }

        // return false (can't move down) if any of the pieces see the floor, or other set pieces
        if (counter > 0)
        {
            return false;
        }
        else return true;
    }

    public bool CanMoveHorizontal(Direction dir, Orientation orientation)
    {
        GameObject[] currentMinoPieces = null;
        currentMinoBlock = gameObject.GetComponent<MinoBlock>();

        // get the visible/active mino pieces for our orientation
        switch (orientation)
        {
            case Orientation.flat:
                currentMinoPieces = currentMinoBlock.flatPieces;
                break;
            case Orientation.flipped:
                currentMinoPieces = currentMinoBlock.flippedPieces;
                break;
            case Orientation.left:
                currentMinoPieces = currentMinoBlock.leftPieces;
                break;
            case Orientation.right:
                currentMinoPieces = currentMinoBlock.rightPieces;
                break;
        }

        for (int i = 0; i < currentMinoPieces.Length; i++)
        {
            switch (dir)
            {
                case Direction.left:
                    vectorDirection = currentMinoPieces[i].transform.TransformDirection(-Vector3.left);
                    break;
                case Direction.right:
                    vectorDirection = currentMinoPieces[i].transform.TransformDirection(-Vector3.right);
                    break;
            }

            RaycastHit hit;

            if (Physics.Raycast(currentMinoPieces[i].transform.position, vectorDirection, out hit, 1, GameManger.instance.minoBlockLayerMask.value))
            {
                return false;
            }
        }
        return true;
    }
}