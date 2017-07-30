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

    private Rigidbody activeMinoRb;
    private MinoBlock activeMinoMinoBlock;

    private float minoTimer = 0f;
    [SerializeField] private float minoSpawnDelay = 1f;

    private float gravityTimer = 0f;
    [SerializeField] private float gravityDelay = 1f;

    private float buttonTimer = 0f;
    [SerializeField] private float buttonHoldDelay = 3f;
    private bool movedOnce;

    [SerializeField] private MinoSpawner minoSpawner;
    public GameObject activeMino;
    public Row[] rows;
    public GameObject[] minoPrefabs;

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
        if (instance.activeMino == null) //if there's no activeMino spawn one
        {
            minoTimer += Time.deltaTime;

            movedOnce = false; //make sure newly spawned Mino still has an input repeat delay
            buttonTimer = 0;

            if (minoTimer > minoSpawnDelay)
            {
                SpawnActiveMino();
            }
        }
        else // advance the mino one line
        {
            activeMinoRb = instance.activeMino.GetComponent<Rigidbody>();
            activeMinoMinoBlock = GameManger.instance.activeMino.GetComponent<MinoBlock>();

            gravityTimer += Time.deltaTime;

            if (gravityTimer > gravityDelay)
            {
                activeMinoRb.position = new Vector3(activeMinoRb.position.x, (activeMinoRb.position.y - 1), activeMinoRb.position.z);
                gravityTimer = 0;
            }

            activeMinoMinoBlock.CheckBelow();
        }

        PlayerInput();
    }

    private void PlayerInput()
    {

        if (instance.activeMino != null) // make sure there's an activeMino in the scene
        {

            if (Input.GetButton("Left"))
            {
                buttonTimer += Time.deltaTime * 10;
                if (buttonTimer > buttonHoldDelay)
                {
                    if (activeMinoMinoBlock.CanMove(Direction.left))
                    {
                        activeMinoRb.position = new Vector3((activeMinoRb.position.x + 1), activeMinoRb.position.y, activeMinoRb.position.z);
                    }
                }
                else if (!movedOnce)
                {
                    if (activeMinoMinoBlock.CanMove(Direction.left))
                    {
                        activeMinoRb.position = new Vector3((activeMinoRb.position.x + 1), activeMinoRb.position.y, activeMinoRb.position.z);
                        movedOnce = true;
                    }
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
                    if (activeMinoMinoBlock.CanMove(Direction.right))
                    {
                        activeMinoRb.position = new Vector3((activeMinoRb.position.x - 1), activeMinoRb.position.y, activeMinoRb.position.z);
                    }
                }
                else if (!movedOnce)
                {
                    if (activeMinoMinoBlock.CanMove(Direction.right))
                    {
                        activeMinoRb.position = new Vector3((activeMinoRb.position.x - 1), activeMinoRb.position.y, activeMinoRb.position.z);
                        movedOnce = true;
                    }
                }
            }
            else if (Input.GetButtonUp("Right"))
            {
                buttonTimer = 0;
                movedOnce = false;
            }

            if (Input.GetButtonDown("Rotate Left"))
            {
                activeMinoMinoBlock.RotateMinoBlock(Direction.left);
            }

            if (Input.GetButtonDown("Rotate Right"))
            {
                activeMinoMinoBlock.RotateMinoBlock(Direction.right);
            }

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
        }
        else { Debug.LogWarning("Only one activeMino can spawn at a time."); }
    }

    public void ResetMino()
    {
        if (instance.activeMino != null)
        {
            minoTimer = 0;

            foreach (Transform child in instance.activeMino.GetComponent<Transform>())
            {
                child.gameObject.layer = 9;
            }
            //instance.activeMino.GetComponent<Rigidbody>().isKinematic = true;
            instance.activeMino = null;
        }
    }
}