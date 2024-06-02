//Shameless copy of @jacob.lorentzen's LogHandler in 
//https://github.com/UnityJacob/matchmaker_game_sample

using System;
using System.IO;
using System.Text;
using UnityEngine;

public class LogHandler : ILogHandler
{
    private FileStream m_FileStream;
    private StreamWriter m_StreamWriter;
    private ILogHandler m_DefaultLogHandler = Debug.unityLogger.logHandler;

    /// <summary>
    /// What folder do we post logs to.
    /// </summary>
    public LogHandler(string logDir)
    {
        if (!Directory.Exists(logDir))
            Directory.CreateDirectory(logDir);
        var filePath = $"{logDir}/log_{DateTime.Now:yyyyddM-HHmmss}.log";

        var path = Path.GetFullPath(filePath);
        Debug.Log($"Log file @ {path}");
        m_FileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        m_StreamWriter = new StreamWriter(m_FileStream);

        // Replace the default debug log handler
        Debug.unityLogger.logHandler = this;
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        StringBuilder logString = new StringBuilder(logType.ToString());
        logString.Append(" - ");
        foreach (var o in args)
        {
            logString.Append(o);
            logString.Append(" ");
        }

        logString.AppendLine(".");
        m_StreamWriter.WriteLine(logString.ToString());
        m_StreamWriter.Flush();
        m_DefaultLogHandler.LogFormat(logType, context, format, args);
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        StringBuilder exceptionString = new StringBuilder(exception.ToString());
        exceptionString.Append(" - ");
        exceptionString.AppendLine(exception.Message);
        exceptionString.AppendLine(exception.StackTrace);
        m_StreamWriter.WriteLine(exceptionString.ToString());
        m_StreamWriter.Flush();
        m_DefaultLogHandler.LogException(exception, context);
    }
}
