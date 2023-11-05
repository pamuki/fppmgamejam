using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainmenuScript : MonoBehaviour
{
    public GameObject mainMenu;
    public bool isPaused;

    void Start(){
        Screen.SetResolution(640, 480, FullScreenMode.ExclusiveFullScreen, new RefreshRate() { numerator = 60, denominator = 1 });
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
