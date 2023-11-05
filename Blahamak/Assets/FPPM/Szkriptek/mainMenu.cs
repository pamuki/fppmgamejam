using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainmenuScript : MonoBehaviour
{
    public GameObject mainMenu;
    public bool isPaused;

    void Start(){
        mainMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    void Update(){
        if(isPaused){
            if(Input.GetKeyDown(KeyCode.Return)){
                    ResumeGame();
        } 
        }
    }

    public void ResumeGame(){
        mainMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    
}
