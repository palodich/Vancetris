using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinoType
{
    iMino,
    jMino,
    lMino,
    oMino,
    sMino,
    tMino,
    zMino
}

public enum MinoOrientation
{
    flat,
    left,
    right,
    flipped
}

public enum MinoRotateDirection
{
    left,
    right
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

    private void Start()
    {

    }

    public static void SpawnActiveMino()
    {
        if (GameManger.instance.activeMino == null)
        {
            int randomIndex = Random.Range(0, GameManger.instance.minoPrefabs.Length);
            GameManger.instance.activeMino = Instantiate(GameManger.instance.minoPrefabs[randomIndex], MinoSpawner.instance.transform.position, Quaternion.identity);
            GameManger.instance.activeMino.GetComponent<MinoBlock>().SetMinoOrientation(MinoOrientation.flat);
            GameManger.instance.activeMino.layer = 8;
            GameManger.instance.activeMino.GetComponent<Rigidbody>().velocity = new Vector3(0, -(GameManger.instance.gameSpeed), 0);
        }
        else { Debug.LogWarning("Only one activeMino can spawn at a time."); }
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

    public void RotateMinoBlock(MinoRotateDirection dir)
    {
        mb = GameManger.instance.activeMino.GetComponent<MinoBlock>();

        switch (mb.activeMinoOrientation)
        {
            case MinoOrientation.flat:

                switch (dir)
                {
                    case MinoRotateDirection.left:
                        mb.SetMinoOrientation(MinoOrientation.left);
                        break;
                    case MinoRotateDirection.right:
                        mb.SetMinoOrientation(MinoOrientation.right);
                        break;
                }
                break;

            case MinoOrientation.left:

                switch (dir)
                {
                    case MinoRotateDirection.left:
                        mb.SetMinoOrientation(MinoOrientation.flipped);
                        break;
                    case MinoRotateDirection.right:
                        mb.SetMinoOrientation(MinoOrientation.flat);
                        break;
                }
                break;

            case MinoOrientation.right:

                switch (dir)
                {
                    case MinoRotateDirection.left:
                        mb.SetMinoOrientation(MinoOrientation.flat);
                        break;
                    case MinoRotateDirection.right:
                        mb.SetMinoOrientation(MinoOrientation.flipped);
                        break;
                }
                break;

            case MinoOrientation.flipped:

                switch (dir)
                {
                    case MinoRotateDirection.left:
                        mb.SetMinoOrientation(MinoOrientation.right);
                        break;
                    case MinoRotateDirection.right:
                        mb.SetMinoOrientation(MinoOrientation.left);
                        break;
                }
                break;
        }
    }
}