using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    private bool x = false;

    [SerializeField] InputManager IManager;
    [SerializeField] GameObject PauseMenuUI;
    [SerializeField] GameObject OptionsPanel;
    [SerializeField] GameObject ControlsPanel;
    [SerializeField] ControllsScript ButtonManager;

    private void Start()
    {
        Resume();
        x = true;
    }
    void Update()
    {
        if (!RoomManager.inTransition)
        {
            foreach (KeyCode key in IManager.GetInputsFor(InputManager.PossibleKeys.Pause))
            {
                if (Input.GetKeyDown(key))
                {
                    if (GameIsPaused)
                    {
                        Resume();
                    }
                    else
                    {
                        Pause();
                    }
                }
            }
        }
    }
    public void Resume()
    {
        if (!ControllsScript.isWaitingForKey)
        {
            if (x)
            {
                ResetMenu();
            }
            PauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }
    }
    void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void ResetMenu()
    {
        //same as controls cancel button
        ControlsPanel.SetActive(false);
        OptionsPanel.SetActive(true);
        ButtonManager.ReloadButtons();
        ButtonManager.RemoveApplyNotice();
    }
    public void GoToMainMenu( )
    {
        SceneManager.LoadSceneAsync(0);
    }
}
