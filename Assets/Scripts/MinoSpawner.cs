using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoSpawner : MonoBehaviour
{
    public void SpawnActiveMino()
    {
        if (GameManger.instance.activeMino == null)
        {
            int randomIndex = Random.Range(0, GameManger.instance.minoPrefabs.Length);

            GameManger.instance.activeMino = (GameObject)Instantiate(GameManger.instance.minoPrefabs[randomIndex], this.transform.position, Quaternion.identity);

            foreach (Transform child in GameManger.instance.activeMino.GetComponentsInChildren<Transform>(true))
            {
                if (child.tag == "flat")
                {
                    child.gameObject.SetActive(true);
                }
                if (child.tag == "right")
                {
                    child.gameObject.SetActive(false);
                }
            }

            GameManger.instance.activeMino.layer = 8;

            GameManger.instance.activeMino.GetComponent<Rigidbody>().velocity = new Vector3(0, -(GameManger.instance.gameSpeed), 0);
        }
        else { Debug.LogWarning("Only one activeMino can spawn at a time."); }
    }
}
