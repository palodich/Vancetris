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
    private MinoBlock holdMinoMinoBlock;

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
    public GameObject nextMino4;
    public GameObject nextMino5;
    public GameObject holdMino;
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
    private bool swappedHoldMinoLastTurn;
    private bool canSwapHoldMino;

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
        // only proceed through the game loop if we are actually in the game, so the game will be paused
        // if we're in a menu, etc.
        if (CurrentGameState == GameState.inGame && IsGameInProgress())
        {
            GameLoop();
        }
    }

    public void StartNewGame()
    {
        // if there was an existing game we will need to clear it before starting again
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
        MinoBlock nextMino4minoBlock;
        MinoBlock nextMino5minoBlock;
        Outline[] ghostMinoPiecesOutline;
        MeshRenderer[] ghostMinoPieceMeshRenderers;

        if (instance.activeMino == null)
        {
            // create the activeMino by copying the first nextMino
            instance.activeMino = Instantiate(instance.nextMino1, MinoSpawner.instance.transform.position, Quaternion.identity);
            instance.activeMino.name = "ActiveMino";
            activeMinoMinoBlock = instance.activeMino.GetComponent<MinoBlock>();
            activeMinoMinoBlock.SetMinoOrientation(Orientation.flat);
            
            // create the ghostMino, enable the outline script, and disable the renderer, so we'll have a cool "ghost" mino
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

            // if our new activeMino is colliding with an existing piece it's game over man, game over!
            // TODO: game over should probably be when a set mino is above a certain height, rather than checking for a collision
            instance.CheckForGameOver();

            // create the 5 mino previews
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

            Destroy(instance.nextMino3);
            instance.nextMino3 = Instantiate(instance.nextMino4, MinoPreview3.instance.transform.position, Quaternion.identity);
            instance.nextMino3.name = "NextMino3";
            nextMino3minoBlock = instance.nextMino3.GetComponent<MinoBlock>();
            nextMino3minoBlock.SetMinoOrientation(Orientation.flat);

            Destroy(instance.nextMino4);
            instance.nextMino4 = Instantiate(instance.nextMino5, MinoPreview4.instance.transform.position, Quaternion.identity);
            instance.nextMino4.name = "NextMino4";
            nextMino4minoBlock = instance.nextMino4.GetComponent<MinoBlock>();
            nextMino4minoBlock.SetMinoOrientation(Orientation.flat);

            // we'll need to come up with a random mino for the 5th preview. make sure it's not the same as the last mino, so you
            // don't end up with a bunch of repeat pieces

            int randomIndex = Random.Range(0, instance.minoPrefabs.Length);

            while (randomIndex == (int)nextMino4minoBlock.activeMinoType)
            {
                randomIndex = Random.Range(0, instance.minoPrefabs.Length);
            }
            
            Destroy(instance.nextMino5);
            instance.nextMino5 = Instantiate(instance.minoPrefabs[randomIndex], MinoPreview5.instance.transform.position, Quaternion.identity);
            instance.nextMino5.name = "NextMino5";
            nextMino5minoBlock = instance.nextMino5.GetComponent<MinoBlock>();
            nextMino5minoBlock.SetMinoOrientation(Orientation.flat);

            instance.activeMino.layer = 8;

            // prevent swapping if we just swapped a Mino
            if (instance.swappedHoldMinoLastTurn)
            {
                instance.canSwapHoldMino = false;
                instance.swappedHoldMinoLastTurn = false;
            }
            else
            {
                instance.canSwapHoldMino = true;
                instance.swappedHoldMinoLastTurn = false;
            }
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

    public bool IsGameInProgress()
    {
        // if none of our special minos in the scene are null then we know there is a game in progress
        if (instance.nextMino1 != null && instance.nextMino2 != null && instance.nextMino3 != null && instance.nextMino4 != null && instance.nextMino5 != null)
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
        // TODO: add an if-statement so that we only update the rotation if we need to
    
        // have the ghost mino mirror the orientation of the activeMino, and place it in the scene properly
        ghostMinoMinoBlock = instance.ghostMino.GetComponent<MinoBlock>();
        Outline[] ghostMinoPiecesOutline;

        // turn off the current outline...
        ghostMinoPiecesOutline = MinoBlock.GetActiveMinoPieceOutlineComponent(ghostMinoMinoBlock);
        for (int i = 0; i < ghostMinoPiecesOutline.Length; i++)
        {
            ghostMinoPiecesOutline[i].enabled = false;
        }
        
        // have the ghostMino orientation match the activeMino...
        ghostMinoMinoBlock.SetMinoOrientation(activeMinoMinoBlock.activeMinoOrientation);

        // turn on the ghost outline for the new rotation
        ghostMinoPiecesOutline = MinoBlock.GetActiveMinoPieceOutlineComponent(ghostMinoMinoBlock);
        for (int i = 0; i < ghostMinoPiecesOutline.Length; i++)
        {
            ghostMinoPiecesOutline[i].enabled = true;
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

        for (int i = 0; i < fullRows.Count; i++)
        {
            for (int j = fullRows[i] + 1; j < instance.rows.Length; j++)
            {
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

            if (inputHorizontal == 0) // if no horizontal input, reset our timers
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
                SwapHoldMino();
            }
        }
    }

    private void SwapHoldMino()
    {
        MinoType activeMinoType;
        Outline[] ghostMinoPiecesOutline;
        MeshRenderer[] ghostMinoPieceMeshRenderers;

        if (instance.holdMino == null) // if there's no mino being held, just move the active mino there
        {
            instance.holdMino = Instantiate(instance.activeMino, HoldMino.instance.transform.position, Quaternion.identity);
            instance.holdMino.name = "HoldMino";
            holdMinoMinoBlock = instance.holdMino.GetComponent<MinoBlock>();
            holdMinoMinoBlock.SetMinoOrientation(Orientation.flat);

            Destroy(instance.activeMino.gameObject);
            Destroy(instance.ghostMino.gameObject);

            swappedHoldMinoLastTurn = true;
        }
        else // if there's already a held mino we will have to swap it
        {
            if (canSwapHoldMino)
            {
                activeMinoMinoBlock = instance.activeMino.GetComponent<MinoBlock>();
                holdMinoMinoBlock = instance.activeMino.GetComponent<MinoBlock>();

                activeMinoType = activeMinoMinoBlock.activeMinoType;

                Destroy(instance.activeMino.gameObject);
                Destroy(instance.ghostMino.gameObject);

                // TODO: lots of redundant stuff from SpawnActiveMino, may want to make a method just for spawning an active mino anywhere with ghost piece
                // and have this and SpawnActiveMino use that.

                instance.activeMino = Instantiate(instance.holdMino, MinoSpawner.instance.transform.position, Quaternion.identity);
                instance.activeMino.name = "ActiveMino";
                activeMinoMinoBlock = instance.activeMino.GetComponent<MinoBlock>();
                activeMinoMinoBlock.SetMinoOrientation(Orientation.flat);

                instance.ghostMino = Instantiate(instance.holdMino, MinoSpawner.instance.transform.position, Quaternion.identity);
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

                Destroy(instance.holdMino.gameObject);

                instance.holdMino = Instantiate(instance.minoPrefabs[(int)activeMinoType], HoldMino.instance.transform.position, Quaternion.identity);
                instance.holdMino.name = "HoldMino";
                holdMinoMinoBlock = instance.holdMino.GetComponent<MinoBlock>();
                holdMinoMinoBlock.SetMinoOrientation(Orientation.flat);

                swappedHoldMinoLastTurn = false;
                canSwapHoldMino = false;
            }
            else
            {
                Debug.Log("Already swapped! Do nothing");
            }
        }
    }

    private static void InitMinoQueue()
    {
        MinoBlock currentMinoBlock;
        int randomIndex1 = Random.Range(0, instance.minoPrefabs.Length);
        int randomIndex2 = Random.Range(0, instance.minoPrefabs.Length);
        int randomIndex3 = Random.Range(0, instance.minoPrefabs.Length);
        int randomIndex4 = Random.Range(0, instance.minoPrefabs.Length);
        int randomIndex5 = Random.Range(0, instance.minoPrefabs.Length);

        // make sure we don't have a queue that repeats consecutively
        while (randomIndex2 == randomIndex1)
        {
            randomIndex2 = Random.Range(0, instance.minoPrefabs.Length);
        }

        while (randomIndex3 == randomIndex2)
        {
            randomIndex3 = Random.Range(0, instance.minoPrefabs.Length);
        }

        while (randomIndex4 == randomIndex3)
        {
            randomIndex4 = Random.Range(0, instance.minoPrefabs.Length);
        }

        while (randomIndex5 == randomIndex4)
        {
            randomIndex5 = Random.Range(0, instance.minoPrefabs.Length);
        }

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

        instance.nextMino4 = Instantiate(instance.minoPrefabs[randomIndex4], MinoPreview4.instance.transform.position, Quaternion.identity);
        instance.nextMino3.name = "NextMino4";
        currentMinoBlock = instance.nextMino4.GetComponent<MinoBlock>();
        currentMinoBlock.SetMinoOrientation(Orientation.flat);

        instance.nextMino5 = Instantiate(instance.minoPrefabs[randomIndex5], MinoPreview5.instance.transform.position, Quaternion.identity);
        instance.nextMino3.name = "NextMino5";
        currentMinoBlock = instance.nextMino5.GetComponent<MinoBlock>();
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
