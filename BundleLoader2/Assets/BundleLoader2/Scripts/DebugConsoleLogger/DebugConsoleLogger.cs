using System;
using UnityEngine;


namespace NG.TRIPSS.CORE.LOG
{
    /// <summary>
    /// Use Debug Flag Only For Currently Debugged Items.
    /// Remove or change to Info Before Commit
    /// </summary>
    [Flags]
    public enum LogLevel
    {
        Debug = 1,
        Info = 2,
        Warning = 4,
        Error = 8,
    }

    [Flags]
    public enum LogLevelDisplay
    {
        None = 0,
        NoDebug = LogLevel.Info | LogLevel.Warning | LogLevel.Error,
        WarningErrorsOnly = LogLevel.Warning | LogLevel.Error,
        ErrorsOnly = LogLevel.Error,
        ErrorsDebugOnly = LogLevel.Error | LogLevel.Debug,
        NoInfoOnly = WarningErrorsOnly | LogLevel.Debug,
        DebugOnly = LogLevel.Debug,
        All = LogLevel.Debug | LogLevel.Info | LogLevel.Warning | LogLevel.Error
    }

    public class DebugConsoleLogger : MonoBehaviour
    {
        [SerializeField] private LogLevelDisplay logLevelDisplay = LogLevelDisplay.All;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                LogToConsole();
            }
        }

        private void LogToConsole()
        {
            "DEBUG setting".ConsoleLog(LogLevel.Debug, this.logLevelDisplay);
            "INFO setting".ConsoleLog(LogLevel.Info, this.logLevelDisplay);
            "WARNING setting".ConsoleLog(LogLevel.Warning, this.logLevelDisplay);
            "ERROR setting".ConsoleLog(LogLevel.Error, this.logLevelDisplay);
        }
    }

    public static class DebugConsoleLoggerExtensions
    {
        public static void LogIt(this string text, LogLevel localLevel = LogLevel.Debug)
        {
            switch (localLevel)
            {
                case LogLevel.Debug:
                case LogLevel.Info:
                    Debug.Log(text);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(text);
                    break;
                case LogLevel.Error:
                    Debug.LogError(text);
                    break;
                default:
                    throw new Exception($"Handler is undefined for {localLevel.ToString()} LogLevel");
            }
        }
        public static void ConsoleLog(this string text, LogLevel localLevel = LogLevel.Debug, LogLevelDisplay globalLevel = LogLevelDisplay.All)
        {
            switch (globalLevel)
            {
                case LogLevelDisplay.All:
                    text.LogIt(localLevel);
                    break;
                case LogLevelDisplay.NoDebug:
                    if (localLevel != LogLevel.Debug)
                    {
                        text.LogIt(localLevel);
                    }
                    break;
                case LogLevelDisplay.WarningErrorsOnly:
                    if (localLevel != LogLevel.Debug && localLevel != LogLevel.Info)
                    {
                        text.LogIt(localLevel);
                    }
                    break;
                case LogLevelDisplay.NoInfoOnly:
                    if (localLevel != LogLevel.Info)
                    {
                        text.LogIt(localLevel);
                    }
                    break;
                case LogLevelDisplay.ErrorsOnly:
                    if (localLevel == LogLevel.Error)
                    {
                        text.LogIt(localLevel);
                    }
                    break;
                case LogLevelDisplay.ErrorsDebugOnly:
                    if (localLevel == LogLevel.Error || localLevel == LogLevel.Debug)
                    {
                        text.LogIt(localLevel);
                    }
                    break;
                case LogLevelDisplay.DebugOnly:
                    if (localLevel == LogLevel.Debug)
                    {
                        text.LogIt(localLevel);
                    }
                    break;
                case LogLevelDisplay.None:
                    break;
                default:
                    throw new Exception($"Handler is undefined for {globalLevel.ToString()} LogLevelDisplay");
            }
        }
    }
}
