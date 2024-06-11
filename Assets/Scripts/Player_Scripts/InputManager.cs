using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class InputObject
{
    public List<KeyCode> Jump = new List<KeyCode>();
    public List<KeyCode> Left = new List<KeyCode>();
    public List<KeyCode> Right = new List<KeyCode>();
    public List<KeyCode> Up = new List<KeyCode>();
    public List<KeyCode> Down = new List<KeyCode>();
    public List<KeyCode> Dash = new List<KeyCode>();
    public List<KeyCode> Pause = new List<KeyCode>();

    public InputObject() 
    {
        //Default inputs
        this.Jump.Add(KeyCode.Space);
        this.Left.Add(KeyCode.A);
        this.Right.Add(KeyCode.D);
        this.Up.Add(KeyCode.W);
        this.Down.Add(KeyCode.S);
        this.Dash.Add(KeyCode.K);
        this.Pause.Add(KeyCode.Escape);
    }
}
public class InputManager : MonoBehaviour
{
    private Dictionary<string, object> jsonData;
    public string FileName = "PlayerInputs.json";
    public string inputFilePath;
    public InputObject inputObject;
    private float x;
    private bool HorizontalInputIsDown;
    private float y;
    private bool VerticalInputIsDown;

    void Awake()
    {
        inputFilePath = Path.Combine(Application.persistentDataPath, FileName);
        
        

        if (!File.Exists(inputFilePath))
        {
            inputObject = new InputObject();
            SaveInputsToJSONFile();
        }

        LoadAndGetInputsFromJSONFile();
    }

    public float HorizontalInput()
    {
        foreach (KeyCode key in inputObject.Left) 
        {
            if(Input.GetKeyDown(key))
            {
                x = -1;
            }
            if (Input.GetKey(key))
            {
                HorizontalInputIsDown = true;
            }
        }
        foreach (KeyCode key in inputObject.Right)
        {
            if (Input.GetKeyDown(key))
            {
                x = 1;
            }
            if (Input.GetKey(key))
            {
                HorizontalInputIsDown = true;
            }
        }
        if(!HorizontalInputIsDown)
        {
            x = 0;
        }
        HorizontalInputIsDown = false;
        return x;
    }
    public float VerticalInput()
    {
        foreach (KeyCode key in inputObject.Down)
        {
            if (Input.GetKeyDown(key))
            {
                y = -1;
            }
            if (Input.GetKey(key))
            {
                VerticalInputIsDown = true;
            }
        }
        foreach (KeyCode key in inputObject.Up)
        {
            if (Input.GetKeyDown(key))
            {
                y = 1;
            }
            if (Input.GetKey(key))
            {
                VerticalInputIsDown = true;
            }
        }
        if (!VerticalInputIsDown)
        {
            y = 0;
        }
        VerticalInputIsDown = false;
        return y;
    }
    public void SaveInputsToJSONFile()
    {
        var inputJSON = JsonUtility.ToJson(inputObject);
        File.WriteAllText(inputFilePath, inputJSON);
    }
    public void LoadAndGetInputsFromJSONFile()
    {
        var outputJSON = File.ReadAllText(inputFilePath);
        InputObject IObject = JsonUtility.FromJson<InputObject>(outputJSON);
        inputObject = IObject;
        IfEmptySetDefaultKey();
    }
    public List<KeyCode> GetInputsFor(PossibleKeys input)
    {
        IfEmptySetDefaultKey();
        if (input == PossibleKeys.Jump)
        {
            return inputObject.Jump;
        }
        if (input == PossibleKeys.Left)
        {
            return inputObject.Left;
        }
        if (input == PossibleKeys.Right)
        {
            return inputObject.Right;
        }
        if (input == PossibleKeys.Up)
        {
            return inputObject.Up;
        }
        if (input == PossibleKeys.Down)
        {
            return inputObject.Down;
        }
        if (input == PossibleKeys.Dash)
        {
            return inputObject.Dash;
        }
        if (input == PossibleKeys.Pause)
        {
            return inputObject.Pause;
        }
        return null;
        
    }
    public void AddKey(PossibleKeys input, KeyCode newKey)
    {
        //Ensures there are no duplicate keys
        List<KeyCode> __keys = GetInputsFor(input);
        foreach(KeyCode __key in __keys)
        {
            if(__key == newKey)
            {
                return;
            }
        }

        if (input == PossibleKeys.Jump)
        {
            inputObject.Jump.Add(newKey);
        }
        if (input == PossibleKeys.Left)
        {
            inputObject.Left.Add(newKey);
        }
        if (input == PossibleKeys.Right)
        {
            inputObject.Right.Add(newKey);
        }
        if (input == PossibleKeys.Up)
        {
            inputObject.Up.Add(newKey);
        }
        if (input == PossibleKeys.Down)
        {
            inputObject.Down.Add(newKey);
        }
        if (input == PossibleKeys.Dash)
        {
            inputObject.Dash.Add(newKey);
        }
        if (input == PossibleKeys.Pause)
        {
            inputObject.Pause.Add(newKey);
        }
    }
    public void ClearKeys(PossibleKeys input)
    {
        if (input == PossibleKeys.Jump)
        {
            inputObject.Jump.Clear();
        }
        if (input == PossibleKeys.Left)
        {
            inputObject.Left.Clear();
        }
        if (input == PossibleKeys.Right)
        {
            inputObject.Right.Clear();
        }
        if (input == PossibleKeys.Up)
        {
            inputObject.Up.Clear();
        }
        if (input == PossibleKeys.Down)
        {
            inputObject.Down.Clear();
        }
        if (input == PossibleKeys.Dash)
        {
            inputObject.Dash.Clear();
        }
        if (input == PossibleKeys.Pause)
        {
            inputObject.Pause.Clear();
        }
    }
    public void IfEmptySetDefaultKey()
    {
        InputObject defaultBindings = new InputObject();
        if (inputObject.Jump.Count <= 0)
        {
            inputObject.Jump = defaultBindings.Jump;
        }
        if (inputObject.Left.Count <= 0)
        {
            inputObject.Left = defaultBindings.Left;
        }
        if (inputObject.Right.Count <= 0)
        {
            inputObject.Right = defaultBindings.Right;
        }
        if (inputObject.Up.Count <= 0)
        {
            inputObject.Up = defaultBindings.Up;
        }
        if (inputObject.Down.Count <= 0)
        {
            inputObject.Down = defaultBindings.Down;
        }
        if (inputObject.Dash.Count <= 0)
        {
            inputObject.Dash = defaultBindings.Dash;
        }
        if (inputObject.Pause.Count <= 0)
        {
            inputObject.Pause = defaultBindings.Pause;
        }
    }
    public void ResetToDefault()
    {
        inputObject = new InputObject();
    }
    public enum PossibleKeys
    {
        Jump,
        Left,
        Right,
        Up,
        Down,
        Dash,
        Pause
    }      
}
