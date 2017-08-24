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

    public GameObject nextMino1;
    public GameObject nextMino2;
    public GameObject nextMino3;

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
        InitMinoQueue();
        minoTimer = minoSpawnDelay; //drop it like it's hot
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
                    activeMinoMovement.MoveDown(instance.activeMino, 1);
                    gravityTimer = 0;
                }
                else
                {
                    //Debug.Log(Time.time + " | " + activeMinoMinoBlock.CanMoveHorizontal(Direction.left, activeMinoMinoBlock.activeMinoOrientation) + " | " + activeMinoMinoBlock.CanMoveHorizontal(Direction.right, activeMinoMinoBlock.activeMinoOrientation));
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

    private static void InitMinoQueue()
    {
        Debug.Log("init");
        MinoMovement minoMovementComponent;
        MinoBlock minoBlockComponent;
        int randomIndex1;
        int randomIndex2;
        int randomIndex3;

        //make sure the queue does not repeat shapes
        randomIndex1 = Random.Range(0, instance.minoPrefabs.Length);
        randomIndex2 = randomIndex1;
        do
        {
            randomIndex2 = Random.Range(0, instance.minoPrefabs.Length);
        } while (randomIndex2 == randomIndex1);
        randomIndex3 = randomIndex2;
        do
        {
            randomIndex3 = Random.Range(0, instance.minoPrefabs.Length);
        } while (randomIndex3 == randomIndex2);

        instance.nextMino1 = Instantiate(instance.minoPrefabs[randomIndex1], MinoPreview1.instance.transform.position, Quaternion.identity);
        minoMovementComponent = instance.nextMino1.GetComponent<MinoMovement>();
        minoMovementComponent.SetMinoOrientation(MinoOrientation.flat);

        instance.nextMino2 = Instantiate(instance.minoPrefabs[randomIndex2], MinoPreview2.instance.transform.position, Quaternion.identity);
        minoMovementComponent = instance.nextMino2.GetComponent<MinoMovement>();
        minoMovementComponent.SetMinoOrientation(MinoOrientation.flat);

        instance.nextMino3 = Instantiate(instance.minoPrefabs[randomIndex3], MinoPreview3.instance.transform.position, Quaternion.identity);
        minoMovementComponent = instance.nextMino3.GetComponent<MinoMovement>();
        minoMovementComponent.SetMinoOrientation(MinoOrientation.flat);
    }

    private static void SpawnActiveMino()
    {
        MinoMovement minoMovementComponent;

        if (instance.nextMino1 == null || instance.nextMino2 == null || instance.nextMino3 == null)
        {
            //InitMinoQueue();
        }

        if (instance.activeMino == null)
        {
            int randomIndex = Random.Range(0, instance.minoPrefabs.Length);

            instance.activeMino = Instantiate(instance.nextMino1, MinoSpawner.instance.transform.position, Quaternion.identity);
            minoMovementComponent = instance.activeMino.GetComponent<MinoMovement>();
            minoMovementComponent.SetMinoOrientation(MinoOrientation.flat);

            Destroy(instance.nextMino1);
            instance.nextMino1 = Instantiate(instance.nextMino2, MinoPreview1.instance.transform.position, Quaternion.identity);
            minoMovementComponent = instance.nextMino1.GetComponent<MinoMovement>();
            minoMovementComponent.SetMinoOrientation(MinoOrientation.flat);

            Destroy(instance.nextMino2);
            instance.nextMino2 = Instantiate(instance.nextMino3, MinoPreview2.instance.transform.position, Quaternion.identity);
            minoMovementComponent = instance.nextMino2.GetComponent<MinoMovement>();
            minoMovementComponent.SetMinoOrientation(MinoOrientation.flat);

            Destroy(instance.nextMino3);
            instance.nextMino3 = Instantiate(instance.minoPrefabs[randomIndex], MinoPreview3.instance.transform.position, Quaternion.identity);
            minoMovementComponent = instance.nextMino3.GetComponent<MinoMovement>();
            minoMovementComponent.SetMinoOrientation(MinoOrientation.flat);

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
                        foreach (GameObject piece in activeMinoMinoBlock.leftPieces)
                        {
                            Destroy(piece.gameObject);
                        }
                        foreach (GameObject piece in activeMinoMinoBlock.rightPieces)
                        {
                            Destroy(piece.gameObject);
                        }
                        foreach (GameObject piece in activeMinoMinoBlock.flippedPieces)
                        {
                            Destroy(piece.gameObject);
                        }
                        break;

                    case MinoOrientation.left:
                        foreach (GameObject piece in activeMinoMinoBlock.flatPieces)
                        {
                            Destroy(piece.gameObject);
                        }
                        foreach (GameObject piece in activeMinoMinoBlock.leftPieces)
                        {
                            piece.gameObject.layer = 9;
                        }
                        foreach (GameObject piece in activeMinoMinoBlock.rightPieces)
                        {
                            Destroy(piece.gameObject);
                        }
                        foreach (GameObject piece in activeMinoMinoBlock.flippedPieces)
                        {
                            Destroy(piece.gameObject);
                        }
                        break;

                    case MinoOrientation.right:
                        foreach (GameObject piece in activeMinoMinoBlock.flatPieces)
                        {
                            Destroy(piece.gameObject);
                        }
                        foreach (GameObject piece in activeMinoMinoBlock.leftPieces)
                        {
                            Destroy(piece.gameObject);
                        }
                        foreach (GameObject piece in activeMinoMinoBlock.rightPieces)
                        {
                            piece.gameObject.layer = 9;
                        }
                        foreach (GameObject piece in activeMinoMinoBlock.flippedPieces)
                        {
                            Destroy(piece.gameObject);
                        }
                        break;

                    case MinoOrientation.flipped:
                        foreach (GameObject piece in activeMinoMinoBlock.flatPieces)
                        {
                            Destroy(piece.gameObject);
                        }
                        foreach (GameObject piece in activeMinoMinoBlock.leftPieces)
                        {
                            Destroy(piece.gameObject);
                        }
                        foreach (GameObject piece in activeMinoMinoBlock.rightPieces)
                        {
                            Destroy(piece.gameObject);
                        }
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