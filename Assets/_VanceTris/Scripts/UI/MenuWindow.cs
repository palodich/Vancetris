using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWindow : MonoBehaviour
{
    public void Open()
    {
        gameObject.SetActive(true);
        GameManger.instance.CurrentGameState = GameState.inMenu;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        GameManger.instance.CurrentGameState = GameState.inGame;
    }
}
