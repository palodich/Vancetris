using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoMovement : MonoBehaviour
{
    private Rigidbody currentRigidbody;
    private MinoBlock currentMinoBlock;
    private MinoPiece currentMinoPiece;
    private MeshRenderer currentMeshRenderer;
    private MinoMovement minoMovementComponent;

    public void MoveHorizontal(GameObject mino, Direction direction, int distance)
    {
        //currentRigidbody = mino.GetComponent<Rigidbody>();

        switch (direction)
        {
            case Direction.left:
                mino.transform.position = new Vector3((mino.transform.position.x + distance), mino.transform.position.y, mino.transform.position.z);
                break;
            case Direction.right:
                mino.transform.position = new Vector3((mino.transform.position.x - distance), mino.transform.position.y, mino.transform.position.z);
                break;
        }
    }

    public void MoveDown(GameObject mino, int distance)
    {
        /*currentRigidbody = mino.GetComponent<Rigidbody>();

        currentRigidbody.position = new Vector3(currentRigidbody.position.x, (currentRigidbody.position.y - distance), currentRigidbody.position.z);*/

        mino.transform.position = new Vector3(mino.transform.position.x, (mino.transform.position.y - distance), mino.transform.position.z);
    }

    public void MoveUp(GameObject mino, int distance)
    {
        /*currentRigidbody = mino.GetComponent<Rigidbody>();

        currentRigidbody.position = new Vector3(currentRigidbody.position.x, (currentRigidbody.position.y + distance), currentRigidbody.position.z);*/

        mino.transform.position = new Vector3(mino.transform.position.x, (mino.transform.position.y + distance), mino.transform.position.z);
    }

    public void SetMinoOrientation(MinoOrientation orientation)
    {
        currentMinoBlock = GetComponent<MinoBlock>();

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
            SetMinoOrientation(endOrientation);
        }

        Destroy(testBlock.gameObject);
    }

    public void Kick(GameObject mino, MinoOrientation endOrientation)
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
            case MinoOrientation.flat:
                testBlockUpPieces = testBlockUp.flatPieces;
                testBlockLeftPieces = testBlockLeft.flatPieces;
                testBlockRightPieces = testBlockRight.flatPieces;
                break;

            case MinoOrientation.left:
                testBlockUpPieces = testBlockUp.leftPieces;
                testBlockLeftPieces = testBlockLeft.leftPieces;
                testBlockRightPieces = testBlockRight.leftPieces;
                break;

            case MinoOrientation.right:
                testBlockUpPieces = testBlockUp.rightPieces;
                testBlockLeftPieces = testBlockLeft.rightPieces;
                testBlockRightPieces = testBlockRight.rightPieces;
                break;

            case MinoOrientation.flipped:
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
        MoveUp(testBlockUp.gameObject, 1);
        minoMovementComponent = testBlockUp.GetComponent<MinoMovement>();
        minoMovementComponent.SetMinoOrientation(endOrientation);
        foreach (GameObject piece in testBlockUpPieces)
        {
            currentMeshRenderer = piece.GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;

            testPiecePos = piece.transform.position;

            testColliders = Physics.OverlapBox(testPiecePos, overlapBoxSize, Quaternion.identity, GameManger.instance.minoBlockLayerMask);

            if (testColliders.Length > 0)
            {
                upCounter++;

                /*foreach (Collider col in testColliders)
                {
                    Debug.Log(piece.transform.parent.name + " " + piece.name + " " + piece.transform.position + " collided with " + col.name + " " + testPiecePos);
                }*/
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
        MoveHorizontal(testBlockLeft.gameObject, Direction.left, 1);
        minoMovementComponent = testBlockLeft.GetComponent<MinoMovement>();
        minoMovementComponent.SetMinoOrientation(endOrientation);
        foreach (GameObject piece in testBlockLeftPieces)
        {
            currentMeshRenderer = piece.GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;

            testPiecePos = piece.transform.position;

            testColliders = Physics.OverlapBox(testPiecePos, overlapBoxSize, Quaternion.identity, GameManger.instance.minoBlockLayerMask);

            if (testColliders.Length > 0)
            {
                leftCounter++;

                /*foreach (Collider col in testColliders)
                {
                    Debug.Log(piece.transform.parent.name + " " + piece.name + " " + piece.transform.position + " collided with " + col.name + " " + testPiecePos);
                }*/
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
        MoveHorizontal(testBlockRight.gameObject, Direction.right, 1);
        minoMovementComponent = testBlockRight.GetComponent<MinoMovement>();
        minoMovementComponent.SetMinoOrientation(endOrientation);
        foreach (GameObject piece in testBlockRightPieces)
        {
            currentMeshRenderer = piece.GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;

            testPiecePos = piece.transform.position;

            testColliders = Physics.OverlapBox(testPiecePos, overlapBoxSize, Quaternion.identity, GameManger.instance.minoBlockLayerMask);

            if (testColliders.Length > 0)
            {
                rightCounter++;

                /*foreach (Collider col in testColliders)
                {
                    Debug.Log(piece.transform.parent.name + " " + piece.name + " " + piece.transform.position + " collided with " + col.name + " " + testPiecePos);
                }*/
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

        minoMovementComponent = mino.GetComponent<MinoMovement>();

        if (!canKickUp && canKickLeft && !canKickRight)
        {
            MoveHorizontal(mino, Direction.left, 1);
            minoMovementComponent.SetMinoOrientation(endOrientation);
        }
        if (!canKickUp && !canKickLeft && canKickRight)
        {
            MoveHorizontal(mino, Direction.right, 1);
            minoMovementComponent.SetMinoOrientation(endOrientation);
        }
        if (canKickUp && !canKickLeft && !canKickRight)
        {
            MoveUp(mino, 1);
            minoMovementComponent.SetMinoOrientation(endOrientation);
        }
    }

    public void RotateMinoBlock(GameObject mino, Direction dir)
    {
        currentMinoBlock = GameManger.instance.activeMino.GetComponent<MinoBlock>();
        minoMovementComponent = mino.GetComponent<MinoMovement>();
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
                            minoMovementComponent.SetMinoOrientation(MinoOrientation.left);
                        }
                        else
                        {
                            Kick(mino, MinoOrientation.left);
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
                            minoMovementComponent.SetMinoOrientation(MinoOrientation.right);
                        }
                        else
                        {
                            Kick(mino, MinoOrientation.right);
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
                            minoMovementComponent.SetMinoOrientation(MinoOrientation.flipped);
                        }
                        else
                        {
                            Kick(mino, MinoOrientation.flipped);
                            /*else if (currentMinoBlock.activeMinoType == MinoType.iMino && currentMinoBlock.CanMoveHorizontal(Direction.left, currentMinoBlock.activeMinoOrientation))
                            {
                                IMinoKick(mino, Direction.left, MinoOrientation.flipped);
                            }*/
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
                            minoMovementComponent.SetMinoOrientation(MinoOrientation.flat);
                        }
                        else
                        {
                            Kick(mino, MinoOrientation.flat);
                            /*else if (currentMinoBlock.activeMinoType == MinoType.iMino && currentMinoBlock.CanMoveHorizontal(Direction.left, currentMinoBlock.activeMinoOrientation))
                            {
                                IMinoKick(mino, Direction.left, MinoOrientation.flat);
                            }*/
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
                            minoMovementComponent.SetMinoOrientation(MinoOrientation.flat);
                        }
                        else
                        {
                            Kick(mino, MinoOrientation.flat);
                            /*else if (currentMinoBlock.activeMinoType == MinoType.iMino && currentMinoBlock.CanMoveHorizontal(Direction.right, currentMinoBlock.activeMinoOrientation))
                            {
                                IMinoKick(mino, Direction.right, MinoOrientation.flat);
                            }*/

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
                            minoMovementComponent.SetMinoOrientation(MinoOrientation.flipped);
                        }
                        else
                        {
                            Kick(mino, MinoOrientation.flipped);
                            /*else if (currentMinoBlock.activeMinoType == MinoType.iMino && currentMinoBlock.CanMoveHorizontal(Direction.right, currentMinoBlock.activeMinoOrientation))
                            {
                                IMinoKick(mino, Direction.right, MinoOrientation.flipped);
                            }*/
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
                            minoMovementComponent.SetMinoOrientation(MinoOrientation.right);
                        }
                        else
                        {
                            Kick(mino, MinoOrientation.right);
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
                            minoMovementComponent.SetMinoOrientation(MinoOrientation.left);
                        }
                        else
                        {
                            Kick(mino, MinoOrientation.left);
                        }
                        break;
                }
                break;
        }
    }



}
