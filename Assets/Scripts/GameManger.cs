using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Button
{
    left,
    right,
    up,
    down
}

public class GameManger : MonoBehaviour
{
    public static GameManger instance;

    private MinoMovement activeMinoMovement;
    private MinoBlock activeMinoMinoBlock;

    [HideInInspector] public float minoTimer = 0f;
    [Tooltip("Time until a new mino is spawned.")]public float minoSpawnDelay = 1f;

    [HideInInspector] public float gravityTimer = 0f;
    [Tooltip("The time until pieces fall one line.")] public float gravityDelay = 0.75f;
    [Tooltip("How fast the pieces fall when the player holds down.")] public float fastGravityDelay = 0.05f;
    [HideInInspector] public float currentGravityDelay;

    [HideInInspector] public float buttonTimer = 0f;
    [Tooltip("Delay until fast horizontal movement is applied.")] public float buttonHoldDelay = 0.5f;
    [HideInInspector] public bool movedOnce;

    [HideInInspector] public float moveRepeatTimer = 0f;
    [Tooltip("Horizontal movement speed when the left/right button is held down.")] public float moveRepeatDelay = 5f;

    [HideInInspector] public float lockTimer = 0f;
    [Tooltip("How long until a mino touching the floor is locked in place.")] public float lockTimerDelay = 0.5f;

    public LayerMask minoBlockLayerMask;

    public GameObject activeMino;
    public GameObject nextMino1;
    public GameObject nextMino2;
    public GameObject nextMino3;
    public GameObject[] minoPrefabs;

    private float inputHorizontal;
    private float lastInputHorizontal;
    private float inputVertical;
    private bool inputRotateLeft;
    private bool inputRotateRight;

    public Row[] rows;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitMinoQueue();
        minoTimer = minoSpawnDelay; //make a mino drop immediately
    }

    private void Update()
    {
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
                activeMinoMinoBlock = instance.activeMino.GetComponent<MinoBlock>();
                activeMinoMovement = instance.activeMino.GetComponent<MinoMovement>();
            }
        }
        else // if there's an activeMino continue the game loop
        {
            GetPlayerInput();
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

            //activeMinoMinoBlock.SnapToGrid();

            // check to see if there are any lines that need to be cleared
            for (int i = 0; i < instance.rows.Length; i++)
            {
                instance.rows[i].CheckRow();
            }
        }
    }

    private void GetPlayerInput()
    {
        if (instance.activeMino != null) // make sure there's an activeMino in the scene
        {
            // we need the last horiz input to determine if the button has just been pressed, or if it's being held down
            lastInputHorizontal = inputHorizontal;
            inputHorizontal = Input.GetAxis("Horizontal");
            inputVertical = Input.GetAxis("Vertical");
            inputRotateLeft = Input.GetButtonDown("Rotate Left");
            inputRotateRight = Input.GetButtonDown("Rotate Right");

            if (inputHorizontal < 0) // if left input
            {
                moveRepeatTimer += Time.deltaTime * 100f;
                buttonTimer += Time.deltaTime;

                if (buttonTimer > buttonHoldDelay && moveRepeatTimer > moveRepeatDelay && activeMinoMinoBlock.CanMoveHorizontal(Direction.left, activeMinoMinoBlock.activeMinoOrientation) == true)
                {
                    //Debug.Log("move continuously");
                    activeMinoMovement.MoveHorizontal(Direction.left, 1);
                    moveRepeatTimer = 0;
                }
                else if (lastInputHorizontal == 0 && !movedOnce && activeMinoMinoBlock.CanMoveHorizontal(Direction.left, activeMinoMinoBlock.activeMinoOrientation) == true)
                {
                    //Debug.Log("move once");
                    activeMinoMovement.MoveHorizontal(Direction.left, 1);
                    movedOnce = true;
                }
            }

            if (inputHorizontal == 0) // if no horizontal input
            {
                buttonTimer = 0f;
                moveRepeatTimer = 0f;
                movedOnce = false;
            }

            if (inputHorizontal > 0) // if right input
            {
                moveRepeatTimer += Time.deltaTime * 100f;
                buttonTimer += Time.deltaTime;

                if (buttonTimer > buttonHoldDelay && moveRepeatTimer > moveRepeatDelay && activeMinoMinoBlock.CanMoveHorizontal(Direction.right, activeMinoMinoBlock.activeMinoOrientation) == true)
                {
                    //Debug.Log("move continuously");
                    activeMinoMovement.MoveHorizontal(Direction.right, 1);
                    moveRepeatTimer = 0;
                }
                else if (lastInputHorizontal == 0 && !movedOnce && activeMinoMinoBlock.CanMoveHorizontal(Direction.right, activeMinoMinoBlock.activeMinoOrientation) == true)
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

            if (inputVertical < 0) // if down input
            {
                currentGravityDelay = fastGravityDelay;
            }

            if (inputVertical == 0) // if no veritcal input
            {
                currentGravityDelay = gravityDelay;
            }
        }
    }

    private static void InitMinoQueue()
    {
        MinoMovement currentMinoMovement;
        int randomIndex1;
        int randomIndex2;
        int randomIndex3;

        // inital random seed
        randomIndex1 = Random.Range(0, instance.minoPrefabs.Length);
        randomIndex2 = Random.Range(0, instance.minoPrefabs.Length);
        randomIndex3 = Random.Range(0, instance.minoPrefabs.Length);

        // make sure queue is unique
        do
        {
            randomIndex2 = Random.Range(0, instance.minoPrefabs.Length);
        } while (randomIndex2 == randomIndex1);

        do
        {
            randomIndex3 = Random.Range(0, instance.minoPrefabs.Length);
        } while (randomIndex3 == randomIndex1 || randomIndex3 == randomIndex2);

        instance.nextMino1 = Instantiate(instance.minoPrefabs[randomIndex1], MinoPreview1.instance.transform.position, Quaternion.identity);
        instance.nextMino1.name = "NextMino1";
        currentMinoMovement = instance.nextMino1.GetComponent<MinoMovement>();
        currentMinoMovement.SetMinoOrientation(Orientation.flat);

        instance.nextMino2 = Instantiate(instance.minoPrefabs[randomIndex2], MinoPreview2.instance.transform.position, Quaternion.identity);
        instance.nextMino2.name = "NextMino2";
        currentMinoMovement = instance.nextMino2.GetComponent<MinoMovement>();
        currentMinoMovement.SetMinoOrientation(Orientation.flat);

        instance.nextMino3 = Instantiate(instance.minoPrefabs[randomIndex3], MinoPreview3.instance.transform.position, Quaternion.identity);
        instance.nextMino3.name = "NextMino3";
        currentMinoMovement = instance.nextMino3.GetComponent<MinoMovement>();
        currentMinoMovement.SetMinoOrientation(Orientation.flat);
    }

    private static void SpawnActiveMino()
    {
        MinoMovement currentMinoMovement;
        MinoBlock nextMino1minoBlock;
        MinoBlock nextMino2minoBlock;

        if (instance.activeMino == null)
        {
            instance.activeMino = Instantiate(instance.nextMino1, MinoSpawner.instance.transform.position, Quaternion.identity);
            instance.activeMino.name = "ActiveMino";
            currentMinoMovement = instance.activeMino.GetComponent<MinoMovement>();
            currentMinoMovement.SetMinoOrientation(Orientation.flat);

            Destroy(instance.nextMino1);
            instance.nextMino1 = Instantiate(instance.nextMino2, MinoPreview1.instance.transform.position, Quaternion.identity);
            instance.nextMino1.name = "NextMino1";
            currentMinoMovement = instance.nextMino1.GetComponent<MinoMovement>();
            currentMinoMovement.SetMinoOrientation(Orientation.flat);

            Destroy(instance.nextMino2);
            instance.nextMino2 = Instantiate(instance.nextMino3, MinoPreview2.instance.transform.position, Quaternion.identity);
            instance.nextMino2.name = "NextMino2";
            currentMinoMovement = instance.nextMino2.GetComponent<MinoMovement>();
            currentMinoMovement.SetMinoOrientation(Orientation.flat);

            nextMino1minoBlock = instance.nextMino1.GetComponent<MinoBlock>();
            nextMino2minoBlock = instance.nextMino2.GetComponent<MinoBlock>();
            int randomIndex = Random.Range(0, instance.minoPrefabs.Length);

            do
            {
                randomIndex = Random.Range(0, instance.minoPrefabs.Length);
            } while (randomIndex == (int)nextMino1minoBlock.activeMinoType || randomIndex == (int)nextMino2minoBlock.activeMinoType);

            Destroy(instance.nextMino3);
            instance.nextMino3 = Instantiate(instance.minoPrefabs[randomIndex], MinoPreview3.instance.transform.position, Quaternion.identity);
            instance.nextMino3.name = "NextMino3";
            currentMinoMovement = instance.nextMino3.GetComponent<MinoMovement>();
            currentMinoMovement.SetMinoOrientation(Orientation.flat);

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
                    case Orientation.flat:
                        for (int i = 0; i < activeMinoMinoBlock.flatPieces.Length; i++)
                        {
                            activeMinoMinoBlock.flatPieces[i].gameObject.layer = 9;
                        }
                        for (int i = 0; i < activeMinoMinoBlock.leftPieces.Length; i++)
                        {
                            Destroy(activeMinoMinoBlock.leftPieces[i].gameObject);
                        }
                        for (int i = 0; i < activeMinoMinoBlock.rightPieces.Length; i++)
                        {
                            Destroy(activeMinoMinoBlock.rightPieces[i].gameObject);
                        }
                        for (int i = 0; i < activeMinoMinoBlock.flippedPieces.Length; i++)
                        {
                            Destroy(activeMinoMinoBlock.flippedPieces[i].gameObject);
                        }
                        break;

                    case Orientation.left:
                        for (int i = 0; i < activeMinoMinoBlock.flatPieces.Length; i++)
                        {
                            Destroy(activeMinoMinoBlock.flatPieces[i].gameObject);
                        }
                        for (int i = 0; i < activeMinoMinoBlock.leftPieces.Length; i++)
                        {
                            activeMinoMinoBlock.leftPieces[i].gameObject.layer = 9;
                        }
                        for (int i = 0; i < activeMinoMinoBlock.rightPieces.Length; i++)
                        {
                            Destroy(activeMinoMinoBlock.rightPieces[i].gameObject);
                        }
                        for (int i = 0; i < activeMinoMinoBlock.flippedPieces.Length; i++)
                        {
                            Destroy(activeMinoMinoBlock.flippedPieces[i].gameObject);
                        }
                        break;

                    case Orientation.right:
                        for (int i = 0; i < activeMinoMinoBlock.flatPieces.Length; i++)
                        {
                            Destroy(activeMinoMinoBlock.flatPieces[i].gameObject);
                        }
                        for (int i = 0; i < activeMinoMinoBlock.leftPieces.Length; i++)
                        {
                            Destroy(activeMinoMinoBlock.leftPieces[i].gameObject);
                        }
                        for (int i = 0; i < activeMinoMinoBlock.rightPieces.Length; i++)
                        {
                            activeMinoMinoBlock.rightPieces[i].gameObject.layer = 9;
                        }
                        for (int i = 0; i < activeMinoMinoBlock.flippedPieces.Length; i++)
                        {
                            Destroy(activeMinoMinoBlock.flippedPieces[i].gameObject);
                        }
                        break;

                    case Orientation.flipped:
                        for (int i = 0; i < activeMinoMinoBlock.flatPieces.Length; i++)
                        {
                            Destroy(activeMinoMinoBlock.flatPieces[i].gameObject);
                        }
                        for (int i = 0; i < activeMinoMinoBlock.leftPieces.Length; i++)
                        {
                            Destroy(activeMinoMinoBlock.leftPieces[i].gameObject);
                        }
                        for (int i = 0; i < activeMinoMinoBlock.rightPieces.Length; i++)
                        {
                            Destroy(activeMinoMinoBlock.rightPieces[i].gameObject);
                        }
                        for (int i = 0; i < activeMinoMinoBlock.flippedPieces.Length; i++)
                        {
                            activeMinoMinoBlock.flippedPieces[i].gameObject.layer = 9;
                        }
                        break;
                }
                instance.activeMino.layer = 0;

                for (int i = 0; i < activeMinoMinoBlock.flatPieces.Length; i++)
                {
                    activeMinoMinoBlock.flatPieces[i].transform.parent = null;
                }
                for (int i = 0; i < activeMinoMinoBlock.leftPieces.Length; i++)
                {
                    activeMinoMinoBlock.leftPieces[i].transform.parent = null;
                }
                for (int i = 0; i < activeMinoMinoBlock.rightPieces.Length; i++)
                {
                    activeMinoMinoBlock.rightPieces[i].transform.parent = null;
                }
                for (int i = 0; i < activeMinoMinoBlock.flippedPieces.Length; i++)
                {
                    activeMinoMinoBlock.flippedPieces[i].transform.parent = null;
                }

                Destroy(instance.activeMino.gameObject);
            }
        }
    }
}