using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartGame(debugLoops, debugLoopsDelay));
    }

    private void Update()
    {
        bool inputLeft = Input.GetButtonDown("Left");
        bool inputRight = Input.GetButtonDown("Right");
        bool inputRotateLeft = Input.GetButtonDown("Rotate Left");
        bool inputRotateRight = Input.GetButtonDown("Rotate Right");

        
        //input
        if (instance.activeMino != null)
        {
            activeMinoRb = instance.activeMino.GetComponent<Rigidbody>();
            activeMinoMinoBlock = GameManger.instance.activeMino.GetComponent<MinoBlock>();

            if (inputLeft)
            { 
                activeMinoRb.position = new Vector3((activeMinoRb.position.x + 1), activeMinoRb.position.y, activeMinoRb.position.z);
            }
            if (inputRight)
            {
                activeMinoRb.position = new Vector3((activeMinoRb.position.x - 1), activeMinoRb.position.y, activeMinoRb.position.z);
            }
            if (inputRotateLeft)
            {
                activeMinoMinoBlock.RotateMinoBlock(MinoRotateDirection.left);
            }
            if (inputRotateRight)
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
