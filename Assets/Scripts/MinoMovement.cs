using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoMovement : MonoBehaviour
{
    private MinoBlock currentMinoBlock;
    private MinoPiece currentMinoPiece;
    private MeshRenderer currentMeshRenderer;
    private MinoMovement currentMinoMovement;

    public void MoveHorizontal(Direction direction, int distance)
    {
        switch (direction)
        {
            case Direction.left:
                gameObject.transform.position = new Vector3((gameObject.transform.position.x + distance), gameObject.transform.position.y, gameObject.transform.position.z);
                break;
            case Direction.right:
                gameObject.transform.position = new Vector3((gameObject.transform.position.x - distance), gameObject.transform.position.y, gameObject.transform.position.z);
                break;
        }
    }

    public void MoveDown(int distance)
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, (gameObject.transform.position.y - distance), gameObject.transform.position.z);
    }

    public void MoveUp(int distance)
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, (gameObject.transform.position.y + distance), gameObject.transform.position.z);
    }

    public void SetMinoOrientation(Orientation orientation)
    {
        currentMinoBlock = GetComponent<MinoBlock>();

        //set the pieces that we want to be visible
        switch (orientation)
        {
            case Orientation.flat:
                for (int i = 0; i < currentMinoBlock.flatPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.flatPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = true;
                }
                for (int i = 0; i < currentMinoBlock.leftPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.leftPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                for (int i = 0; i < currentMinoBlock.rightPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.rightPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                for (int i = 0; i < currentMinoBlock.flippedPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.flippedPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                currentMinoBlock.activeMinoOrientation = Orientation.flat;
                break;

            case Orientation.left:
                for (int i = 0; i < currentMinoBlock.flatPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.flatPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                for (int i = 0; i < currentMinoBlock.leftPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.leftPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = true;
                }
                for (int i = 0; i < currentMinoBlock.rightPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.rightPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                for (int i = 0; i < currentMinoBlock.flippedPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.flippedPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                currentMinoBlock.activeMinoOrientation = Orientation.left;
                break;

            case Orientation.right:
                for (int i = 0; i < currentMinoBlock.flatPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.flatPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                for (int i = 0; i < currentMinoBlock.leftPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.leftPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                for (int i = 0; i < currentMinoBlock.rightPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.rightPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = true;
                }
                for (int i = 0; i < currentMinoBlock.flippedPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.flippedPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                currentMinoBlock.activeMinoOrientation = Orientation.right;
                break;

            case Orientation.flipped:
                for (int i = 0; i < currentMinoBlock.flatPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.flatPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                for (int i = 0; i < currentMinoBlock.leftPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.leftPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                for (int i = 0; i < currentMinoBlock.rightPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.rightPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = false;
                }
                for (int i = 0; i < currentMinoBlock.flippedPieces.Length; i++)
                {
                    currentMeshRenderer = currentMinoBlock.flippedPieces[i].GetComponent<MeshRenderer>();
                    currentMeshRenderer.enabled = true;
                }
                currentMinoBlock.activeMinoOrientation = Orientation.flipped;
                break;
        }
    }

    // IMinos are special because they need to kicked 2 spaces instead of one
    private void IMinoKick(Direction endDirection, Orientation endOrientation)
    {
        MinoBlock testBlock;

        int counter = 0;

        // Set isColliding to false before instantiate
        for (int i = 0; i < currentMinoBlock.flatPieces.Length; i++)
        {
            currentMinoPiece = currentMinoBlock.flatPieces[i].gameObject.GetComponent<MinoPiece>();
            currentMinoPiece.isColliding = false;
        }

        testBlock = Instantiate(currentMinoBlock, currentMinoBlock.transform.position, Quaternion.identity);

        for (int i = 0; i < testBlock.leftPieces.Length; i++)
        {
            currentMeshRenderer = testBlock.leftPieces[i].GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;
        }

        testBlock.name = "testBlock";

        currentMinoMovement = testBlock.GetComponent<MinoMovement>();
        currentMinoMovement.MoveHorizontal(endDirection, 2);

        for (int i = 0; i < testBlock.flatPieces.Length; i++)
        {
            currentMinoPiece = testBlock.flatPieces[i].gameObject.GetComponent<MinoPiece>();
            if (currentMinoPiece.isColliding)
            {
                counter++;
            }
        }

        if (counter == 0)
        {
            MoveHorizontal(endDirection, 2);
            SetMinoOrientation(endOrientation);
        }

        Destroy(testBlock.gameObject);
    }

    public void Kick(Orientation endOrientation)
    {
        MinoBlock testBlockUp;
        MinoBlock testBlockLeft;
        MinoBlock testBlockRight;

        GameObject[] testBlockUpPieces = null;
        GameObject[] testBlockLeftPieces = null;
        GameObject[] testBlockRightPieces = null;

        bool canKickUp = false;
        bool canKickLeft = false;
        bool canKickRight = false;

        int upCounter = 0;
        int leftCounter = 0;
        int rightCounter = 0;

        Collider[] testColliders;
        Vector3 testPiecePos;
        Vector3 overlapBoxSize = new Vector3(0.25f, 0.25f, 0.25f);

        testBlockUp = Instantiate(currentMinoBlock, currentMinoBlock.transform.position, Quaternion.identity);
        testBlockLeft = Instantiate(currentMinoBlock, currentMinoBlock.transform.position, Quaternion.identity);
        testBlockRight = Instantiate(currentMinoBlock, currentMinoBlock.transform.position, Quaternion.identity);

        switch (endOrientation)
        {
            case Orientation.flat:
                testBlockUpPieces = testBlockUp.flatPieces;
                testBlockLeftPieces = testBlockLeft.flatPieces;
                testBlockRightPieces = testBlockRight.flatPieces;
                break;

            case Orientation.left:
                testBlockUpPieces = testBlockUp.leftPieces;
                testBlockLeftPieces = testBlockLeft.leftPieces;
                testBlockRightPieces = testBlockRight.leftPieces;
                break;

            case Orientation.right:
                testBlockUpPieces = testBlockUp.rightPieces;
                testBlockLeftPieces = testBlockLeft.rightPieces;
                testBlockRightPieces = testBlockRight.rightPieces;
                break;

            case Orientation.flipped:
                testBlockUpPieces = testBlockUp.flippedPieces;
                testBlockLeftPieces = testBlockLeft.flippedPieces;
                testBlockRightPieces = testBlockRight.flippedPieces;
                break;
        }

        testBlockUp.name = "testBlockUp";
        testBlockLeft.name = "testBlockLeft";
        testBlockRight.name = "testBlockRight";

        testBlockUp.gameObject.layer = 0;
        testBlockLeft.gameObject.layer = 0;
        testBlockRight.gameObject.layer = 0;

        /*
         * Test moving the mino up
         */
        currentMinoMovement = testBlockUp.GetComponent<MinoMovement>();
        currentMinoMovement.MoveUp(1);
        currentMinoMovement.SetMinoOrientation(endOrientation);
        for (int i = 0; i < testBlockUpPieces.Length; i++)
        {
            currentMeshRenderer = testBlockUpPieces[i].GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;

            testPiecePos = testBlockUpPieces[i].transform.position;

            testColliders = Physics.OverlapBox(testPiecePos, overlapBoxSize, Quaternion.identity, GameManger.instance.minoBlockLayerMask);

            if (testColliders.Length > 0)
            {
                upCounter++;
            }
        }
        if (upCounter == 0)
        {
            canKickUp = true;
        }
        else
        {
            canKickUp = false;
        }

        /*
         * Test moving the mino left
         */
        currentMinoMovement = testBlockLeft.GetComponent<MinoMovement>();
        currentMinoMovement.MoveHorizontal(Direction.left, 1);
        currentMinoMovement.SetMinoOrientation(endOrientation);
        for (int i = 0; i < testBlockLeftPieces.Length; i++)
        {
            currentMeshRenderer = testBlockLeftPieces[i].GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;

            testPiecePos = testBlockLeftPieces[i].transform.position;

            testColliders = Physics.OverlapBox(testPiecePos, overlapBoxSize, Quaternion.identity, GameManger.instance.minoBlockLayerMask);

            if (testColliders.Length > 0)
            {
                leftCounter++;
            }
        }
        if (leftCounter == 0)
        {
            canKickLeft = true;
        }
        else
        {
            canKickLeft = false;
        }

        /*
         * Test moving the mino right
         */
        currentMinoMovement = testBlockRight.GetComponent<MinoMovement>();
        currentMinoMovement.MoveHorizontal(Direction.right, 1);
        currentMinoMovement.SetMinoOrientation(endOrientation);
        for (int i = 0; i < testBlockRightPieces.Length; i++)
        {
            currentMeshRenderer = testBlockRightPieces[i].GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;

            testPiecePos = testBlockRightPieces[i].transform.position;

            testColliders = Physics.OverlapBox(testPiecePos, overlapBoxSize, Quaternion.identity, GameManger.instance.minoBlockLayerMask);

            if (testColliders.Length > 0)
            {
                rightCounter++;
            }
        }
        if (rightCounter == 0)
        {
            canKickRight = true;
        }
        else
        {
            canKickRight = false;
        }

        //Debug.Log("canKickUp: " + canKickUp + " | " + "canKickLeft: " + canKickLeft + " | " + "canKickRight: " + canKickRight);

        Destroy(testBlockUp.gameObject);
        Destroy(testBlockLeft.gameObject);
        Destroy(testBlockRight.gameObject);

        currentMinoMovement = gameObject.GetComponent<MinoMovement>();

        if (!canKickUp && canKickLeft && !canKickRight)
        {
            currentMinoMovement.MoveHorizontal(Direction.left, 1);
            currentMinoMovement.SetMinoOrientation(endOrientation);
        }
        if (!canKickUp && !canKickLeft && canKickRight)
        {
            currentMinoMovement.MoveHorizontal(Direction.right, 1);
            currentMinoMovement.SetMinoOrientation(endOrientation);
        }
        if (canKickUp && !canKickLeft && !canKickRight)
        {
            MoveUp(1);
            currentMinoMovement.SetMinoOrientation(endOrientation);
        }
    }

    public void RotateMinoBlock(Direction dir)
    {
        currentMinoBlock = gameObject.GetComponent<MinoBlock>();
        currentMinoMovement = gameObject.GetComponent<MinoMovement>();
        int counter = 0;

        // rotate relative to our current orientation
        switch (currentMinoBlock.activeMinoOrientation)
        {
            case Orientation.flat:

                switch (dir)
                {
                    case Direction.left:
                        for (int i = 0; i < currentMinoBlock.leftPieces.Length; i++)
                        {
                            currentMinoPiece = currentMinoBlock.leftPieces[i].gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            currentMinoMovement.SetMinoOrientation(Orientation.left);
                        }
                        else
                        {
                            Kick(Orientation.left);
                        }
                        break;
                    case Direction.right:
                        for (int i = 0; i < currentMinoBlock.rightPieces.Length; i++)
                        {
                            currentMinoPiece = currentMinoBlock.rightPieces[i].gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            currentMinoMovement.SetMinoOrientation(Orientation.right);
                        }
                        else
                        {
                            Kick(Orientation.right);
                        }
                        break;
                }
                break;

            case Orientation.left:

                switch (dir)
                {
                    case Direction.left: //flipped
                        for (int i = 0; i < currentMinoBlock.flippedPieces.Length; i++)
                        {
                            currentMinoPiece = currentMinoBlock.flippedPieces[i].gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            currentMinoMovement.SetMinoOrientation(Orientation.flipped);
                        }
                        else
                        {
                            Kick(Orientation.flipped);
                            /*else if (currentMinoBlock.activeMinoType == MinoType.iMino && currentMinoBlock.CanMoveHorizontal(Direction.left, currentMinoBlock.activeMinoOrientation))
                            {
                                IMinoKick(mino, Direction.left, MinoOrientation.flipped);
                            }*/
                        }
                        break;
                    case Direction.right: //flat
                        for (int i = 0; i < currentMinoBlock.flatPieces.Length; i++)
                        {
                            currentMinoPiece = currentMinoBlock.flatPieces[i].gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            currentMinoMovement.SetMinoOrientation(Orientation.flat);
                        }
                        else
                        {
                            Kick(Orientation.flat);
                            /*else if (currentMinoBlock.activeMinoType == MinoType.iMino && currentMinoBlock.CanMoveHorizontal(Direction.left, currentMinoBlock.activeMinoOrientation))
                            {
                                IMinoKick(mino, Direction.left, MinoOrientation.flat);
                            }*/
                        }
                        break;
                }
                break;

            case Orientation.right:

                switch (dir)
                {
                    case Direction.left: //flat
                        for (int i = 0; i < currentMinoBlock.flatPieces.Length; i++)
                        {
                            currentMinoPiece = currentMinoBlock.flatPieces[i].gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            currentMinoMovement.SetMinoOrientation(Orientation.flat);
                        }
                        else
                        {
                            Kick(Orientation.flat);
                            /*else if (currentMinoBlock.activeMinoType == MinoType.iMino && currentMinoBlock.CanMoveHorizontal(Direction.right, currentMinoBlock.activeMinoOrientation))
                            {
                                IMinoKick(mino, Direction.right, MinoOrientation.flat);
                            }*/

                        }
                        break;
                    case Direction.right: //flipped
                        for (int i = 0; i < currentMinoBlock.flippedPieces.Length; i++)
                        {
                            currentMinoPiece = currentMinoBlock.flippedPieces[i].gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            currentMinoMovement.SetMinoOrientation(Orientation.flipped);
                        }
                        else
                        {
                            Kick(Orientation.flipped);
                            /*else if (currentMinoBlock.activeMinoType == MinoType.iMino && currentMinoBlock.CanMoveHorizontal(Direction.right, currentMinoBlock.activeMinoOrientation))
                            {
                                IMinoKick(mino, Direction.right, MinoOrientation.flipped);
                            }*/
                        }
                        break;
                }
                break;

            case Orientation.flipped:

                switch (dir)
                {
                    case Direction.left: //right
                        for (int i = 0; i < currentMinoBlock.rightPieces.Length; i++)
                        {
                            currentMinoPiece = currentMinoBlock.rightPieces[i].gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            currentMinoMovement.SetMinoOrientation(Orientation.right);
                        }
                        else
                        {
                            Kick(Orientation.right);
                        }
                        break;

                    case Direction.right: //left
                        for (int i = 0; i < currentMinoBlock.leftPieces.Length; i++)
                        {
                            currentMinoPiece = currentMinoBlock.leftPieces[i].gameObject.GetComponent<MinoPiece>();
                            if (currentMinoPiece.isColliding)
                            {
                                counter++;
                            }
                        }

                        if (counter == 0)
                        {
                            currentMinoMovement.SetMinoOrientation(Orientation.left);
                        }
                        else
                        {
                            Kick(Orientation.left);
                        }
                        break;
                }
                break;
        }
    }
}
