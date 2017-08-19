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

    [HideInInspector] public float minoTimer = 0f;
    public float minoSpawnDelay = 1f;

    [HideInInspector] public float gravityTimer = 0f;
    public float gravityDelay = 0.25f;
    public float fastGravityDelay = 0.05f;
    [HideInInspector] public float currentGravityDelay;

    [HideInInspector] public float buttonTimer = 0f;
    public float buttonHoldDelay = 0.5f;
    [HideInInspector] public bool movedOnce;

    [HideInInspector] public float moveRepeatTimer = 0f;
    public float moveRepeatDelay = 0.5f;

    [HideInInspector] public float lockTimer = 0f;
    public float lockTimerDelay = 0.5f;

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
                    instance.LockMino();
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

                if (buttonTimer > buttonHoldDelay && moveRepeatTimer > moveRepeatDelay && activeMinoMinoBlock.CanMoveHorizontal(Direction.left, activeMinoMinoBlock.activeMinoOrientation) == true)
                {
                    //Debug.Log("move continuously");
                    activeMinoMovement.MoveHorizontal(instance.activeMino, Direction.left, 1);
                    moveRepeatTimer = 0;
                }
                else if (lastInputHorizontal == 0 && !movedOnce && activeMinoMinoBlock.CanMoveHorizontal(Direction.left, activeMinoMinoBlock.activeMinoOrientation) == true)
                {
                    //Debug.Log("move once");
                    activeMinoMovement.MoveHorizontal(instance.activeMino, Direction.left, 1);
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

                if (buttonTimer > buttonHoldDelay && moveRepeatTimer > moveRepeatDelay && activeMinoMinoBlock.CanMoveHorizontal(Direction.right, activeMinoMinoBlock.activeMinoOrientation) == true)
                {
                    //Debug.Log("move continuously");
                    activeMinoMovement.MoveHorizontal(instance.activeMino, Direction.right, 1);
                    moveRepeatTimer = 0;
                }
                else if (lastInputHorizontal == 0 && !movedOnce && activeMinoMinoBlock.CanMoveHorizontal(Direction.right, activeMinoMinoBlock.activeMinoOrientation) == true)
                {
                    //Debug.Log("move once");
                    activeMinoMovement.MoveHorizontal(instance.activeMino, Direction.right, 1);
                    movedOnce = true;
                }
            }

            if (inputRotateLeft)
            {
                activeMinoMovement.RotateMinoBlock(instance.activeMino, Direction.left);
            }

            if (inputRotateRight)
            {
                activeMinoMovement.RotateMinoBlock(instance.activeMino, Direction.right);
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
            instance.activeMino.GetComponent<MinoMovement>().SetMinoOrientation(instance.activeMino, MinoOrientation.flat);
            instance.activeMino.layer = 8;
        }
        else { Debug.LogWarning("Only one activeMino can spawn at a time."); }
    }

    public void LockMino()
    {
        if (instance.activeMino != null)
        {
            lockTimer += Time.deltaTime;

            if (lockTimer > lockTimerDelay)
            {
                minoTimer = 0;
                lockTimer = 0;

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
}