using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] private MenuWindow menuWindow;
    [SerializeField] private Button continueButton;

    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        continueButton = continueButton.GetComponent<Button>();
    }

    private void Update()
    {
        if (GameManger.Instance.IsGameInProgress())
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
        switch (GameManger.Instance.CurrentGameState)
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
        GameManger.Instance.StartNewGame();
        menuWindow.Close();
    }

    public void OnQuitGameButton()
    {
        Application.Quit();
        Debug.Log("Quitting Game...");
    }
}
