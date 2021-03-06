﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

// these should match the GameManager.Instance.MinoPrefabs[] array
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
    private MinoType _activeMinoType;
    public MinoType ActiveMinoType
    {
        get { return _activeMinoType; }
        set { _activeMinoType = value; }
    }

    public GameObject[] flatPieces;
    public GameObject[] leftPieces;
    public GameObject[] rightPieces;
    public GameObject[] flippedPieces;

    private Vector3 vectorDirection;

    private MeshRenderer currentMeshRenderer;

    public bool CanMoveDown()
    {
        GameObject[] currentMinoPieces = null;
        int counter = 0;

        // get the visible/active mino pieces for our orientation
        switch (ActiveMinoOrientation)
        {
            case Orientation.flat:
                currentMinoPieces = flatPieces;
                break;
            case Orientation.flipped:
                currentMinoPieces = flippedPieces;
                break;
            case Orientation.left:
                currentMinoPieces = leftPieces;
                break;
            case Orientation.right:
                currentMinoPieces = rightPieces;
                break;
        }

        for (int i = 0; i < currentMinoPieces.Length; i++)
        {
            Vector3 forward = currentMinoPieces[i].transform.TransformDirection(Vector3.forward);
            RaycastHit hit;

            if (Physics.Raycast(currentMinoPieces[i].transform.position, forward, out hit, 1, GameManger.Instance.MinoBlockLayerMask.value))
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

        // get the visible/active mino pieces for our orientation
        switch (orientation)
        {
            case Orientation.flat:
                currentMinoPieces = flatPieces;
                break;
            case Orientation.flipped:
                currentMinoPieces = flippedPieces;
                break;
            case Orientation.left:
                currentMinoPieces = leftPieces;
                break;
            case Orientation.right:
                currentMinoPieces = rightPieces;
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

            if (Physics.Raycast(currentMinoPieces[i].transform.position, vectorDirection, out hit, 1, GameManger.Instance.MinoBlockLayerMask.value))
            {
                return false;
            }
        }
        return true;
    }

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

    private Orientation _activeMinoOrientation;
    public Orientation ActiveMinoOrientation
    {
        get { return _activeMinoOrientation; }

        set
        {
            // set the pieces that we want to be visible, only change the MeshRenderer for non-ghostMinos, because we want the GhostMino to
            // always have the mesh renderer off
            switch (value)
            {
                case Orientation.flat:
                    if (name != "GhostMino")
                    {
                        for (int i = 0; i < flatPieces.Length; i++)
                        {
                            currentMeshRenderer = flatPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = true;
                        }
                        for (int i = 0; i < leftPieces.Length; i++)
                        {
                            currentMeshRenderer = leftPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = false;
                        }
                        for (int i = 0; i < rightPieces.Length; i++)
                        {
                            currentMeshRenderer = rightPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = false;
                        }
                        for (int i = 0; i < flippedPieces.Length; i++)
                        {
                            currentMeshRenderer = flippedPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = false;
                        }
                    }
                    _activeMinoOrientation = Orientation.flat;
                    break;

                case Orientation.left:
                    if (name != "GhostMino")
                    {
                        for (int i = 0; i < flatPieces.Length; i++)
                        {
                            currentMeshRenderer = flatPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = false;
                        }
                        for (int i = 0; i < leftPieces.Length; i++)
                        {
                            currentMeshRenderer = leftPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = true;
                        }
                        for (int i = 0; i < rightPieces.Length; i++)
                        {
                            currentMeshRenderer = rightPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = false;
                        }
                        for (int i = 0; i < flippedPieces.Length; i++)
                        {
                            currentMeshRenderer = flippedPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = false;
                        }
                    }
                    _activeMinoOrientation = Orientation.left;
                    break;

                case Orientation.right:
                    if (name != "GhostMino")
                    {
                        for (int i = 0; i < flatPieces.Length; i++)
                        {
                            currentMeshRenderer = flatPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = false;
                        }
                        for (int i = 0; i < leftPieces.Length; i++)
                        {
                            currentMeshRenderer = leftPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = false;
                        }
                        for (int i = 0; i < rightPieces.Length; i++)
                        {
                            currentMeshRenderer = rightPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = true;
                        }
                        for (int i = 0; i < flippedPieces.Length; i++)
                        {
                            currentMeshRenderer = flippedPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = false;
                        }
                    }
                    _activeMinoOrientation = Orientation.right;
                    break;

                case Orientation.flipped:
                    if (name != "GhostMino")
                    {
                        for (int i = 0; i < flatPieces.Length; i++)
                        {
                            currentMeshRenderer = flatPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = false;
                        }
                        for (int i = 0; i < leftPieces.Length; i++)
                        {
                            currentMeshRenderer = leftPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = false;
                        }
                        for (int i = 0; i < rightPieces.Length; i++)
                        {
                            currentMeshRenderer = rightPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = false;
                        }
                        for (int i = 0; i < flippedPieces.Length; i++)
                        {
                            currentMeshRenderer = flippedPieces[i].GetComponent<MeshRenderer>();
                            currentMeshRenderer.enabled = true;
                        }
                    }
                    _activeMinoOrientation = Orientation.flipped;
                    break;
            }
        }
    }

    public static GameObject[] GetActiveMinoPieces(MinoBlock minoBlock)
    {
        GameObject[] activeMinoPieces = null;

        switch (minoBlock.ActiveMinoOrientation)
        {
            case Orientation.flat:
                activeMinoPieces = minoBlock.flatPieces;
                break;

            case Orientation.left:
                activeMinoPieces = minoBlock.leftPieces;
                break;

            case Orientation.right:
                activeMinoPieces = minoBlock.rightPieces;
                break;

            case Orientation.flipped:
                activeMinoPieces = minoBlock.flippedPieces;
                break;
        }

        return activeMinoPieces;
    }

    public static Outline[] GetActiveMinoPieceOutlineComponent(MinoBlock minoBlock)
    {
        GameObject[] activeMinoPieces = null;

        switch (minoBlock.ActiveMinoOrientation)
        {
            case Orientation.flat:
                activeMinoPieces = minoBlock.flatPieces;
                break;

            case Orientation.left:
                activeMinoPieces = minoBlock.leftPieces;
                break;

            case Orientation.right:
                activeMinoPieces = minoBlock.rightPieces;
                break;

            case Orientation.flipped:
                activeMinoPieces = minoBlock.flippedPieces;
                break;
        }

        Outline[] activeMinoPiecesOutlineComponent = new Outline[activeMinoPieces.Length];

        for (int i = 0; i < activeMinoPieces.Length; i++)
        {
            activeMinoPiecesOutlineComponent[i] = activeMinoPieces[i].GetComponent<Outline>();
        }

        return activeMinoPiecesOutlineComponent;
    }

    public static float GetHardDropYPosition()
    {
        MinoBlock testBlock;
        MinoBlock activeMinoMinoBlock = GameManger.Instance.ActiveMino.GetComponent<MinoBlock>();

        MeshRenderer currentMeshRenderer;

        GameObject[] testBlockPieces = null;
        GameObject[] activeMinoPieces = null;

        testBlock = Instantiate(activeMinoMinoBlock, new Vector3(activeMinoMinoBlock.transform.position.x, -3f, activeMinoMinoBlock.transform.position.z), Quaternion.identity);
        testBlock.ActiveMinoOrientation = activeMinoMinoBlock.ActiveMinoOrientation;

        testBlock.name = "testBlock";
        testBlock.gameObject.layer = 0;

        testBlockPieces = GetActiveMinoPieces(testBlock);
        activeMinoPieces = GetActiveMinoPieces(activeMinoMinoBlock);

        for (int i = 0; i < testBlockPieces.Length; i++)
        {
            currentMeshRenderer = testBlockPieces[i].GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;
        }

        // move the testBlock up until it is not colliding with anything and there is a clear path between the activeMinoBlock and the testBlock
        while (IsColliding(testBlockPieces) == true || IsPathObstructed(testBlockPieces, activeMinoPieces) == true)
        {
            testBlock.MoveUp(1);
        }

        Destroy(testBlock.gameObject);
        return testBlock.transform.position.y;
    }

    public static bool IsPathObstructed(GameObject[] firstPieces, GameObject[] secondPieces)
    {
        int lineCastCounter = 0;

        for (int i = 0; i < firstPieces.Length; i++)
        {
            if (Physics.Linecast(firstPieces[i].transform.position, secondPieces[i].transform.position, GameManger.Instance.MinoBlockLayerMask))
            {
                lineCastCounter++;
            }
        }

        if (lineCastCounter > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsColliding(GameObject[] pieces)
    {
        Vector3 piecePos;
        Collider[] pieceColliders;
        Vector3 overlapBoxSize = new Vector3(0.25f, 0.25f, 0.25f);
        int counter = 0;

        for (int i = 0; i < pieces.Length; i++)
        {
            piecePos = pieces[i].transform.position;

            pieceColliders = Physics.OverlapBox(piecePos, overlapBoxSize, Quaternion.identity, GameManger.Instance.MinoBlockLayerMask);

            if (pieceColliders.Length > 0)
            {
                counter++;
            }
        }
        if (counter == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public int HardDrop()
    {
        float hardDropYPosition = MinoBlock.GetHardDropYPosition();
        float distance = gameObject.transform.position.y - hardDropYPosition;

        gameObject.transform.position = new Vector3(gameObject.transform.position.x, hardDropYPosition, gameObject.transform.position.z);
        GameManger.Instance.lockTimer = GameManger.Instance.lockTimerDelay;

        return (int)distance;
    }

    public void Kick(Orientation endOrientation)
    {
        MinoBlock testBlockUp;
        MinoBlock testBlockLeft;
        MinoBlock testBlockRight;

        MinoBlock testBlockUpX2;
        MinoBlock testBlockLeftX2;
        MinoBlock testBlockRightX2;

        GameObject[] testBlockUpPieces = null;
        GameObject[] testBlockLeftPieces = null;
        GameObject[] testBlockRightPieces = null;

        GameObject[] testBlockUpX2Pieces = null;
        GameObject[] testBlockLeftX2Pieces = null;
        GameObject[] testBlockRightX2Pieces = null;

        bool canKickUp = false;
        bool canKickLeft = false;
        bool canKickRight = false;

        bool canKickUpX2 = false;
        bool canKickLeftX2 = false;
        bool canKickRightX2 = false;
        
        MinoBlock currentMinoBlock = gameObject.GetComponent<MinoBlock>();

        testBlockUp = Instantiate(currentMinoBlock, currentMinoBlock.transform.position, Quaternion.identity);
        testBlockLeft = Instantiate(currentMinoBlock, currentMinoBlock.transform.position, Quaternion.identity);
        testBlockRight = Instantiate(currentMinoBlock, currentMinoBlock.transform.position, Quaternion.identity);

        testBlockUpX2 = Instantiate(currentMinoBlock, currentMinoBlock.transform.position, Quaternion.identity);
        testBlockLeftX2 = Instantiate(currentMinoBlock, currentMinoBlock.transform.position, Quaternion.identity);
        testBlockRightX2 = Instantiate(currentMinoBlock, currentMinoBlock.transform.position, Quaternion.identity);

        switch (endOrientation)
        {
            case Orientation.flat:
                testBlockUpPieces = testBlockUp.flatPieces;
                testBlockLeftPieces = testBlockLeft.flatPieces;
                testBlockRightPieces = testBlockRight.flatPieces;

                testBlockUpX2Pieces = testBlockUpX2.flatPieces;
                testBlockLeftX2Pieces = testBlockLeftX2.flatPieces;
                testBlockRightX2Pieces = testBlockRightX2.flatPieces;
                break;

            case Orientation.left:
                testBlockUpPieces = testBlockUp.leftPieces;
                testBlockLeftPieces = testBlockLeft.leftPieces;
                testBlockRightPieces = testBlockRight.leftPieces;

                testBlockUpX2Pieces = testBlockUpX2.leftPieces;
                testBlockLeftX2Pieces = testBlockLeftX2.leftPieces;
                testBlockRightX2Pieces = testBlockRightX2.leftPieces;
                break;

            case Orientation.right:
                testBlockUpPieces = testBlockUp.rightPieces;
                testBlockLeftPieces = testBlockLeft.rightPieces;
                testBlockRightPieces = testBlockRight.rightPieces;

                testBlockUpX2Pieces = testBlockUpX2.rightPieces;
                testBlockLeftX2Pieces = testBlockLeftX2.rightPieces;
                testBlockRightX2Pieces = testBlockRightX2.rightPieces;
                break;

            case Orientation.flipped:
                testBlockUpPieces = testBlockUp.flippedPieces;
                testBlockLeftPieces = testBlockLeft.flippedPieces;
                testBlockRightPieces = testBlockRight.flippedPieces;

                testBlockUpX2Pieces = testBlockUpX2.flippedPieces;
                testBlockLeftX2Pieces = testBlockLeftX2.flippedPieces;
                testBlockRightX2Pieces = testBlockRightX2.flippedPieces;
                break;
        }

        testBlockUp.name = "testBlockUp";
        testBlockLeft.name = "testBlockLeft";
        testBlockRight.name = "testBlockRight";

        testBlockUpX2.name = "testBlockUpX2";
        testBlockLeftX2.name = "testBlockLeftX2";
        testBlockRightX2.name = "testBlockRightX2";

        testBlockUp.gameObject.layer = 0;
        testBlockLeft.gameObject.layer = 0;
        testBlockRight.gameObject.layer = 0;

        testBlockUpX2.gameObject.layer = 0;
        testBlockLeftX2.gameObject.layer = 0;
        testBlockRightX2.gameObject.layer = 0;

        /*
         * Test moving the mino up
         */
        testBlockUp.MoveUp(1);
        testBlockUp.ActiveMinoOrientation = endOrientation;
        for (int i = 0; i < testBlockUpPieces.Length; i++)
        {
            currentMeshRenderer = testBlockUpPieces[i].GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;
        }

        if (!MinoBlock.IsColliding(testBlockUpPieces))
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
        testBlockLeft.MoveHorizontal(Direction.left, 1);
        testBlockLeft.ActiveMinoOrientation = endOrientation;
        for (int i = 0; i < testBlockLeftPieces.Length; i++)
        {
            currentMeshRenderer = testBlockLeftPieces[i].GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;
        }

        if (!MinoBlock.IsColliding(testBlockLeftPieces))
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
        testBlockRight.MoveHorizontal(Direction.right, 1);
        testBlockRight.ActiveMinoOrientation = endOrientation;
        for (int i = 0; i < testBlockRightPieces.Length; i++)
        {
            currentMeshRenderer = testBlockRightPieces[i].GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;
        }

        if (!MinoBlock.IsColliding(testBlockRightPieces))
        {
            canKickRight = true;
        }
        else
        {
            canKickRight = false;
        }

        /*
         * Test moving the mino up X2
         */

        testBlockUpX2.MoveUp(2);
        testBlockUpX2.ActiveMinoOrientation = endOrientation;
        for (int i = 0; i < testBlockUpX2Pieces.Length; i++)
        {
            currentMeshRenderer = testBlockUpX2Pieces[i].GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;
        }

        if (!MinoBlock.IsColliding(testBlockUpX2Pieces))
        {
            canKickUpX2 = true;
        }
        else
        {
            canKickUpX2 = false;
        }

        /*
         * Test moving the mino left X2
         */
        testBlockLeftX2.MoveHorizontal(Direction.left, 2);
        testBlockLeftX2.ActiveMinoOrientation = endOrientation;
        for (int i = 0; i < testBlockLeftX2Pieces.Length; i++)
        {
            currentMeshRenderer = testBlockLeftX2Pieces[i].GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;
        }

        if (!MinoBlock.IsColliding(testBlockLeftX2Pieces))
        {
            canKickLeftX2 = true;
        }
        else
        {
            canKickLeftX2 = false;
        }

        /*
         * Test moving the mino right X2
         */
        testBlockRightX2.MoveHorizontal(Direction.right, 2);
        testBlockRightX2.ActiveMinoOrientation = endOrientation;
        for (int i = 0; i < testBlockRightX2Pieces.Length; i++)
        {
            currentMeshRenderer = testBlockRightX2Pieces[i].GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;
        }

        if (!MinoBlock.IsColliding(testBlockRightX2Pieces))
        {
            canKickRightX2 = true;
        }
        else
        {
            canKickRightX2 = false;
        }

        // Debug.Log("[Up: " + canKickUp + ", Up2: " + canKickLeftX2 + "] | [" + "Left: " + canKickLeft + ", Left2: " + canKickLeftX2 + "] | ]" + "Right: " + canKickRight + ", Right2: " + canKickRightX2 + "]");

        Destroy(testBlockUp.gameObject);
        Destroy(testBlockLeft.gameObject);
        Destroy(testBlockRight.gameObject);

        Destroy(testBlockUpX2.gameObject);
        Destroy(testBlockLeftX2.gameObject);
        Destroy(testBlockRightX2.gameObject);

        if (!canKickUp && canKickLeft && !canKickRight)
        {
            MoveHorizontal(Direction.left, 1);
            ActiveMinoOrientation = endOrientation;
        }
        else if (!canKickUp && canKickLeftX2 && !canKickRight)
        {
            MoveHorizontal(Direction.left, 2);
            ActiveMinoOrientation = endOrientation;
        }

        if (!canKickUp && !canKickLeft && canKickRight)
        {
            MoveHorizontal(Direction.right, 1);
            ActiveMinoOrientation = endOrientation;
        }
        else if (!canKickUp && !canKickLeft && canKickRightX2)
        {
            MoveHorizontal(Direction.right, 2);
            ActiveMinoOrientation = endOrientation;
        }

        if (canKickUp && !canKickLeft && !canKickRight)
        {
            MoveUp(1);
            ActiveMinoOrientation = endOrientation;
        }
        else if (canKickUpX2 && !canKickLeft && !canKickRight)
        {
            MoveUp(2);
            ActiveMinoOrientation = endOrientation;
        }
    }

    public void RotateMinoBlock(Direction dir)
    {
        // rotate relative to our current orientation
        switch (ActiveMinoOrientation)
        {
            case Orientation.flat:

                switch (dir)
                {
                    case Direction.left:
                        if (!MinoBlock.IsColliding(leftPieces))
                        {
                            ActiveMinoOrientation = Orientation.left;
                        }
                        else
                        {
                            Kick(Orientation.left);
                        }
                        break;

                    case Direction.right:
                        if (!MinoBlock.IsColliding(rightPieces))
                        {
                            ActiveMinoOrientation = Orientation.right;
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
                        if (!MinoBlock.IsColliding(flippedPieces))
                        {
                            ActiveMinoOrientation = Orientation.flipped;
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
                        if (!MinoBlock.IsColliding(flatPieces))
                        {
                            ActiveMinoOrientation = Orientation.flat;
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
                        if (!MinoBlock.IsColliding(flatPieces))
                        {
                            ActiveMinoOrientation = Orientation.flat;
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
                        if (!MinoBlock.IsColliding(flippedPieces))
                        {
                            ActiveMinoOrientation = Orientation.flipped;
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
                        if (!MinoBlock.IsColliding(rightPieces))
                        {
                            ActiveMinoOrientation = Orientation.right;
                        }
                        else
                        {
                            Kick(Orientation.right);
                        }
                        break;

                    case Direction.right: //left
                        if (!MinoBlock.IsColliding(leftPieces))
                        {
                            ActiveMinoOrientation = Orientation.left;
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
