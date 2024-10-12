using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMaganer : MonoBehaviour
{
    public static GameOverMaganer Instance {get;set;}
    
    public GameObject gameOverCanvas;
    public GameObject uiCanvas;
    public GameObject LoadMenu;
    public GameObject menu;
    public bool isGameOver;

    public int currentFront = 0;

    public int SetAsFront()
    {
        return currentFront++;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if(PlayerState.Instance.currentHealth <= 0)
        {
            uiCanvas.SetActive(false);
            gameOverCanvas.SetActive(true);

            isGameOver = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.Instance.DisableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;
        }  
    }
}
