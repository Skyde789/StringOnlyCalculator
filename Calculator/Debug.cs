using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum LogType
{
    Default = 1 << 0,
    Error = 1 << 1,
}

public static class Debug
{
    static LogType showLogsFrom = LogType.Error | LogType.Default;
    public static void Log(string message, LogType type = LogType.Default)
    {
        string prefix = "";
        switch (type)
        {
            case LogType.Default:
                prefix = "[DEBUG] ";
                break;
            case LogType.Error:
                prefix = "[DEBUG ERROR] ";
                break;
            default:
                break;
        }

        if ((showLogsFrom & type) != 0)
            Console.WriteLine(prefix + message);
    }
}