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
        SceneManager.LoadScene("Medium Map");
    }

    public void GotoHardMapScene() 
    {
        SceneManager.LoadScene("Hard Map");
    
    }

}
