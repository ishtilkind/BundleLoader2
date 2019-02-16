using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NG.TRIPSS.CORE
{
    [System.Serializable]
    public struct Command
    {
        public string action;
        public string type;
        public string parameters;
        public string delimeter;
    }
}
