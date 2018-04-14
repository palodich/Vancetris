using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWindow : MonoBehaviour
{
    public void Open()
    {
        gameObject.SetActive(true);
        GameManger.Instance.CurrentGameState = GameState.inMenu;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        GameManger.Instance.CurrentGameState = GameState.inGame;
    }
}
