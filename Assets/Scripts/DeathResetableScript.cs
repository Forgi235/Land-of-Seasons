using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.World_Scripts;

public class DeathResetableScript : MonoBehaviour
{
    Resetable[] resetables;

    void Start()
    {
        var mObjs = GetComponentsInChildren<MonoBehaviour>();
        resetables = (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(Resetable)) select (Resetable)a).ToArray();
    }

    public void ResetResetable()
    {
        if (resetables != null)
        {
            foreach (Resetable R in resetables)
            {
                R.Reset();
            }
        }
    }
}
