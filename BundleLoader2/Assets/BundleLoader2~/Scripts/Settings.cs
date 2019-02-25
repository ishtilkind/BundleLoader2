using UnityEngine;

namespace NG.TRIPSS.CONFIG
{
    [System.Serializable]
    public class Settings : ScriptableObject
    {
        public Color highlightColor = Color.blue;
        public Color missingModelColor = Color.gray;
        public Color cameraBackgroundColor = Color.red;

        public float doubleClickTimeLimit = 0.3f;
        public float longClickTimeLimit = 0.6f;
        public bool useLocalDB = false;
        public NG.TRIPSS.CORE.LOG.LogLevelDisplay logLevel = NG.TRIPSS.CORE.LOG.LogLevelDisplay.WarningErrorsOnly;
    }
}
