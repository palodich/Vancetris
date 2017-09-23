using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

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
    private Vector3 vectorDirection;

    private MeshRenderer currentMeshRenderer;

    public bool CanMoveDown()
    {
        GameObject[] currentMinoPieces = null;
        int counter = 0;

        // get the visible/active mino pieces for our orientation
        switch (activeMinoOrientation)
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

            if (Physics.Raycast(currentMinoPieces[i].transform.position, vectorDirection, out hit, 1, GameManger.instance.minoBlockLayerMask.value))
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

    public void SetMinoOrientation(Orientation orientation)
    {
        //set the pieces that we want to be visible
        switch (orientation)
        {
            case Orientation.flat:
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
                activeMinoOrientation = Orientation.flat;
                break;

            case Orientation.left:
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
                activeMinoOrientation = Orientation.left;
                break;

            case Orientation.right:
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
                activeMinoOrientation = Orientation.right;
                break;

            case Orientation.flipped:
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
                activeMinoOrientation = Orientation.flipped;
                break;
        }
    }

    /* IMinos are special because they need to kicked 2 spaces instead of one
    private void IMinoKick(Direction endDirection, Orientation endOrientation)
    {
        MinoBlock testBlock;

        int counter = 0;

        // Set isColliding to false before instantiate
        for (int i = 0; i < flatPieces.Length; i++)
        {
            currentMinoPiece = flatPieces[i].gameObject.GetComponent<MinoPiece>();
            currentMinoPiece.isColliding = false;
        }

        MinoBlock currentMinoBlock = gameObject.GetComponent<MinoBlock>();

        testBlock = Instantiate(currentMinoBlock, currentMinoBlock.transform.position, Quaternion.identity);

        for (int i = 0; i < testBlock.leftPieces.Length; i++)
        {
            currentMeshRenderer = testBlock.leftPieces[i].GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;
        }

        testBlock.name = "testBlock";

        testBlock.MoveHorizontal(endDirection, 2);

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
    }*/

    public static GameObject[] GetActiveMinoPieces(MinoBlock minoBlock)
    {
        GameObject[] activeMinoPieces = null;

        switch (minoBlock.activeMinoOrientation)
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

        switch (minoBlock.activeMinoOrientation)
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
        MinoBlock activeMinoMinoBlock = GameManger.instance.activeMino.GetComponent<MinoBlock>();

        MeshRenderer currentMeshRenderer;

        GameObject[] testBlockPieces = null;
        GameObject[] activeMinoPieces = null;

        testBlock = Instantiate(activeMinoMinoBlock, new Vector3(activeMinoMinoBlock.transform.position.x, -3f, activeMinoMinoBlock.transform.position.z), Quaternion.identity);

        testBlock.name = "testBlock";
        testBlock.gameObject.layer = 0;

        testBlockPieces = GetActiveMinoPieces(testBlock);
        activeMinoPieces = GetActiveMinoPieces(activeMinoMinoBlock);

        for (int i = 0; i < testBlockPieces.Length; i++)
        {
            currentMeshRenderer = testBlockPieces[i].GetComponent<MeshRenderer>();
            currentMeshRenderer.enabled = false;
        }

        do
        {
            testBlock.MoveUp(1);
        } while (IsColliding(testBlockPieces) == true || IsPathObstructed(testBlockPieces, activeMinoPieces) == true);

        Destroy(testBlock.gameObject);
        return testBlock.transform.position.y;
    }

    public static bool IsPathObstructed(GameObject[] firstPieces, GameObject[] secondPieces)
    {
        int lineCastCounter = 0;

        for (int i = 0; i < firstPieces.Length; i++)
        {
            if (Physics.Linecast(firstPieces[i].transform.position, secondPieces[i].transform.position, GameManger.instance.minoBlockLayerMask))
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

            pieceColliders = Physics.OverlapBox(piecePos, overlapBoxSize, Quaternion.identity, GameManger.instance.minoBlockLayerMask);

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

    public void HardDrop()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, MinoBlock.GetHardDropYPosition(), gameObject.transform.position.z);
        GameManger.instance.lockTimer = GameManger.instance.lockTimerDelay;
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

        MinoBlock currentMinoBlock = gameObject.GetComponent<MinoBlock>();

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
        testBlockUp.MoveUp(1);
        testBlockUp.SetMinoOrientation(endOrientation);
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
        testBlockLeft.SetMinoOrientation(endOrientation);
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
        testBlockRight.SetMinoOrientation(endOrientation);
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

        //Debug.Log("canKickUp: " + canKickUp + " | " + "canKickLeft: " + canKickLeft + " | " + "canKickRight: " + canKickRight);

        Destroy(testBlockUp.gameObject);
        Destroy(testBlockLeft.gameObject);
        Destroy(testBlockRight.gameObject);

        if (!canKickUp && canKickLeft && !canKickRight)
        {
            MoveHorizontal(Direction.left, 1);
            SetMinoOrientation(endOrientation);
        }
        if (!canKickUp && !canKickLeft && canKickRight)
        {
            MoveHorizontal(Direction.right, 1);
            SetMinoOrientation(endOrientation);
        }
        if (canKickUp && !canKickLeft && !canKickRight)
        {
            MoveUp(1);
            SetMinoOrientation(endOrientation);
        }
    }

    public void RotateMinoBlock(Direction dir)
    {
        // rotate relative to our current orientation
        switch (activeMinoOrientation)
        {
            case Orientation.flat:

                switch (dir)
                {
                    case Direction.left:
                        if (!MinoBlock.IsColliding(leftPieces))
                        {
                            SetMinoOrientation(Orientation.left);
                        }
                        else
                        {
                            Kick(Orientation.left);
                        }
                        break;

                    case Direction.right:
                        if (!MinoBlock.IsColliding(rightPieces))
                        {
                            SetMinoOrientation(Orientation.right);
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
                            SetMinoOrientation(Orientation.flipped);
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
                            SetMinoOrientation(Orientation.flat);
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
                            SetMinoOrientation(Orientation.flat);
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
                            SetMinoOrientation(Orientation.flipped);
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
                            SetMinoOrientation(Orientation.right);
                        }
                        else
                        {
                            Kick(Orientation.right);
                        }
                        break;

                    case Direction.right: //left
                        if (!MinoBlock.IsColliding(leftPieces))
                        {
                            SetMinoOrientation(Orientation.left);
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