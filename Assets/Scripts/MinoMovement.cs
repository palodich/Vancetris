using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoMovement : MonoBehaviour
{
    Rigidbody currentRigidbody;
    MinoBlock currentMinoBlock;
    MeshRenderer currentMeshRenderer;

    public void MoveHorizontal(Direction direction, int distance)
    {
        currentRigidbody = gameObject.GetComponent<Rigidbody>();

        switch (direction)
        {
            case Direction.left:
                currentRigidbody.position = new Vector3((currentRigidbody.position.x + distance), currentRigidbody.position.y, currentRigidbody.position.z);
                break;
            case Direction.right:
                currentRigidbody.position = new Vector3((currentRigidbody.position.x - distance), currentRigidbody.position.y, currentRigidbody.position.z);
                break;
        }
    }

    public void MoveDown(int distance)
    {
        currentRigidbody = gameObject.GetComponent<Rigidbody>();

        currentRigidbody.position = new Vector3(currentRigidbody.position.x, (currentRigidbody.position.y - distance), currentRigidbody.position.z);
    }

    public void MoveUp(int distance)
    {
        currentRigidbody = gameObject.GetComponent<Rigidbody>();

        currentRigidbody.position = new Vector3(currentRigidbody.position.x, (currentRigidbody.position.y + distance), currentRigidbody.position.z);
    }

    public void SetMinoOrientation(MinoOrientation orientation)
    {
        currentMinoBlock = gameObject.GetComponent<MinoBlock>();

        //MinoOrientation startOrientation = mb.activeMinoOrientation;

        //set the pieces that we want to active
        switch (orientation)
        {
            case MinoOrientation.flat:
                foreach (GameObject piece in currentMinoBlock.flatPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = true;
                }
                foreach (GameObject piece in currentMinoBlock.leftPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                foreach (GameObject piece in currentMinoBlock.rightPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                foreach (GameObject piece in currentMinoBlock.flippedPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                currentMinoBlock.activeMinoOrientation = MinoOrientation.flat;
                break;

            case MinoOrientation.left:
                foreach (GameObject piece in currentMinoBlock.flatPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                foreach (GameObject piece in currentMinoBlock.leftPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = true;
                }
                foreach (GameObject piece in currentMinoBlock.rightPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                foreach (GameObject piece in currentMinoBlock.flippedPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                currentMinoBlock.activeMinoOrientation = MinoOrientation.left;
                break;

            case MinoOrientation.right:
                foreach (GameObject piece in currentMinoBlock.flatPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                foreach (GameObject piece in currentMinoBlock.leftPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                foreach (GameObject piece in currentMinoBlock.rightPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = true;
                }
                foreach (GameObject piece in currentMinoBlock.flippedPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                currentMinoBlock.activeMinoOrientation = MinoOrientation.right;
                break;

            case MinoOrientation.flipped:
                foreach (GameObject piece in currentMinoBlock.flatPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                foreach (GameObject piece in currentMinoBlock.leftPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                foreach (GameObject piece in currentMinoBlock.rightPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                foreach (GameObject piece in currentMinoBlock.flippedPieces)
                {
                    currentMeshRenderer = piece.GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = true;
                }
                currentMinoBlock.activeMinoOrientation = MinoOrientation.flipped;
                break;
        }

        //Debug.Log("End Orentation: " + mb.activeMinoOrientation);
    }

    public void RotateMinoBlock(Direction dir)
    {
        currentMinoBlock = gameObject.GetComponent<MinoBlock>();

        // rotate relative to our current orientation
        switch (currentMinoBlock.activeMinoOrientation)
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
