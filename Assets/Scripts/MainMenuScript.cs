using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void VSAi(){
        SceneManager.LoadScene("Split-Screen");
    }

    public void Singleplayer()
    {
        SceneManager.LoadScene("Singleplayer");
    }
}
