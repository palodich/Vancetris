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
    [SerializeField] private float gravityDelay = 1f;

    private float buttonTimer = 0f;
    [SerializeField] private float buttonHoldDelay = 3f;
    private bool movedOnce;

    [SerializeField] private MinoSpawner minoSpawner;
    public GameObject activeMino;
    public Row[] rows;
    public GameObject[] minoPrefabs;

    private float inputHorizontal;
    //private float inputVertical;
    private bool inputRotateLeft;
    private bool inputRotateRight;

    public int moveUpCounter = 0;

    [SerializeField] private bool gravityToggle;

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
                gravityToggle = true;
                SpawnActiveMino();
                moveUpCounter = 0;
                activeMinoRigidbody = instance.activeMino.GetComponent<Rigidbody>();
                activeMinoMinoBlock = instance.activeMino.GetComponent<MinoBlock>();
                activeMinoMovement = instance.activeMino.GetComponent<MinoMovement>();
            }
        }
        else // if there's an activeMino continue the game loop
        {
            PlayerInput();
            gravityTimer += Time.deltaTime;

            if (gravityTimer > gravityDelay)
            {
                if (activeMinoMinoBlock.CheckBelow() && gravityToggle == true)
                {
                    Debug.Log("can move & grav on");
                    activeMinoMovement.MoveDown(1);
                    gravityTimer = 0;
                }
                else if (activeMinoMinoBlock.CheckBelow() && gravityToggle == false)
                {
                    Debug.Log("can move, but grav is off");
                    if (moveUpCounter > 0)
                    {
                        activeMinoMovement.MoveUp(1);
                    }
                    instance.ResetMino();
                    gravityTimer = 0;
                }
                else
                {
                    Debug.Log("bye!");
                    if (moveUpCounter > 0)
                    {
                        activeMinoMovement.MoveUp(1);
                    }
                    instance.ResetMino();
                }
            }

            

        }
    }

    private void PlayerInput()
    {
        if (instance.activeMino != null) // make sure there's an activeMino in the scene
        {
            inputHorizontal = Input.GetAxis("Horizontal");
            //inputVertical = Input.GetAxis("Vertical");
            inputRotateLeft = Input.GetButtonDown("Rotate Left");
            inputRotateRight = Input.GetButtonDown("Rotate Right");

            if (inputHorizontal < 0)
            {
                buttonTimer += Time.deltaTime * 10;
                if (buttonTimer > buttonHoldDelay)
                {
                    if (activeMinoMinoBlock.CanMoveHorizontal(Direction.left))
                    {
                        activeMinoMovement.MoveHorizontal(Direction.left, 1);
                    }
                }
                else if (!movedOnce)
                {
                    if (activeMinoMinoBlock.CanMoveHorizontal(Direction.left))
                    {
                        activeMinoMovement.MoveHorizontal(Direction.left, 1);
                        movedOnce = true;
                    }
                }
            }

            if (inputHorizontal == 0)
            {
                buttonTimer = 0;
                movedOnce = false;
            }

            if (inputHorizontal > 0)
            {
                buttonTimer += Time.deltaTime * 10;
                if (buttonTimer > buttonHoldDelay)
                {
                    if (activeMinoMinoBlock.CanMoveHorizontal(Direction.right))
                    {
                        activeMinoMovement.MoveHorizontal(Direction.right, 1);
                    }
                }
                else if (!movedOnce)
                {
                    if (activeMinoMinoBlock.CanMoveHorizontal(Direction.right))
                    {
                        activeMinoMovement.MoveHorizontal(Direction.right, 1);
                        movedOnce = true;
                    }
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

            foreach (Transform child in instance.activeMino.GetComponent<Transform>())
            {
                child.gameObject.layer = 9;
            }
            //instance.activeMino.GetComponent<Rigidbody>().isKinematic = true;
            instance.activeMino = null;
        }
    }
}