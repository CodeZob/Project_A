using UnityEngine;
using System.Collections;

public class CTitleManager : MonoBehaviour 
{
    public void OnPlayButton()
    {
        Application.LoadLevel("Play");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
