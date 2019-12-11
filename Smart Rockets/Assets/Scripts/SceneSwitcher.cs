using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
   public void GotoEasyMapScene()
    {
        SceneManager.LoadScene("Easy Map");
    }

    public void GotoMediumMapScene()
    {
        SceneManager.LoadScene("Med Map");
    }

    public void GotoHardMapScene() 
    {
        SceneManager.LoadScene("Hard Map");
    
    }

    public void GotoMainMenuScene()
    {
        SceneManager.LoadScene("Start Menu");

    }

    public void GotoSpiralMap(){
        SceneManager.LoadScene("Spiral Map");
        }

    public void GotoRandomSpaceJunk()
    {
        SceneManager.LoadScene("Random Space Junk");
    }
}
