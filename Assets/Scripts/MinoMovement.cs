using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoMovement : MonoBehaviour
{
    private Rigidbody currentRigidbody;
    private MinoBlock currentMinoBlock;
    private MinoPiece currentMinoPiece;
    private MeshRenderer currentMeshRenderer;

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

        //set the pieces that we want to be visible
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
        int counter = 0;

        // rotate relative to our current orientation
        switch (currentMinoBlock.activeMinoOrientation)
        {
            case MinoOrientation.flat:

                switch (dir)
                {
                    case Direction.left:
                        foreach (GameObject piece in currentMinoBlock.leftPieces)
                        {
                            currentMinoPiece = piece.gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            SetMinoOrientation(MinoOrientation.left);
                        }
                        else
                        {
                            Debug.Log("Rotation blocked");
                        }
                        break;
                    case Direction.right:
                        foreach (GameObject piece in currentMinoBlock.rightPieces)
                        {
                            currentMinoPiece = piece.gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            SetMinoOrientation(MinoOrientation.right);
                        }
                        else
                        {
                            Debug.Log("Rotation blocked");
                        }
                        break;
                }
                break;

            case MinoOrientation.left:

                switch (dir)
                {
                    case Direction.left: //flipped
                        foreach (GameObject piece in currentMinoBlock.flippedPieces)
                        {
                            currentMinoPiece = piece.gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            SetMinoOrientation(MinoOrientation.flipped);
                        }
                        else
                        {
                            Debug.Log("Rotation blocked");
                        }
                        break;
                    case Direction.right: //flat
                        foreach (GameObject piece in currentMinoBlock.flatPieces)
                        {
                            currentMinoPiece = piece.gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            SetMinoOrientation(MinoOrientation.flat);
                        }
                        else
                        {
                            Debug.Log("Rotation blocked");
                        }
                        break;
                }
                break;

            case MinoOrientation.right:

                switch (dir)
                {
                    case Direction.left: //flat
                        foreach (GameObject piece in currentMinoBlock.flatPieces)
                        {
                            currentMinoPiece = piece.gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            SetMinoOrientation(MinoOrientation.flat);
                        }
                        else
                        {
                            Debug.Log("Rotation blocked");
                        }
                        break;
                    case Direction.right: //flipped
                        foreach (GameObject piece in currentMinoBlock.flippedPieces)
                        {
                            currentMinoPiece = piece.gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            SetMinoOrientation(MinoOrientation.flipped);
                        }
                        else
                        {
                            Debug.Log("Rotation blocked");
                        }
                        break;
                }
                break;

            case MinoOrientation.flipped:

                switch (dir)
                {
                    case Direction.left: //right
                        foreach (GameObject piece in currentMinoBlock.rightPieces)
                        {
                            currentMinoPiece = piece.gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            SetMinoOrientation(MinoOrientation.right);
                        }
                        else
                        {
                            Debug.Log("Rotation blocked");
                        }
                        break;

                    case Direction.right: //left
                        foreach (GameObject piece in currentMinoBlock.leftPieces)
                        {
                            currentMinoPiece = piece.gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            SetMinoOrientation(MinoOrientation.left);
                        }
                        else
                        {
                            Debug.Log("Rotation blocked");
                        }
                        break;
                }
                break;
        }
    }

}
