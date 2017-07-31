using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinoType
{
    iMino, jMino, lMino, oMino, sMino, tMino, zMino
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
    [SerializeField] private MinoType activeMinoType;
    [SerializeField] private MinoOrientation activeMinoOrientation;
    [SerializeField] private GameObject[] flatPieces;
    [SerializeField] private GameObject[] leftPieces;
    [SerializeField] private GameObject[] rightPieces;
    [SerializeField] private GameObject[] flippedPieces;
    private MinoBlock mb;
    private Vector3 vectorDirection;

    private void Start()
    {

    }

    private void Update()
    {

    }

    public void SetMinoOrientation(MinoOrientation orientation)
    {
        mb = GameManger.instance.activeMino.GetComponent<MinoBlock>();

        //hide all of the pieces
        foreach (GameObject piece in mb.flatPieces)
        {
            piece.gameObject.SetActive(false);
        }
        foreach (GameObject piece in mb.leftPieces)
        {
            piece.gameObject.SetActive(false);
        }
        foreach (GameObject piece in mb.rightPieces)
        {
            piece.gameObject.SetActive(false);
        }
        foreach (GameObject piece in mb.flippedPieces)
        {
            piece.gameObject.SetActive(false);
        }

        //set the pieces that we want to active
        switch (orientation)
        {
            case MinoOrientation.flat:
                foreach (GameObject piece in mb.flatPieces)
                {
                    piece.gameObject.SetActive(true);
                    mb.activeMinoOrientation = MinoOrientation.flat;
                }
                break;

            case MinoOrientation.left:
                foreach (GameObject piece in mb.leftPieces)
                {
                    piece.gameObject.SetActive(true);
                    mb.activeMinoOrientation = MinoOrientation.left;
                }
                break;

            case MinoOrientation.right:
                foreach (GameObject piece in mb.rightPieces)
                {
                    piece.gameObject.SetActive(true);
                    mb.activeMinoOrientation = MinoOrientation.right;
                }
                break;

            case MinoOrientation.flipped:
                foreach (GameObject piece in mb.flippedPieces)
                {
                    piece.gameObject.SetActive(true);
                    mb.activeMinoOrientation = MinoOrientation.flipped;
                }
                break;
        }
    }

    public void RotateMinoBlock(Direction dir)
    {
        mb = GameManger.instance.activeMino.GetComponent<MinoBlock>();

        // rotate relative to our current orientation
        switch (mb.activeMinoOrientation)
        {
            case MinoOrientation.flat:

                switch (dir)
                {
                    case Direction.left:
                        mb.SetMinoOrientation(MinoOrientation.left);
                        break;
                    case Direction.right:
                        mb.SetMinoOrientation(MinoOrientation.right);
                        break;
                }
                break;

            case MinoOrientation.left:

                switch (dir)
                {
                    case Direction.left:
                        mb.SetMinoOrientation(MinoOrientation.flipped);
                        break;
                    case Direction.right:
                        mb.SetMinoOrientation(MinoOrientation.flat);
                        break;
                }
                break;

            case MinoOrientation.right:

                switch (dir)
                {
                    case Direction.left:
                        mb.SetMinoOrientation(MinoOrientation.flat);
                        break;
                    case Direction.right:
                        mb.SetMinoOrientation(MinoOrientation.flipped);
                        break;
                }
                break;

            case MinoOrientation.flipped:

                switch (dir)
                {
                    case Direction.left:
                        mb.SetMinoOrientation(MinoOrientation.right);
                        break;
                    case Direction.right:
                        mb.SetMinoOrientation(MinoOrientation.left);
                        break;
                }
                break;
        }
    }

    public void CheckBelow()
    {
        GameObject[] minoPieces = null;
        mb = GameManger.instance.activeMino.GetComponent<MinoBlock>();

        switch (mb.activeMinoOrientation)
        {
            case MinoOrientation.flat:
                minoPieces = mb.flatPieces;
                break;
            case MinoOrientation.flipped:
                minoPieces = mb.flippedPieces;
                break;
            case MinoOrientation.left:
                minoPieces = mb.leftPieces;
                break;
            case MinoOrientation.right:
                minoPieces = mb.rightPieces;
                break;
        }

        foreach (GameObject child in minoPieces)
        {
            Vector3 forward = child.transform.TransformDirection(Vector3.forward);
            RaycastHit hit;

            int placedMinoLayer = 1 << 9;
            int borderLayer = 1 << 10;

            int layerMask = placedMinoLayer | borderLayer;

            if (Physics.Raycast(child.transform.position, forward, out hit, 1, layerMask))
            {
                //Debug.Log(child.name + " can see " + hit.transform.name);
                GameManger.instance.ResetMino();
            }

            //Debug.DrawRay(child.transform.position, forward * 1, Color.green);
        }
    }

    public bool CanMove(Direction dir)
    {
        GameObject[] minoPieces = null;
        mb = GameManger.instance.activeMino.GetComponent<MinoBlock>();

        switch (mb.activeMinoOrientation)
        {
            case MinoOrientation.flat:
                minoPieces = mb.flatPieces;
                break;
            case MinoOrientation.flipped:
                minoPieces = mb.flippedPieces;
                break;
            case MinoOrientation.left:
                minoPieces = mb.leftPieces;
                break;
            case MinoOrientation.right:
                minoPieces = mb.rightPieces;
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

            int placedMinoLayer = 1 << 9;
            int borderLayer = 1 << 10;

            int layerMask = placedMinoLayer | borderLayer;

            if (Physics.Raycast(child.transform.position, vectorDirection, out hit, 1, layerMask))
            {
                //Debug.Log(child.name + " can see " + hit.transform.name);
                //GameManger.instance.ResetMino();
                return false;
            }
            //Debug.DrawRay(child.transform.position, left * 1, Color.green);
        }

        return true;
    }
}