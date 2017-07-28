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
    public Row[] rows;
    public GameObject[] minoPrefabs;
    public float gameSpeed = 5;
    [SerializeField] private MinoSpawner minoSpawner;
    public GameObject activeMino;
    public int debugLoops = 1;
    public float debugLoopsDelay = 0f;
    private Rigidbody activeMinoRb;
    private Transform[] activeMinoChildren;
    private MinoBlock activeMinoMinoBlock;

    private float buttonTimer = 0; // Counter time for pressed button
    public float buttonHoldDelay = 3;
    private bool movedOnce;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartGame(debugLoops, debugLoopsDelay));
        MinoBlock.SpawnActiveMino();
    }

    private void Update()
    {
        if (instance.activeMino == null)
        {
            movedOnce = false; //make sure newly spawned Mino still has an input repeat delay
            buttonTimer = 0;
        }
        PlayerInput();
    }

    private void PlayerInput()
    {
        if (instance.activeMino != null) // make sure there's an activeMino in the scene
        {
            activeMinoRb = instance.activeMino.GetComponent<Rigidbody>();
            activeMinoMinoBlock = GameManger.instance.activeMino.GetComponent<MinoBlock>();

            if (Input.GetButton("Left"))
            {
                buttonTimer += Time.deltaTime * 10;
                if (buttonTimer > buttonHoldDelay)
                {
                    activeMinoRb.position = new Vector3((activeMinoRb.position.x + 1), activeMinoRb.position.y, activeMinoRb.position.z);
                }
                else if (!movedOnce)
                {
                    activeMinoRb.position = new Vector3((activeMinoRb.position.x + 1), activeMinoRb.position.y, activeMinoRb.position.z);
                    movedOnce = true;
                }
            }
            else if (Input.GetButtonUp("Left"))
            {
                buttonTimer = 0;
                movedOnce = false;
            }

            if (Input.GetButton("Right"))
            {
                buttonTimer += Time.deltaTime * 10;
                if (buttonTimer > buttonHoldDelay)
                {
                    Debug.Log("Keep moving right");
                    activeMinoRb.position = new Vector3((activeMinoRb.position.x - 1), activeMinoRb.position.y, activeMinoRb.position.z);
                }
                else if (!movedOnce)
                {
                    Debug.Log("Right once");
                    activeMinoRb.position = new Vector3((activeMinoRb.position.x - 1), activeMinoRb.position.y, activeMinoRb.position.z);
                    movedOnce = true;
                }
            }
            else if (Input.GetButtonUp("Right"))
            {
                buttonTimer = 0;
                movedOnce = false;
            }

            if (Input.GetButtonDown("Rotate Left"))
            {
                activeMinoMinoBlock.RotateMinoBlock(MinoRotateDirection.left);
            }

            if (Input.GetButtonDown("Rotate Right"))
            {
                activeMinoMinoBlock.RotateMinoBlock(MinoRotateDirection.right);
            }

        }
    }

    private IEnumerator StartGame(int loops, float seconds)
    {
        for (int i = 0; i < loops; i++)
        {
            MinoBlock.SpawnActiveMino();
            yield return new WaitForSeconds(seconds);
        }
    }
}