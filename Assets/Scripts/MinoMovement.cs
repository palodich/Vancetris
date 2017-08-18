using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoMovement : MonoBehaviour
{
    private Rigidbody currentRigidbody;
    private MinoBlock currentMinoBlock;
    private MinoPiece currentMinoPiece;
    private MeshRenderer currentMeshRenderer;

    public void MoveHorizontal(GameObject mino, Direction direction, int distance)
    {
        currentRigidbody = mino.GetComponent<Rigidbody>();

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

    public void SetMinoOrientation(GameObject mino, MinoOrientation orientation)
    {
        currentMinoBlock = mino.GetComponent<MinoBlock>();

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

    // IMinos are special because they need to kicked 2 spaces instead of one
    private void IMinoKick(GameObject mino, Direction endDirection, MinoOrientation endOrientation)
    {
        Debug.Log("Boosh, it's a mess!");

        MinoBlock testBlock;
        int counter = 0;

        // Set isColliding to false before instantiate
        foreach (GameObject piece in currentMinoBlock.flatPieces)
        {
            currentMinoPiece = piece.gameObject.GetComponent<MinoPiece>();
            currentMinoPiece.isColliding = false;
        }

        testBlock = Instantiate(currentMinoBlock, currentMinoBlock.transform.position, Quaternion.identity);

        foreach (GameObject piece in testBlock.leftPieces)
        {
            currentMeshRenderer = piece.GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;
        }

        testBlock.name = "testBlock";

        MoveHorizontal(testBlock.gameObject, endDirection, 2);

        foreach (GameObject piece in testBlock.flatPieces)
        {
            currentMinoPiece = piece.gameObject.GetComponent<MinoPiece>();
            if (currentMinoPiece.isColliding)
            {
                counter++;
            }
        }

        if (counter == 0)
        {
            MoveHorizontal(mino, endDirection, 2);
            SetMinoOrientation(mino, endOrientation);
        }

        Destroy(testBlock.gameObject);
    }

    public void RotateMinoBlock(GameObject mino, Direction dir)
    {
        currentMinoBlock = GameManger.instance.activeMino.GetComponent<MinoBlock>();
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
                            SetMinoOrientation(mino, MinoOrientation.left);
                        }
                        else
                        {
                            if (currentMinoBlock.CanMoveHorizontal(Direction.left, MinoOrientation.left))
                            {
                                MoveHorizontal(mino, Direction.left, 1);
                                SetMinoOrientation(mino, MinoOrientation.left);
                            }
                            else if (currentMinoBlock.CanMoveHorizontal(Direction.right, MinoOrientation.left))
                            {
                                MoveHorizontal(mino, Direction.right, 1);
                                SetMinoOrientation(mino, MinoOrientation.left);
                            }
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
                            SetMinoOrientation(mino, MinoOrientation.right);
                        }
                        else
                        {
                            if (currentMinoBlock.CanMoveHorizontal(Direction.left, MinoOrientation.right))
                            {
                                MoveHorizontal(mino, Direction.left, 1);
                                SetMinoOrientation(mino, MinoOrientation.right);
                            }
                            else if (currentMinoBlock.CanMoveHorizontal(Direction.right, MinoOrientation.right))
                            {
                                MoveHorizontal(mino, Direction.right, 1);
                                SetMinoOrientation(mino, MinoOrientation.right);
                            }
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
                            SetMinoOrientation(mino, MinoOrientation.flipped);
                        }
                        else
                        {
                            if (currentMinoBlock.CanMoveHorizontal(Direction.left, MinoOrientation.flipped))
                            {
                                MoveHorizontal(mino, Direction.left, 1);
                                SetMinoOrientation(mino, MinoOrientation.flipped);
                            }
                            else if (currentMinoBlock.CanMoveHorizontal(Direction.right, MinoOrientation.flipped))
                            {
                                MoveHorizontal(mino, Direction.right, 1);
                                SetMinoOrientation(mino, MinoOrientation.flipped);
                            }
                            else if (currentMinoBlock.activeMinoType == MinoType.iMino && currentMinoBlock.CanMoveHorizontal(Direction.left, currentMinoBlock.activeMinoOrientation))
                            {
                                IMinoKick(mino, Direction.left, MinoOrientation.flipped);
                            }
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
                            SetMinoOrientation(mino, MinoOrientation.flat);
                        }
                        else
                        {
                            if (currentMinoBlock.CanMoveHorizontal(Direction.left, MinoOrientation.flat))
                            {
                                MoveHorizontal(mino, Direction.left, 1);
                                SetMinoOrientation(mino, MinoOrientation.flat);
                            }
                            else if (currentMinoBlock.CanMoveHorizontal(Direction.right, MinoOrientation.flat))
                            {
                                MoveHorizontal(mino, Direction.right, 1);
                                SetMinoOrientation(mino, MinoOrientation.flat);
                            }
                            else if (currentMinoBlock.activeMinoType == MinoType.iMino && currentMinoBlock.CanMoveHorizontal(Direction.left, currentMinoBlock.activeMinoOrientation))
                            {
                                IMinoKick(mino, Direction.left, MinoOrientation.flat);
                            }
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
                            SetMinoOrientation(mino, MinoOrientation.flat);
                        }
                        else
                        {
                            if (currentMinoBlock.CanMoveHorizontal(Direction.left, MinoOrientation.flat))
                            {
                                MoveHorizontal(mino, Direction.left, 1);
                                SetMinoOrientation(mino, MinoOrientation.flat);
                            }
                            else if (currentMinoBlock.CanMoveHorizontal(Direction.right, MinoOrientation.flat))
                            {
                                MoveHorizontal(mino, Direction.right, 1);
                                SetMinoOrientation(mino, MinoOrientation.flat);
                            }
                            else if (currentMinoBlock.activeMinoType == MinoType.iMino && currentMinoBlock.CanMoveHorizontal(Direction.right, currentMinoBlock.activeMinoOrientation))
                            {
                                IMinoKick(mino, Direction.right, MinoOrientation.flat);
                            }

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
                            SetMinoOrientation(mino, MinoOrientation.flipped);
                        }
                        else
                        {
                            if (currentMinoBlock.CanMoveHorizontal(Direction.left, MinoOrientation.flipped))
                            {
                                MoveHorizontal(mino, Direction.left, 1);
                                SetMinoOrientation(mino, MinoOrientation.flipped);
                            }
                            else if (currentMinoBlock.CanMoveHorizontal(Direction.right, MinoOrientation.flipped))
                            {
                                MoveHorizontal(mino, Direction.right, 1);
                                SetMinoOrientation(mino, MinoOrientation.flipped);
                            }
                            else if (currentMinoBlock.activeMinoType == MinoType.iMino && currentMinoBlock.CanMoveHorizontal(Direction.right, currentMinoBlock.activeMinoOrientation))
                            {
                                IMinoKick(mino, Direction.right, MinoOrientation.flipped);
                            }
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
                            SetMinoOrientation(mino, MinoOrientation.right);
                        }
                        else
                        {
                            if (currentMinoBlock.CanMoveHorizontal(Direction.left, MinoOrientation.right))
                            {
                                MoveHorizontal(mino, Direction.left, 1);
                                SetMinoOrientation(mino, MinoOrientation.right);
                            }
                            else if (currentMinoBlock.CanMoveHorizontal(Direction.right, MinoOrientation.right))
                            {
                                MoveHorizontal(mino, Direction.right, 1);
                                SetMinoOrientation(mino, MinoOrientation.right);
                            }
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
                            SetMinoOrientation(mino, MinoOrientation.left);
                        }
                        else
                        {
                            if (currentMinoBlock.CanMoveHorizontal(Direction.left, MinoOrientation.left))
                            {
                                MoveHorizontal(mino, Direction.left, 1);
                                SetMinoOrientation(mino, MinoOrientation.left);
                            }
                            else if (currentMinoBlock.CanMoveHorizontal(Direction.right, MinoOrientation.left))
                            {
                                MoveHorizontal(mino, Direction.right, 1);
                                SetMinoOrientation(mino, MinoOrientation.left);
                            }
                        }
                        break;
                }
                break;
        }
    }



}
