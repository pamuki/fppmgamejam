using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pausescript : MonoBehaviour
{
    public GameObject _pauseMenu;
    public bool isPaused;

    void Start(){
        _pauseMenu.SetActive(false);
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.P)){
            if(isPaused){
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame(){
        _pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame(){
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitGame(){
        Application.Quit();
    }

}
