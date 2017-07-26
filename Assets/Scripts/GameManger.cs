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
        float inputDirection = Input.GetAxis("Horizontal");
        float inputRotateLeft = Input.GetAxis("Rotate Left");
        float inputRotateRight = Input.GetAxis("Rotate Right");

        

        if (instance.activeMino != null)
        {
            activeMinoRb = instance.activeMino.GetComponent<Rigidbody>();
            activeMinoChildren = instance.activeMino.GetComponentsInChildren<Transform>(true);

            if (inputDirection < 0)
            { 
                activeMinoRb.position = new Vector3((activeMinoRb.position.x + 1), activeMinoRb.position.y, activeMinoRb.position.z);
            }
            if (inputDirection > 0)
            {
                activeMinoRb.position = new Vector3((activeMinoRb.position.x - 1), activeMinoRb.position.y, activeMinoRb.position.z);
            }
            if (inputRotateLeft > 0)
            {
                Debug.Log(Time.time + " Rotate left! " + inputRotateLeft);
            }
            if (inputRotateRight > 0)
            {
                foreach (Transform child in activeMinoChildren)
                {
                    if (child.tag == "flat")
                    {
                        child.gameObject.SetActive(false);
                    }
                    if (child.tag == "right")
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }


    }

    private IEnumerator StartGame(int loops, float seconds)
    {
        for (int i = 0; i < loops; i++)
        {
            minoSpawner.SpawnActiveMino();
            yield return new WaitForSeconds(seconds);
        }
    }
}
