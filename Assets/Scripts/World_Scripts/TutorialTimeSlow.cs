using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

[System.Serializable]
public class RequredKeysOptions
{
    
    public InputManager.PossibleKeys RequredKey;
    public RequredKeysOptions.options Option;

    public enum options
    {
        KeyPressed,
        KeyHeld
    }
}
public class TutorialTimeSlow : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    public PlayerMovement PM;
    private IEnumerator TimeSlowCoroutine;
    private bool InTimeSlow;
    private bool TutorialTriggered;
    private bool TutorialCompleted;
    private bool ParametersSet = false;

    public GameObject objject;
    private bool gone = false;

    [SerializeField] List<RequredKeysOptions> RequredKeys;
    private List<List<KeyCode>> TriggerKeys;
    private List<bool> TriggerKeysTriggered;
    private List<int> TriggerCount;
    private bool x;

    public Collider2D collider_;

    private void Start()
    {
        TimeSlowCoroutine = timeSlow();
        InTimeSlow = false;
        TutorialTriggered = false;
        ParametersSet = false;
        TutorialCompleted = false;
        TriggerKeys = new List<List<KeyCode>>();
        TriggerKeysTriggered = new List<bool>();
        TriggerCount = new List<int>();
    }
    // Update is called once per frame
    void Update()
    {
        if(TutorialCompleted && !gone)
        {
            gone = true;
            objject.SetActive(false);
            return;
        }
        if (InTimeSlow)
        {
            for (int i = 0; i < TriggerKeys.Count; i++)
            {
                foreach (KeyCode key in TriggerKeys[i])
                {
                    if (TriggerCount[i] < 0)
                    {
                        TriggerCount[i] = 0;
                    }
                    if (RequredKeys[i].Option == RequredKeysOptions.options.KeyPressed)
                    {
                        if (Input.GetKeyDown(key))
                        {
                            TriggerCount[i]++;
                        }
                        if (Input.GetKeyUp(key))
                        {
                            TriggerCount[i]--;
                        }
                    }
                    else if (RequredKeys[i].Option == RequredKeysOptions.options.KeyHeld)
                    {
                        if (Input.GetKey(key))
                        {
                            TriggerCount[i]++;
                        }
                        else
                        {
                            TriggerCount[i]--;
                        }
                    }
                    if (TriggerCount[i] > 0)
                    {
                        TriggerKeysTriggered[i] = true;
                    }
                    else
                    {
                        TriggerKeysTriggered[i] = false;
                    }
                }
            }
            x = true;
            foreach(bool a in TriggerKeysTriggered)
            {
                if (!a)
                {
                    x = false;
                }
            }
            if (x || PM.startedDieing)
            {
                StopCoroutine(TimeSlowCoroutine);
                TimeSlowCoroutine = timeSlow();
                Time.timeScale = 1;
                InTimeSlow = false;
                if (x)
                {
                    TutorialCompleted = true;
                }
                if (PM.startedDieing)
                {
                    TutorialTriggered = false;
                    TutorialCompleted = false;
                }
            }
            
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !TutorialTriggered && !TutorialCompleted)
        {
            collider_ = collision;
            if (!ParametersSet)
            { 
                PM = collider_.GetComponent<PlayerMovement>();
                TriggerKeys = new List<List<KeyCode>>();
                foreach (RequredKeysOptions KeyOptions in RequredKeys)
                {
                    InputManager.PossibleKeys requredKey = KeyOptions.RequredKey;
                    TriggerKeys.Add(inputManager.GetInputsFor(requredKey));
                    TriggerKeysTriggered.Add(false);
                    TriggerCount.Add(0);
                }
                ParametersSet = true;
            }
            TutorialTriggered = true;
            InTimeSlow = true;
            StartCoroutine(TimeSlowCoroutine);
        }
    }
    private IEnumerator timeSlow()
    {
        yield return new WaitForSeconds(0.01f);
        Time.timeScale = 0.9f;
        yield return new WaitForSeconds(0.01f);
        Time.timeScale = 0.8f;
        yield return new WaitForSeconds(0.02f);
        Time.timeScale = 0.7f;
        yield return new WaitForSeconds(0.02f);
        Time.timeScale = 0.6f;
        yield return new WaitForSeconds(0.03f);
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(0.03f);
        Time.timeScale = 0.4f;
        yield return new WaitForSeconds(0.04f);
        Time.timeScale = 0.3f;
        yield return new WaitForSeconds(0.04f);
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(0.05f);
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.05f);
        Time.timeScale = 0f;
    }
}
