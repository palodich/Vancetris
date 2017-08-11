using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Button
{
    left,
    right,
    up,
    down
}

public class GameManger : MonoBehaviour
{
    public static GameManger instance;

    private Rigidbody activeMinoRigidbody;
    private MinoMovement activeMinoMovement;
    private MinoBlock activeMinoMinoBlock;

    private float minoTimer = 0f;
    [SerializeField] private float minoSpawnDelay = 1f;

    private float gravityTimer = 0f;
    [SerializeField] private float gravityDelay = 0.25f;
    [SerializeField] private float fastGravityDelay = 0.05f;
    private float currentGravityDelay;

    private float buttonTimer = 0f;
    [SerializeField] private float buttonHoldDelay = 0.5f;
    private bool movedOnce;

    private float moveRepeatTimer = 0f;
    [SerializeField] private float moveRepeatDelay = 0.5f;

    [SerializeField] private MinoSpawner minoSpawner;
    public GameObject activeMino;
    public Row[] rows;
    public GameObject[] minoPrefabs;

    private float inputHorizontal;
    private float lastInputHorizontal;
    private float inputVertical;
    private float lastInputVertical;
    private bool inputRotateLeft;
    private bool inputRotateRight;

    public LayerMask minoBlockLayerMask;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        minoTimer = minoSpawnDelay; //drop it like it's hot
    }

    private void Update()
    {
        //Debug.Log(Time.time + " | minoTimer: " + minoTimer + " | gravityTimer: " + gravityTimer + " | buttonTimer: " + buttonTimer);

        if (instance.activeMino == null) //if there's no activeMino spawn one
        {
            minoTimer += Time.deltaTime;

            movedOnce = false; //make sure newly spawned Mino still has an input repeat delay
            moveRepeatTimer = 0;
            buttonTimer = 0;

            if (minoTimer > minoSpawnDelay)
            {
                SpawnActiveMino();
                currentGravityDelay = gravityDelay;
                activeMinoRigidbody = instance.activeMino.GetComponent<Rigidbody>();
                activeMinoMinoBlock = instance.activeMino.GetComponent<MinoBlock>();
                activeMinoMovement = instance.activeMino.GetComponent<MinoMovement>();
            }
        }
        else // if there's an activeMino continue the game loop
        {
            PlayerInput();
            gravityTimer += Time.deltaTime;

            if (gravityTimer > currentGravityDelay)
            {
                if (activeMinoMinoBlock.CanMoveDown())
                {
                    activeMinoMovement.MoveDown(1);
                    gravityTimer = 0;
                }
                else
                {
                    instance.ResetMino();
                }
            }



        }
    }

    private void PlayerInput()
    {
        if (instance.activeMino != null) // make sure there's an activeMino in the scene
        {
            // we need the last horiz input to determine if the button has just been pressed, or if it's being held down
            lastInputHorizontal = inputHorizontal;
            inputHorizontal = Input.GetAxis("Horizontal");
            lastInputVertical = inputVertical;
            inputVertical = Input.GetAxis("Vertical");
            inputRotateLeft = Input.GetButtonDown("Rotate Left");
            inputRotateRight = Input.GetButtonDown("Rotate Right");

            if (inputHorizontal < 0)
            {
                moveRepeatTimer += Time.deltaTime * 100f;
                buttonTimer += Time.deltaTime;

                if (buttonTimer > buttonHoldDelay && moveRepeatTimer > moveRepeatDelay && activeMinoMinoBlock.CanMoveHorizontal(Direction.left) == true)
                {
                    //Debug.Log("move continuously");
                    activeMinoMovement.MoveHorizontal(Direction.left, 1);
                    moveRepeatTimer = 0;
                }
                else if (lastInputHorizontal == 0 && !movedOnce && activeMinoMinoBlock.CanMoveHorizontal(Direction.left) == true)
                {
                    //Debug.Log("move once");
                    activeMinoMovement.MoveHorizontal(Direction.left, 1);
                    movedOnce = true;
                }
            }

            if (inputHorizontal == 0)
            {
                buttonTimer = 0f;
                moveRepeatTimer = 0f;
                movedOnce = false;
            }

            if (inputHorizontal > 0)
            {
                moveRepeatTimer += Time.deltaTime * 100f;
                buttonTimer += Time.deltaTime;

                if (buttonTimer > buttonHoldDelay && moveRepeatTimer > moveRepeatDelay && activeMinoMinoBlock.CanMoveHorizontal(Direction.right) == true)
                {
                    //Debug.Log("move continuously");
                    activeMinoMovement.MoveHorizontal(Direction.right, 1);
                    moveRepeatTimer = 0;
                }
                else if (lastInputHorizontal == 0 && !movedOnce && activeMinoMinoBlock.CanMoveHorizontal(Direction.right) == true)
                {
                    //Debug.Log("move once");
                    activeMinoMovement.MoveHorizontal(Direction.right, 1);
                    movedOnce = true;
                }
            }

            if (inputRotateLeft)
            {
                activeMinoMovement.RotateMinoBlock(Direction.left);
            }

            if (inputRotateRight)
            {
                activeMinoMovement.RotateMinoBlock(Direction.right);
            }

            if (inputVertical < 0)
            {
                currentGravityDelay = fastGravityDelay;
            }

            if (inputVertical == 0)
            {
                currentGravityDelay = gravityDelay;
            }
        }
    }

    public static void SpawnActiveMino()
    {
        if (instance.activeMino == null)
        {
            int randomIndex = Random.Range(0, instance.minoPrefabs.Length);
            instance.activeMino = Instantiate(instance.minoPrefabs[randomIndex], MinoSpawner.instance.transform.position, Quaternion.identity);
            instance.activeMino.GetComponent<MinoMovement>().SetMinoOrientation(MinoOrientation.flat);
            instance.activeMino.layer = 8;
        }
        else { Debug.LogWarning("Only one activeMino can spawn at a time."); }
    }

    public void ResetMino()
    {
        if (instance.activeMino != null)
        {
            minoTimer = 0;

            // Set the whole mino to the default layer, and the minoblocks to the placed mino layer
            switch (activeMinoMinoBlock.activeMinoOrientation)
            {
                case MinoOrientation.flat:
                    foreach (GameObject piece in activeMinoMinoBlock.flatPieces)
                    {
                        piece.gameObject.layer = 9;
                    }
                    break;

                case MinoOrientation.left:
                    foreach (GameObject piece in activeMinoMinoBlock.leftPieces)
                    {
                        piece.gameObject.layer = 9;
                    }
                    break;

                case MinoOrientation.right:
                    foreach (GameObject piece in activeMinoMinoBlock.rightPieces)
                    {
                        piece.gameObject.layer = 9;
                    }
                    break;

                case MinoOrientation.flipped:
                    foreach (GameObject piece in activeMinoMinoBlock.flippedPieces)
                    {
                        piece.gameObject.layer = 9;
                    }
                    break;
            }
            instance.activeMino.layer = 0;

            instance.activeMino = null;
        }
    }
}