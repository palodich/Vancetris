using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public MenuWindow menuWindow;
    public Button continueButton;

    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        continueButton = continueButton.GetComponent<Button>();
    }

    private void Update()
    {
        if (GameManger.instance.IsGameInProgress())
        {
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            continueButton.gameObject.SetActive(false);
        }
    }

    public void OnMenuButton()
    {
        switch (GameManger.instance.GetGameState())
        {
            case GameState.inGame:
                menuWindow.Open();
                break;

            case GameState.inMenu:
                menuWindow.Close();
                break;
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnCloseButton()
    {
        menuWindow.Close();

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnStartNewGameButton()
    {
        GameManger.instance.StartNewGame();
        menuWindow.Close();
    }

    public void OnQuitGameButton()
    {
        Application.Quit();
        Debug.Log("Quitting Game...");
    }
}
