using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public static class Logger
{
    /// <summary>
    /// 일반 로그 출력
    /// </summary>
    /// <param name="message">출력할 문자열</param>
    [Conditional("DEV_VER")]
    public static void Info(string message) // 일반 로그
    {
        Debug.LogFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), message);
    }

    /// <summary>
    /// 경고 로그 출력
    /// </summary>
    /// <param name="message">출력할 문자열</param>
    [Conditional("DEV_VER")]
    public static void Warning(string message) // 경고 로그
    {
        Debug.LogWarningFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), message);
    }

    /// <summary>
    /// 에러 로그 출력
    /// </summary>
    /// <param name="message">출력할 문자열</param>
    public static void Error(string message)
    {
        Debug.LogErrorFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), message);
    }
}
