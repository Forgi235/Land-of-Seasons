using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MenuScripts
{
    [System.Serializable]
    public abstract class MenuAppleHelpClass : MonoBehaviour, ICoinStuff
    {
        public abstract bool isThisACoin();
    }
}
