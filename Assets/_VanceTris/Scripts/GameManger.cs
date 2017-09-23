using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public enum DirectionButton
{
    left, right, up, down
}

public enum GameState
{
    inGame, inMenu
}

public class GameManger : MonoBehaviour
{
    public static GameManger instance;

    private MinoBlock activeMinoMinoBlock;
    private MinoBlock ghostMinoMinoBlock;

    [HideInInspector] public float minoTimer = 0f;
    [Tooltip("Time until a new mino is spawned.")] public float minoSpawnDelay = 1f;

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

    [Tooltip("How long a row is 'highlighted' before it is cleared.")] public float rowClearDelay = 1f;

    public LayerMask minoBlockLayerMask;
    public LayerMask rowLayerMask;

    public GameObject activeMino;
    public GameObject ghostMino;
    public GameObject nextMino1;
    public GameObject nextMino2;
    public GameObject nextMino3;
    public GameObject[] minoPrefabs;

    public Row[] rows;

    [SerializeField] private GameState _currentGameState;

    private float inputHorizontal;
    private float lastInputHorizontal;
    private float inputVertical;
    private bool inputSwapHoldMino;
    private bool inputRotateLeft;
    private bool inputRotateRight;

    private bool lineClearInProgress = false;
    private List<int> fullRows;

    private bool firstStart;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CurrentGameState = GameState.inMenu;

        firstStart = true;
    }

    private void Update()
    {
        if (GetGameState() == GameState.inGame && IsGameInProgress())
        {
            GameLoop();
        }
    }

    public void StartNewGame()
    {
        if (!firstStart)
        {
            ClearGame();
        }
        InitMinoQueue();
        minoTimer = minoSpawnDelay; //make a mino drop immediately
        firstStart = false;
    }

    private void ClearGame()
    {
        Destroy(activeMino.gameObject);
        Destroy(ghostMino.gameObject);
        Destroy(nextMino1.gameObject);
        Destroy(nextMino2.gameObject);
        Destroy(nextMino3.gameObject);

        for (int i = 0; i < instance.rows.Length; i++)
        {
            instance.rows[i].DestroyRow();
        }
    }

    private static void SpawnActiveMino()
    {
        MinoBlock activeMinoMinoBlock;
        MinoBlock ghostMinoMinoBlock;
        MinoBlock nextMino1minoBlock;
        MinoBlock nextMino2minoBlock;
        MinoBlock nextMino3minoBlock;
        Outline[] ghostMinoPiecesOutline;
        MeshRenderer[] ghostMinoPieceMeshRenderers;

        if (instance.activeMino == null)
        {
            instance.activeMino = Instantiate(instance.nextMino1, MinoSpawner.instance.transform.position, Quaternion.identity);
            instance.activeMino.name = "ActiveMino";
            activeMinoMinoBlock = instance.activeMino.GetComponent<MinoBlock>();
            activeMinoMinoBlock.SetMinoOrientation(Orientation.flat);

            instance.CheckForGameOver();

            instance.ghostMino = Instantiate(instance.nextMino1, MinoSpawner.instance.transform.position, Quaternion.identity);
            instance.ghostMino.name = "GhostMino";
            ghostMinoMinoBlock = instance.ghostMino.GetComponent<MinoBlock>();
            ghostMinoMinoBlock.SetMinoOrientation(Orientation.flat);
            ghostMinoPiecesOutline = MinoBlock.GetActiveMinoPieceOutlineComponent(ghostMinoMinoBlock);
            for (int i = 0; i < ghostMinoPiecesOutline.Length; i++)
            {
                ghostMinoPiecesOutline[i].enabled = true;
            }
            ghostMinoPieceMeshRenderers = instance.ghostMino.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < ghostMinoPieceMeshRenderers.Length; i++)
            {
                ghostMinoPieceMeshRenderers[i].enabled = false;
            }

            Destroy(instance.nextMino1);
            instance.nextMino1 = Instantiate(instance.nextMino2, MinoPreview1.instance.transform.position, Quaternion.identity);
            instance.nextMino1.name = "NextMino1";
            nextMino1minoBlock = instance.nextMino1.GetComponent<MinoBlock>();
            nextMino1minoBlock.SetMinoOrientation(Orientation.flat);

            Destroy(instance.nextMino2);
            instance.nextMino2 = Instantiate(instance.nextMino3, MinoPreview2.instance.transform.position, Quaternion.identity);
            instance.nextMino2.name = "NextMino2";
            nextMino2minoBlock = instance.nextMino2.GetComponent<MinoBlock>();
            nextMino2minoBlock.SetMinoOrientation(Orientation.flat);

            int randomIndex = Random.Range(0, instance.minoPrefabs.Length);

            do
            {
                randomIndex = Random.Range(0, instance.minoPrefabs.Length);
            } while (randomIndex == (int)nextMino1minoBlock.activeMinoType || randomIndex == (int)nextMino2minoBlock.activeMinoType);

            Destroy(instance.nextMino3);
            instance.nextMino3 = Instantiate(instance.minoPrefabs[randomIndex], MinoPreview3.instance.transform.position, Quaternion.identity);
            instance.nextMino3.name = "NextMino3";
            nextMino3minoBlock = instance.nextMino3.GetComponent<MinoBlock>();
            nextMino3minoBlock.SetMinoOrientation(Orientation.flat);

            instance.activeMino.layer = 8;
        }
        else { Debug.LogWarning("Only one activeMino can spawn at a time."); }
    }

    private void GameLoop()
    {
        if (instance.activeMino == null && lineClearInProgress == false) //if there's no activeMino spawn one
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
            }
        }
        else if (instance.activeMino != null && lineClearInProgress == false) // if there's an activeMino continue the game loop
        {
            GetPlayerInput();
            UpdateGhostMino();
            gravityTimer += Time.deltaTime;

            // move down if we can, otherwise start the lock timer/lock mino/clear rows
            if (gravityTimer > currentGravityDelay)
            {
                if (activeMinoMinoBlock.CanMoveDown())
                {
                    activeMinoMinoBlock.MoveDown(1);
                    gravityTimer = 0;
                }
                else
                {
                    lockTimer += Time.deltaTime;

                    if (lockTimer > lockTimerDelay)
                    {
                        minoTimer = 0;
                        lockTimer = 0;

                        instance.LockActiveMino();
                        Destroy(instance.activeMino.gameObject);
                        Destroy(instance.ghostMino.gameObject);

                        // check to see if there are any lines that need to be cleared
                        fullRows = new List<int>();

                        for (int i = 0; i < instance.rows.Length; i++)
                        {
                            if (instance.rows[i].IsRowFull())
                            {
                                fullRows.Add(i);
                            }
                        }

                        if (fullRows.Count > 0)
                        {
                            StartCoroutine(ClearRows(fullRows));
                        }
                    }
                }
            }
        }
    }

    public GameState CurrentGameState
    {
        get
        {
            return _currentGameState;
        }
        set
        {
            _currentGameState = value;
        }
    }

    public GameState GetGameState()
    {
        return _currentGameState;
    }

    public bool IsGameInProgress()
    {
        if (instance.nextMino1 != null && instance.nextMino2 != null && instance.nextMino3 != null)
        {
            return true;
        }
        else return false;
    }

    private void CheckForGameOver()
    {
        activeMinoMinoBlock = instance.activeMino.GetComponent<MinoBlock>();

        GameObject[] activeMinoPieces = MinoBlock.GetActiveMinoPieces(activeMinoMinoBlock);

        if (MinoBlock.IsColliding(activeMinoPieces))
        {
            Debug.Log("Game over yeah!");
            StartNewGame();
        }

    }

    private void UpdateGhostMino()
    {
        MeshRenderer[] ghostMinoPieceMeshRenderers;
        ghostMinoMinoBlock = instance.ghostMino.GetComponent<MinoBlock>();
        Outline[] ghostMinoPiecesOutline;

        ghostMinoPiecesOutline = MinoBlock.GetActiveMinoPieceOutlineComponent(ghostMinoMinoBlock);
        for (int i = 0; i < ghostMinoPiecesOutline.Length; i++)
        {
            ghostMinoPiecesOutline[i].enabled = false;
        }

        ghostMinoMinoBlock.SetMinoOrientation(activeMinoMinoBlock.activeMinoOrientation);

        ghostMinoPiecesOutline = MinoBlock.GetActiveMinoPieceOutlineComponent(ghostMinoMinoBlock);
        for (int i = 0; i < ghostMinoPiecesOutline.Length; i++)
        {
            ghostMinoPiecesOutline[i].enabled = true;
        }

        // TODO: Disabling the mesh renderer after SetMinoOrientation is stupid, maybe add an option to SetMinoOrientation to rotate but not enable the MeshRenderer

        ghostMinoPieceMeshRenderers = instance.ghostMino.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < ghostMinoPieceMeshRenderers.Length; i++)
        {
            ghostMinoPieceMeshRenderers[i].enabled = false;
        }

        instance.ghostMino.gameObject.transform.position = new Vector3(instance.activeMino.gameObject.transform.position.x, MinoBlock.GetHardDropYPosition(), instance.ghostMino.gameObject.transform.position.z);
    }

    private IEnumerator ClearRows(List<int> fullRows)
    {
        lineClearInProgress = true;

        for (int i = 0; i < fullRows.Count; i++)
        {
            instance.rows[fullRows[i]].HighlightRow();
        }

        yield return new WaitForSeconds(rowClearDelay);

        for (int i = 0; i < fullRows.Count; i++)
        {
            instance.rows[fullRows[i]].DestroyRow();
        }

        //Debug.Log("Cleared " + fullRows.Count + " row(s)");

        for (int i = 0; i < fullRows.Count; i++)
        {
            for (int j = fullRows[i] + 1; j < instance.rows.Length; j++)
            {
                //Debug.Log("Row " + fullRows[i] + " cleared, row " + j + " will be moved down.");

                instance.rows[j - i].MoveRowDown();
            }
        }

        lineClearInProgress = false;
    }

    private void GetPlayerInput()
    {
        if (instance.activeMino != null) // make sure there's an activeMino in the scene
        {
            // we need the last horiz input to determine if the button has just been pressed, or if it's being held down
            lastInputHorizontal = inputHorizontal;
            inputHorizontal = Input.GetAxis("Horizontal");
            inputVertical = Input.GetAxis("Vertical");
            inputSwapHoldMino = Input.GetButtonDown("Swap Hold Mino");
            inputRotateLeft = Input.GetButtonDown("Rotate Left");
            inputRotateRight = Input.GetButtonDown("Rotate Right");

            if (inputHorizontal < 0) // if left input
            {
                moveRepeatTimer += Time.deltaTime * 100f;
                buttonTimer += Time.deltaTime;

                if (buttonTimer > buttonHoldDelay && moveRepeatTimer > moveRepeatDelay && activeMinoMinoBlock.CanMoveHorizontal(Direction.left, activeMinoMinoBlock.activeMinoOrientation) == true)
                {
                    //Debug.Log("move continuously");
                    activeMinoMinoBlock.MoveHorizontal(Direction.left, 1);
                    moveRepeatTimer = 0;
                }
                else if (lastInputHorizontal == 0 && !movedOnce && activeMinoMinoBlock.CanMoveHorizontal(Direction.left, activeMinoMinoBlock.activeMinoOrientation) == true)
                {
                    //Debug.Log("move once");
                    activeMinoMinoBlock.MoveHorizontal(Direction.left, 1);
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
                    activeMinoMinoBlock.MoveHorizontal(Direction.right, 1);
                    moveRepeatTimer = 0;
                }
                else if (lastInputHorizontal == 0 && !movedOnce && activeMinoMinoBlock.CanMoveHorizontal(Direction.right, activeMinoMinoBlock.activeMinoOrientation) == true)
                {
                    //Debug.Log("move once");
                    activeMinoMinoBlock.MoveHorizontal(Direction.right, 1);
                    movedOnce = true;
                }
            }

            if (inputRotateLeft)
            {
                activeMinoMinoBlock.RotateMinoBlock(Direction.left);
            }

            if (inputRotateRight)
            {
                activeMinoMinoBlock.RotateMinoBlock(Direction.right);
            }

            if (inputVertical > 0) // if up input
            {
                activeMinoMinoBlock.HardDrop();
            }

            if (inputVertical == 0) // if no veritcal input
            {
                currentGravityDelay = gravityDelay;
            }

            if (inputVertical < 0) // if down input
            {
                currentGravityDelay = fastGravityDelay;
            }

            if (inputSwapHoldMino)
            {
                Debug.Log("This is when I should swap the hold mino!");
            }
        }
    }

    private static void InitMinoQueue()
    {
        MinoBlock currentMinoBlock;
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
        currentMinoBlock = instance.nextMino1.GetComponent<MinoBlock>();
        currentMinoBlock.SetMinoOrientation(Orientation.flat);

        instance.nextMino2 = Instantiate(instance.minoPrefabs[randomIndex2], MinoPreview2.instance.transform.position, Quaternion.identity);
        instance.nextMino2.name = "NextMino2";
        currentMinoBlock = instance.nextMino2.GetComponent<MinoBlock>();
        currentMinoBlock.SetMinoOrientation(Orientation.flat);

        instance.nextMino3 = Instantiate(instance.minoPrefabs[randomIndex3], MinoPreview3.instance.transform.position, Quaternion.identity);
        instance.nextMino3.name = "NextMino3";
        currentMinoBlock = instance.nextMino3.GetComponent<MinoBlock>();
        currentMinoBlock.SetMinoOrientation(Orientation.flat);
    }

    public void LockActiveMino()
    {
        if (instance.activeMino != null)
        {
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
        }
    }
}