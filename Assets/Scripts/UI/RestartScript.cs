using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScript : SingletonPersistent<RestartScript>
{
    public void ResetGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
