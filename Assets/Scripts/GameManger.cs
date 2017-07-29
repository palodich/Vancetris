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

    private float gravityTimer = 0;
    public float gravityDelay = 3;

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
        SpawnActiveMino();
    }

    private void Update()
    {
        if (instance.activeMino == null)
        {
            movedOnce = false; //make sure newly spawned Mino still has an input repeat delay
            buttonTimer = 0;
        }

        StartCoroutine("StartGravity");

        PlayerInput();
    }

    private void PlayerInput()
    {
        Rigidbody activeMinoRb;
        MinoBlock activeMinoMinoBlock;

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
                    activeMinoRb.position = new Vector3((activeMinoRb.position.x - 1), activeMinoRb.position.y, activeMinoRb.position.z);
                }
                else if (!movedOnce)
                {
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
            SpawnActiveMino();
            yield return new WaitForSeconds(seconds);
        }
    }

    public static void SpawnActiveMino()
    {
        if (instance.activeMino == null)
        {
            int randomIndex = Random.Range(0, instance.minoPrefabs.Length);
            instance.activeMino = Instantiate(instance.minoPrefabs[randomIndex], MinoSpawner.instance.transform.position, Quaternion.identity);
            instance.activeMino.GetComponent<MinoBlock>().SetMinoOrientation(MinoOrientation.flat);
            instance.activeMino.layer = 8;
            //instance.activeMino.GetComponent<Rigidbody>().velocity = new Vector3(0, -(instance.gameSpeed), 0);          
        }
        else { Debug.LogWarning("Only one activeMino can spawn at a time."); }
    }

    public IEnumerator StartGravity()
    {
        Rigidbody activeMinoRb;

        if (instance.activeMino != null)
        {
            activeMinoRb = instance.activeMino.GetComponent<Rigidbody>();
            gravityTimer += Time.deltaTime;

            Debug.Log("gravityTimer > gravityDelay | " + gravityTimer + " > " + gravityDelay);

            if (gravityTimer > gravityDelay)
            {
                activeMinoRb.position = new Vector3(activeMinoRb.position.x, (activeMinoRb.position.y - 1), activeMinoRb.position.z);
                gravityTimer = 0;
            }
        }

        yield return null;
    }
}