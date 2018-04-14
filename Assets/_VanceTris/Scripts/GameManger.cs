using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using TMPro;

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
    // TODO Consider making a "Timer" class?
    private float minoTimer = 0f;
    [SerializeField] [Tooltip("Time until a new mino is spawned.")] private float minoSpawnDelay = 1f;
    private float gravityTimer = 0f;
    [SerializeField] [Tooltip("The time until pieces fall one line.")] private float gravityDelay = 0.75f;
    [SerializeField] [Tooltip("How fast the pieces fall when the player holds down.")] private float fastGravityDelay = 0.05f;
    private float currentGravityDelay;
    private float buttonTimer = 0f;
    [SerializeField] [Tooltip("Delay until fast horizontal movement is applied.")] private float buttonHoldDelay = 0.5f;
    private bool movedOnce;
    private float moveRepeatTimer = 0f;
    [SerializeField] [Tooltip("Horizontal movement speed when the left/right button is held down.")] private float moveRepeatDelay = 5f;
    [HideInInspector] public float lockTimer = 0f; //TODO: need to make these private with properties
    [Tooltip("How long until a mino touching the floor is locked in place.")] public float lockTimerDelay = 0.5f;
    [SerializeField] [Tooltip("How long a row is 'highlighted' before it is cleared.")] private float rowClearDelay = 1f;

    //
    // Fields and Properties
    //

    private static GameManger _instance;
    public static GameManger Instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    private MinoBlock activeMinoMinoBlock;
    private MinoBlock ghostMinoMinoBlock;
    private MinoBlock holdMinoMinoBlock;
    private bool lineClearInProgress = false;
    private bool firstStart;
    private bool swappedHoldMinoLastTurn;
    private bool canSwapHoldMino;

    // Set to _minoBlockLayerMask to PlacedMinoLayer and BorderLayer in Unity UI
    [SerializeField] private LayerMask _minoBlockLayerMask;
    public LayerMask MinoBlockLayerMask
    {
        get { return _minoBlockLayerMask; }
        set { _minoBlockLayerMask = value; }
    }

    // Set to _rowLayerMask to PlacedMinoLayer in Unity UI
    [SerializeField] private LayerMask _rowLayerMask;
    public LayerMask RowLayerMask
    {
        get { return _rowLayerMask; }
        set { _rowLayerMask = value; }
    }

    [SerializeField] private GameObject _activeMino;
    public GameObject ActiveMino
    {
        get { return _activeMino; }
        set { _activeMino = value; }
    }

    private GameObject _ghostMino;
    public GameObject GhostMino
    {
        get { return _ghostMino; }
        set { _ghostMino = value; }
    }

    private GameObject _nextMino1;
    public GameObject NextMino1
    {
        get { return _nextMino1; }
        set { _nextMino1 = value; }
    }

    private GameObject _nextMino2;
    public GameObject NextMino2
    {
        get { return _nextMino2; }
        set { _nextMino2 = value; }
    }

    private GameObject _nextMino3;
    public GameObject NextMino3
    {
        get { return _nextMino3; }
        set { _nextMino3 = value; }
    }

    private GameObject _nextMino4;
    public GameObject NextMino4
    {
        get { return _nextMino4; }
        set { _nextMino4 = value; }
    }

    private GameObject _nextMino5;
    public GameObject NextMino5
    {
        get { return _nextMino5; }
        set { _nextMino5 = value; }
    }

    private GameObject _holdMino;
    public GameObject HoldMino
    {
        get { return _holdMino; }
        set { _holdMino = value; }
    }

    [SerializeField] private GameObject[] _minoPrefabs;
    public GameObject[] MinoPrefabs
    {
        get { return _minoPrefabs; }
        set { MinoPrefabs = value; }
    }

    [SerializeField] private Row[] _rows;
    public Row[] Rows
    {
        get { return _rows; }
        set { _rows = value; }
    }

    [SerializeField] private GameState _currentGameState;
    public GameState CurrentGameState
    {
        get { return _currentGameState; }
        set { _currentGameState = value; }
    }

    [SerializeField] private float _currentGameTime;
    [SerializeField] private GameObject gameTimeText;
    private TextMeshPro gameTimeTextTMP;
    public float CurrentGameTime
    {
        get { return _currentGameTime; }
        set
        {
            gameTimeTextTMP = Instance.gameTimeText.GetComponent<TextMeshPro>();

            _currentGameTime = value;
            gameTimeTextTMP.text = string.Format("{0}:{1:00.00}", (int)_currentGameTime / 60, _currentGameTime % 60);
        }
    }

    [SerializeField] private int _currentLinesCleared;
    [SerializeField] private GameObject linesClearedText;
    private TextMeshPro linesClearedTextTMP;
    public int CurrentLinesCleared
    {
        get { return _currentLinesCleared; }
        set
        {
            linesClearedTextTMP = Instance.linesClearedText.GetComponent<TextMeshPro>();

            _currentLinesCleared = value;
            linesClearedTextTMP.text = value.ToString();
        }
    }

    [SerializeField] private int _currentScore;
    [SerializeField] private GameObject scoreText;
    private TextMeshPro scoreTextTMP;
    public int CurrentScore
    {
        get { return _currentScore; }
        set
        {
            scoreTextTMP = Instance.scoreText.GetComponent<TextMeshPro>();

            _currentScore = value;
            scoreTextTMP.text = string.Format("{0:00000000}", _currentScore);
        }
    }

    [SerializeField] private int _currentLevel;
    [SerializeField] private GameObject levelText;
    private TextMeshPro levelTextTMP;

    public int CurrentLevel
    {
        get { return _currentLevel; }
        set
        {
            levelTextTMP = Instance.levelText.GetComponent<TextMeshPro>();

            _currentLevel = value;
            levelTextTMP.text = value.ToString();
        }
    }

    //
    // Methods
    //

    private void Awake()
    {
        Instance = this;
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
        CurrentGameTime = 0;
        CurrentLinesCleared = 0;
        CurrentLevel = 1;
        CurrentScore = 0;
    }

    private void ClearGame()
    {
        Destroy(ActiveMino.gameObject);
        Destroy(GhostMino.gameObject);
        Destroy(NextMino1.gameObject);
        Destroy(NextMino2.gameObject);
        Destroy(NextMino3.gameObject);
        Destroy(NextMino4.gameObject);
        Destroy(NextMino5.gameObject);

        for (int i = 0; i < Instance.Rows.Length; i++)
        {
            Instance.Rows[i].DestroyRow();
        }
    }

    private void SpawnActiveMino()
    {
        MinoBlock nextMino1minoBlock;
        MinoBlock nextMino2minoBlock;
        MinoBlock nextMino3minoBlock;
        MinoBlock nextMino4minoBlock;
        MinoBlock nextMino5minoBlock;
        Outline[] ghostMinoPiecesOutline;
        MeshRenderer[] ghostMinoPieceMeshRenderers;

        if (Instance.ActiveMino == null)
        {
            // create the ActiveMino by copying the first nextMino
            Instance.ActiveMino = Instantiate(Instance.NextMino1, MinoSpawner.Instance.transform.position, Quaternion.identity);
            Instance.ActiveMino.name = "ActiveMino";
            activeMinoMinoBlock = Instance.ActiveMino.GetComponent<MinoBlock>();
            activeMinoMinoBlock.ActiveMinoOrientation = Orientation.flat;
            
            // create the GhostMino, enable the outline script, and disable the renderer, so we'll have a cool "ghost" mino
            Instance.GhostMino = Instantiate(Instance.NextMino1, MinoSpawner.Instance.transform.position, Quaternion.identity);
            Instance.GhostMino.name = "GhostMino";
            ghostMinoMinoBlock = Instance.GhostMino.GetComponent<MinoBlock>();
            ghostMinoMinoBlock.ActiveMinoOrientation = Orientation.flat;
            ghostMinoPiecesOutline = MinoBlock.GetActiveMinoPieceOutlineComponent(ghostMinoMinoBlock);
            for (int i = 0; i < ghostMinoPiecesOutline.Length; i++)
            {
                ghostMinoPiecesOutline[i].enabled = true;
            }
            ghostMinoPieceMeshRenderers = Instance.GhostMino.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < ghostMinoPieceMeshRenderers.Length; i++)
            {
                ghostMinoPieceMeshRenderers[i].enabled = false;
            }

            // if our new ActiveMino is colliding with an existing piece it's game over man, game over!
            // TODO: game over should probably be when a set mino is above a certain height, rather than checking for a collision
            Instance.CheckForGameOver();

            // create the 5 mino previews
            Destroy(Instance.NextMino1);
            Instance.NextMino1 = Instantiate(Instance.NextMino2, MinoPreview1.Instance.transform.position, Quaternion.identity);
            Instance.NextMino1.name = "NextMino1";
            nextMino1minoBlock = Instance.NextMino1.GetComponent<MinoBlock>();
            nextMino1minoBlock.ActiveMinoOrientation = Orientation.flat;

            Destroy(Instance.NextMino2);
            Instance.NextMino2 = Instantiate(Instance.NextMino3, MinoPreview2.Instance.transform.position, Quaternion.identity);
            Instance.NextMino2.name = "NextMino2";
            nextMino2minoBlock = Instance.NextMino2.GetComponent<MinoBlock>();
            nextMino2minoBlock.ActiveMinoOrientation = Orientation.flat;

            Destroy(Instance.NextMino3);
            Instance.NextMino3 = Instantiate(Instance.NextMino4, MinoPreview3.Instance.transform.position, Quaternion.identity);
            Instance.NextMino3.name = "NextMino3";
            nextMino3minoBlock = Instance.NextMino3.GetComponent<MinoBlock>();
            nextMino3minoBlock.ActiveMinoOrientation = Orientation.flat;

            Destroy(Instance.NextMino4);
            Instance.NextMino4 = Instantiate(Instance.NextMino5, MinoPreview4.Instance.transform.position, Quaternion.identity);
            Instance.NextMino4.name = "NextMino4";
            nextMino4minoBlock = Instance.NextMino4.GetComponent<MinoBlock>();
            nextMino4minoBlock.ActiveMinoOrientation = Orientation.flat;

            // we'll need to come up with a random mino for the 5th preview. make sure it's not the same as the last mino, so you
            // don't end up with a bunch of repeat pieces

            int randomIndex = Random.Range(0, Instance.MinoPrefabs.Length);

            while (randomIndex == (int)nextMino4minoBlock.ActiveMinoType)
            {
                randomIndex = Random.Range(0, Instance.MinoPrefabs.Length);
            }
            
            Destroy(Instance.NextMino5);
            Instance.NextMino5 = Instantiate(Instance.MinoPrefabs[randomIndex], MinoPreview5.Instance.transform.position, Quaternion.identity);
            Instance.NextMino5.name = "NextMino5";
            nextMino5minoBlock = Instance.NextMino5.GetComponent<MinoBlock>();
            nextMino5minoBlock.ActiveMinoOrientation = Orientation.flat;

            Instance.ActiveMino.layer = 8;

            // prevent swapping if we just swapped a Mino
            if (Instance.swappedHoldMinoLastTurn)
            {
                Instance.canSwapHoldMino = false;
                Instance.swappedHoldMinoLastTurn = false;
            }
            else
            {
                Instance.canSwapHoldMino = true;
                Instance.swappedHoldMinoLastTurn = false;
            }
        }
        else { Debug.LogWarning("Only one ActiveMino can spawn at a time."); }
    }

    private void GameLoop()
    {
        TMino currentTMino;
        List<int> fullRows;

        CurrentGameTime += Time.deltaTime;

        if (Instance.ActiveMino == null && lineClearInProgress == false) //if there's no ActiveMino spawn one
        {
            minoTimer += Time.deltaTime;

            movedOnce = false; //make sure newly spawned Mino still has an input repeat delay
            moveRepeatTimer = 0;
            buttonTimer = 0;

            if (minoTimer > minoSpawnDelay)
            {
                SpawnActiveMino();
                currentGravityDelay = gravityDelay;
                activeMinoMinoBlock = Instance.ActiveMino.GetComponent<MinoBlock>();
            }
        }
        else if (Instance.ActiveMino != null && lineClearInProgress == false) // if there's an ActiveMino continue the game loop
        {
            GetPlayerInput();
            UpdateGhostMino();
            gravityTimer += Time.deltaTime;

            // move down if we can, otherwise start the lock timer/lock mino/clear Rows
            if (gravityTimer > currentGravityDelay)
            {
                if (activeMinoMinoBlock.CanMoveDown())
                {
                    activeMinoMinoBlock.MoveDown(1);
                    gravityTimer = 0;

                    // if the player is soft dropping increase the score
                    if (currentGravityDelay == fastGravityDelay)
                    {
                        CurrentScore++;
                    }
                }
                else
                {
                    lockTimer += Time.deltaTime;

                    if (lockTimer > lockTimerDelay)
                    {
                        minoTimer = 0;
                        lockTimer = 0;

                        if (activeMinoMinoBlock.ActiveMinoType == MinoType.tMino)
                        {
                            currentTMino = Instance.ActiveMino.GetComponent<TMino>();

                            currentTMino.CheckTSpin(activeMinoMinoBlock.ActiveMinoOrientation);
                        }

                        Instance.LockActiveMino();
                        Destroy(Instance.ActiveMino.gameObject);
                        Destroy(Instance.GhostMino.gameObject);

                        // check to see if there are any lines that need to be cleared
                        fullRows = new List<int>();

                        for (int i = 0; i < Instance.Rows.Length; i++)
                        {
                            if (Instance.Rows[i].IsRowFull())
                            {
                                fullRows.Add(i);
                            }
                        }

                        CurrentLinesCleared += fullRows.Count;

                        switch (fullRows.Count)
                        {
                            case 1:
                                CurrentScore += 100 * CurrentLevel;
                                Debug.Log("Single line clear!");
                                break;

                            case 2:
                                CurrentScore += 300 * CurrentLevel;
                                Debug.Log("Double line clear!");
                                break;

                            case 3:
                                CurrentScore += 500 * CurrentLevel;
                                Debug.Log("Oh baby a triple!");
                                break;

                            case 4:
                                CurrentScore += 800 * CurrentLevel;
                                Debug.Log("Tetris!");
                                break;
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

    public bool IsGameInProgress()
    {
        // if none of our special minos in the scene are null then we know there is a game in progress
        if (Instance.NextMino1 != null && Instance.NextMino2 != null && Instance.NextMino3 != null && Instance.NextMino4 != null && Instance.NextMino5 != null)
        {
            return true;
        }
        else return false;
    }

    private void CheckForGameOver()
    {
        activeMinoMinoBlock = Instance.ActiveMino.GetComponent<MinoBlock>();

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
    
        // have the ghost mino mirror the orientation of the ActiveMino, and place it in the scene properly
        ghostMinoMinoBlock = Instance.GhostMino.GetComponent<MinoBlock>();
        Outline[] ghostMinoPiecesOutline;

        // turn off the current outline...
        ghostMinoPiecesOutline = MinoBlock.GetActiveMinoPieceOutlineComponent(ghostMinoMinoBlock);
        for (int i = 0; i < ghostMinoPiecesOutline.Length; i++)
        {
            ghostMinoPiecesOutline[i].enabled = false;
        }
        
        // have the GhostMino orientation match the ActiveMino...
        ghostMinoMinoBlock.ActiveMinoOrientation = activeMinoMinoBlock.ActiveMinoOrientation;

        // turn on the ghost outline for the new rotation
        ghostMinoPiecesOutline = MinoBlock.GetActiveMinoPieceOutlineComponent(ghostMinoMinoBlock);
        for (int i = 0; i < ghostMinoPiecesOutline.Length; i++)
        {
            ghostMinoPiecesOutline[i].enabled = true;
        }

        Instance.GhostMino.gameObject.transform.position = new Vector3(Instance.ActiveMino.gameObject.transform.position.x, MinoBlock.GetHardDropYPosition(), Instance.GhostMino.gameObject.transform.position.z);
    }

    private IEnumerator ClearRows(List<int> fullRows)
    {
        lineClearInProgress = true;

        for (int i = 0; i < fullRows.Count; i++)
        {
            Instance.Rows[fullRows[i]].HighlightRow();
        }

        yield return new WaitForSeconds(rowClearDelay);

        for (int i = 0; i < fullRows.Count; i++)
        {
            Instance.Rows[fullRows[i]].DestroyRow();
        }

        for (int i = 0; i < fullRows.Count; i++)
        {
            for (int j = fullRows[i] + 1; j < Instance.Rows.Length; j++)
            {
                Instance.Rows[j - i].MoveRowDown();
            }
        }

        lineClearInProgress = false;
    }

    private void GetPlayerInput()
    {
        float inputHorizontal = 0;
        float lastInputHorizontal = 0;
        float inputVertical = 0;
        bool inputSwapHoldMino;
        bool inputRotateLeft;
        bool inputRotateRight;

        if (Instance.ActiveMino != null) // make sure there's an ActiveMino in the scene
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

                if (buttonTimer > buttonHoldDelay && moveRepeatTimer > moveRepeatDelay && activeMinoMinoBlock.CanMoveHorizontal(Direction.left, activeMinoMinoBlock.ActiveMinoOrientation) == true)
                {
                    //Debug.Log("move continuously");
                    activeMinoMinoBlock.MoveHorizontal(Direction.left, 1);
                    moveRepeatTimer = 0;
                }
                else if (lastInputHorizontal == 0 && !movedOnce && activeMinoMinoBlock.CanMoveHorizontal(Direction.left, activeMinoMinoBlock.ActiveMinoOrientation) == true)
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

                if (buttonTimer > buttonHoldDelay && moveRepeatTimer > moveRepeatDelay && activeMinoMinoBlock.CanMoveHorizontal(Direction.right, activeMinoMinoBlock.ActiveMinoOrientation) == true)
                {
                    //Debug.Log("move continuously");
                    activeMinoMinoBlock.MoveHorizontal(Direction.right, 1);
                    moveRepeatTimer = 0;
                }
                else if (lastInputHorizontal == 0 && !movedOnce && activeMinoMinoBlock.CanMoveHorizontal(Direction.right, activeMinoMinoBlock.ActiveMinoOrientation) == true)
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

            if (inputVertical > 0) // if up input (Hard Drop)
            {
                // HardDrop() returns the distance of the drop. That (doubled) is added to the score
                CurrentScore += activeMinoMinoBlock.HardDrop() * 2;
            }

            if (inputVertical == 0) // if no veritcal input
            {
                currentGravityDelay = gravityDelay;
            }

            if (inputVertical < 0) // if down input (Soft Drop)
            {
                // scoring for a soft drop is handled in the game loop
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

        if (Instance.HoldMino == null) // if there's no mino being held, just move the active mino there
        {
            Instance.HoldMino = Instantiate(Instance.ActiveMino, HoldMinoPreview.Instance.transform.position, Quaternion.identity);
            Instance.HoldMino.name = "HoldMinoPreview";
            holdMinoMinoBlock = Instance.HoldMino.GetComponent<MinoBlock>();
            holdMinoMinoBlock.ActiveMinoOrientation = (Orientation.flat);

            Destroy(Instance.ActiveMino.gameObject);
            Destroy(Instance.GhostMino.gameObject);

            swappedHoldMinoLastTurn = true;
        }
        else // if there's already a held mino we will have to swap it
        {
            if (canSwapHoldMino)
            {
                activeMinoMinoBlock = Instance.ActiveMino.GetComponent<MinoBlock>();
                holdMinoMinoBlock = Instance.ActiveMino.GetComponent<MinoBlock>();

                activeMinoType = activeMinoMinoBlock.ActiveMinoType;

                Destroy(Instance.ActiveMino.gameObject);
                Destroy(Instance.GhostMino.gameObject);

                // TODO: lots of redundant stuff from SpawnActiveMino, may want to make a method just for spawning an active mino anywhere with ghost piece
                // and have this and SpawnActiveMino use that.

                Instance.ActiveMino = Instantiate(Instance.HoldMino, MinoSpawner.Instance.transform.position, Quaternion.identity);
                Instance.ActiveMino.name = "ActiveMino";
                activeMinoMinoBlock = Instance.ActiveMino.GetComponent<MinoBlock>();
                activeMinoMinoBlock.ActiveMinoOrientation = (Orientation.flat);

                Instance.GhostMino = Instantiate(Instance.HoldMino, MinoSpawner.Instance.transform.position, Quaternion.identity);
                Instance.GhostMino.name = "GhostMino";
                ghostMinoMinoBlock = Instance.GhostMino.GetComponent<MinoBlock>();
                ghostMinoMinoBlock.ActiveMinoOrientation = (Orientation.flat);
                ghostMinoPiecesOutline = MinoBlock.GetActiveMinoPieceOutlineComponent(ghostMinoMinoBlock);
                for (int i = 0; i < ghostMinoPiecesOutline.Length; i++)
                {
                    ghostMinoPiecesOutline[i].enabled = true;
                }
                ghostMinoPieceMeshRenderers = Instance.GhostMino.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < ghostMinoPieceMeshRenderers.Length; i++)
                {
                    ghostMinoPieceMeshRenderers[i].enabled = false;
                }

                Destroy(Instance.HoldMino.gameObject);

                Instance.HoldMino = Instantiate(Instance.MinoPrefabs[(int)activeMinoType], HoldMinoPreview.Instance.transform.position, Quaternion.identity);
                Instance.HoldMino.name = "HoldMinoPreview";
                holdMinoMinoBlock = Instance.HoldMino.GetComponent<MinoBlock>();
                holdMinoMinoBlock.ActiveMinoOrientation = (Orientation.flat);

                swappedHoldMinoLastTurn = false;
                canSwapHoldMino = false;
            }
            else
            {
                Debug.Log("Already swapped! Do nothing");
            }
        }
    }

    private void InitMinoQueue()
    {
        MinoBlock currentMinoBlock;
        int randomIndex1 = Random.Range(0, Instance.MinoPrefabs.Length);
        int randomIndex2 = Random.Range(0, Instance.MinoPrefabs.Length);
        int randomIndex3 = Random.Range(0, Instance.MinoPrefabs.Length);
        int randomIndex4 = Random.Range(0, Instance.MinoPrefabs.Length);
        int randomIndex5 = Random.Range(0, Instance.MinoPrefabs.Length);

        // make sure we don't have a queue that repeats consecutively
        while (randomIndex2 == randomIndex1)
        {
            randomIndex2 = Random.Range(0, Instance.MinoPrefabs.Length);
        }

        while (randomIndex3 == randomIndex2)
        {
            randomIndex3 = Random.Range(0, Instance.MinoPrefabs.Length);
        }

        while (randomIndex4 == randomIndex3)
        {
            randomIndex4 = Random.Range(0, Instance.MinoPrefabs.Length);
        }

        while (randomIndex5 == randomIndex4)
        {
            randomIndex5 = Random.Range(0, Instance.MinoPrefabs.Length);
        }

        Instance.NextMino1 = Instantiate(Instance.MinoPrefabs[randomIndex1], MinoPreview1.Instance.transform.position, Quaternion.identity);
        Instance.NextMino1.name = "NextMino1";
        currentMinoBlock = Instance.NextMino1.GetComponent<MinoBlock>();
        currentMinoBlock.ActiveMinoOrientation = (Orientation.flat);

        Instance.NextMino2 = Instantiate(Instance.MinoPrefabs[randomIndex2], MinoPreview2.Instance.transform.position, Quaternion.identity);
        Instance.NextMino2.name = "NextMino2";
        currentMinoBlock = Instance.NextMino2.GetComponent<MinoBlock>();
        currentMinoBlock.ActiveMinoOrientation = (Orientation.flat);

        Instance.NextMino3 = Instantiate(Instance.MinoPrefabs[randomIndex3], MinoPreview3.Instance.transform.position, Quaternion.identity);
        Instance.NextMino3.name = "NextMino3";
        currentMinoBlock = Instance.NextMino3.GetComponent<MinoBlock>();
        currentMinoBlock.ActiveMinoOrientation = (Orientation.flat);

        Instance.NextMino4 = Instantiate(Instance.MinoPrefabs[randomIndex4], MinoPreview4.Instance.transform.position, Quaternion.identity);
        Instance.NextMino3.name = "NextMino4";
        currentMinoBlock = Instance.NextMino4.GetComponent<MinoBlock>();
        currentMinoBlock.ActiveMinoOrientation = (Orientation.flat);

        Instance.NextMino5 = Instantiate(Instance.MinoPrefabs[randomIndex5], MinoPreview5.Instance.transform.position, Quaternion.identity);
        Instance.NextMino3.name = "NextMino5";
        currentMinoBlock = Instance.NextMino5.GetComponent<MinoBlock>();
        currentMinoBlock.ActiveMinoOrientation = (Orientation.flat);
    }

    public void LockActiveMino()
    {
        if (Instance.ActiveMino != null)
        {
            // Set the whole mino to the default layer, and the minoblocks to the placed mino layer
            switch (activeMinoMinoBlock.ActiveMinoOrientation)
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

            Instance.ActiveMino.layer = 0;

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