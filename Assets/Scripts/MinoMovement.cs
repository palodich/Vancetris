using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoMovement : MonoBehaviour
{
    Rigidbody currentRb;
    MinoBlock currentMb;

    public void MoveHorizontal(Direction direction, int distance)
    {
        currentRb = gameObject.GetComponent<Rigidbody>();

        switch (direction)
        {
            case Direction.left:
                currentRb.position = new Vector3((currentRb.position.x + distance), currentRb.position.y, currentRb.position.z);
                break;
            case Direction.right:
                currentRb.position = new Vector3((currentRb.position.x - distance), currentRb.position.y, currentRb.position.z);
                break;
        }
    }

    public void MoveDown(int distance)
    {
        currentRb = gameObject.GetComponent<Rigidbody>();

        currentRb.position = new Vector3(currentRb.position.x, (currentRb.position.y - distance), currentRb.position.z);
    }

    public void MoveUp(int distance)
    {
        currentRb = gameObject.GetComponent<Rigidbody>();

        currentRb.position = new Vector3(currentRb.position.x, (currentRb.position.y + distance), currentRb.position.z);
    }

    public void SetMinoOrientation(MinoOrientation orientation)
    {
        currentMb = gameObject.GetComponent<MinoBlock>();

        //MinoOrientation startOrientation = mb.activeMinoOrientation;

        //hide all of the pieces
        foreach (GameObject piece in currentMb.flatPieces)
        {
            piece.gameObject.SetActive(false);
        }
        foreach (GameObject piece in currentMb.leftPieces)
        {
            piece.gameObject.SetActive(false);
        }
        foreach (GameObject piece in currentMb.rightPieces)
        {
            piece.gameObject.SetActive(false);
        }
        foreach (GameObject piece in currentMb.flippedPieces)
        {
            piece.gameObject.SetActive(false);
        }

        //set the pieces that we want to active
        switch (orientation)
        {
            case MinoOrientation.flat:
                foreach (GameObject piece in currentMb.flatPieces)
                {
                    piece.gameObject.SetActive(true);
                    currentMb.activeMinoOrientation = MinoOrientation.flat;
                }
                break;

            case MinoOrientation.left:
                foreach (GameObject piece in currentMb.leftPieces)
                {
                    piece.gameObject.SetActive(true);
                    currentMb.activeMinoOrientation = MinoOrientation.left;
                }
                break;

            case MinoOrientation.right:
                foreach (GameObject piece in currentMb.rightPieces)
                {
                    piece.gameObject.SetActive(true);
                    currentMb.activeMinoOrientation = MinoOrientation.right;
                }
                break;

            case MinoOrientation.flipped:
                foreach (GameObject piece in currentMb.flippedPieces)
                {
                    piece.gameObject.SetActive(true);
                    currentMb.activeMinoOrientation = MinoOrientation.flipped;
                }
                break;
        }

        //Debug.Log("End Orentation: " + mb.activeMinoOrientation);
    }

    public void RotateMinoBlock(Direction dir)
    {
        currentMb = gameObject.GetComponent<MinoBlock>();

        // rotate relative to our current orientation
        switch (currentMb.activeMinoOrientation)
        {
            case MinoOrientation.flat:

                switch (dir)
                {
                    case Direction.left:
                        SetMinoOrientation(MinoOrientation.left);
                        break;
                    case Direction.right:
                        SetMinoOrientation(MinoOrientation.right);
                        break;
                }
                break;

            case MinoOrientation.left:

                switch (dir)
                {
                    case Direction.left:
                        SetMinoOrientation(MinoOrientation.flipped);
                        break;
                    case Direction.right:
                        SetMinoOrientation(MinoOrientation.flat);
                        break;
                }
                break;

            case MinoOrientation.right:

                switch (dir)
                {
                    case Direction.left:
                        SetMinoOrientation(MinoOrientation.flat);
                        break;
                    case Direction.right:
                        SetMinoOrientation(MinoOrientation.flipped);
                        break;
                }
                break;

            case MinoOrientation.flipped:

                switch (dir)
                {
                    case Direction.left:
                        SetMinoOrientation(MinoOrientation.right);
                        break;
                    case Direction.right:
                        SetMinoOrientation(MinoOrientation.left);
                        break;
                }
                break;
        }
    }

}
