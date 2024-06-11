using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ControllsScript : MonoBehaviour
{
    [SerializeField] private Button UpButton;
    [SerializeField] private Button DownButton;
    [SerializeField] private Button LeftButton;
    [SerializeField] private Button RightButton;
    [SerializeField] private Button JumpButton;
    [SerializeField] private Button DashButton;
    [SerializeField] private Button PauseButton;

    [SerializeField] private List<InputTextThings> TextList;

    [SerializeField] private GameObject ApplyNotice;
    [SerializeField] private GameObject CoverPanel;

    [SerializeField] private InputManager IManager; 

    private InputManager.PossibleKeys ListenedKey;
    public static bool isWaitingForKey = false;
    private KeyCode savedKeyCode;
    private IEnumerator ListenerCoroutine;

    void Start()
    {
        UpButton.onClick.AddListener(delegate { StartListening(InputManager.PossibleKeys.Up); });
        DownButton.onClick.AddListener(delegate { StartListening(InputManager.PossibleKeys.Down); });
        LeftButton.onClick.AddListener(delegate { StartListening(InputManager.PossibleKeys.Left); });
        RightButton.onClick.AddListener(delegate { StartListening(InputManager.PossibleKeys.Right); });
        JumpButton.onClick.AddListener(delegate { StartListening(InputManager.PossibleKeys.Jump); });
        DashButton.onClick.AddListener(delegate { StartListening(InputManager.PossibleKeys.Dash); });
        PauseButton.onClick.AddListener(delegate { StartListening(InputManager.PossibleKeys.Pause); });
    }

    public void ReloadText()
    {
        List<KeyCode> keys = new List<KeyCode>();
        foreach (InputTextThings ITT in TextList)
        {
            ITT.InputText.text = "";
            keys = IManager.GetInputsFor(ITT.Key);
            foreach(KeyCode key in keys)
            {
                if(keys.IndexOf(key) > 0)
                {
                    ITT.InputText.text += "  ";
                }
                ITT.InputText.text += key.ToString();
            }
        }
    }

    private void StartListening(InputManager.PossibleKeys key)
    {
        ListenerCoroutine = WaitForKeyPress();
        CoverPanel.SetActive(true);
        StartCoroutine(ListenerCoroutine);
        ListenedKey = key;
    }
    IEnumerator WaitForKeyPress()
    {
        isWaitingForKey = true;

        while (isWaitingForKey)
        {
            yield return null; // Wait for the next frame
            if (Input.anyKeyDown)
            {
                if(Input.GetKeyDown(KeyCode.Delete))
                {
                    isWaitingForKey = false;
                    CoverPanel.SetActive(false);
                    ClearKey();
                    NotifyPlayerToPrsssAply();
                    ReloadText();
                    StopCoroutine(ListenerCoroutine);
                    break;
                }
                if(Input.GetKeyDown(KeyCode.Return /*enter*/))
                {
                    isWaitingForKey = false;
                    CoverPanel.SetActive(false);
                    ReloadText();
                    StopCoroutine(ListenerCoroutine);
                    break;
                }
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        savedKeyCode = keyCode;
                        isWaitingForKey = false;
                        CoverPanel.SetActive(false);
                        SaveKeyCode();
                        NotifyPlayerToPrsssAply();
                        ReloadText();
                        StopCoroutine(ListenerCoroutine);
                        break;
                    }
                }
            }
        }
    }
    private void SaveKeyCode()
    {
        IManager.AddKey(ListenedKey, savedKeyCode);
    }
    private void ClearKey()
    {
        IManager.ClearKeys(ListenedKey);
    }
    public void ReloadButtons()
    {
        IManager.LoadAndGetInputsFromJSONFile();
    }
    public void NotifyPlayerToPrsssAply()
    {
        ApplyNotice.SetActive(true);
    }
    public void RemoveApplyNotice()
    {
        ApplyNotice.SetActive(false);
    }
}

[System.Serializable]
public class InputTextThings
{
    public InputManager.PossibleKeys Key;
    public TextMeshProUGUI InputText;
}
