using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    public enum LogLevel
    {
        Trace = 0,
        Info = 1,
        Warn = 2,
        Error = 3,
        None = 4
    }
    public class Logger : ILogger
    {
        private readonly string _path;
        private readonly string _fileLog;
        private readonly LogLevel _level;
        public Logger()
        {
            _path = System.Reflection.Assembly.GetEntryAssembly().Location;
            _path = System.IO.Path.Combine(_path, "Log");
            if(!System.IO.Directory.Exists(_path))
                System.IO.Directory.CreateDirectory(_path);

            _level = Enum.TryParse(ConfigurationManager.AppSettings["LogLevel"], out LogLevel outLogLevel) ? outLogLevel : LogLevel.Info;
            _fileLog = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".Log";
        }
        public void Error(string message)
        {
            WriteLog(LogLevel.Error, message);
        }

        public void Info(string message)
        {
            WriteLog(LogLevel.Info, message);
        }

        public void Trace(string message)
        {
            WriteLog(LogLevel.Trace, message);
        }

        public void Warn(string message)
        {
            WriteLog(LogLevel.Warn, message);
        }

        private void WriteLog(LogLevel level, string message)
        {
            string newLine = $"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")} -> {level.ToString()}: {message}";
            if((int)_level <= (int)level)
            {
                string pathLog = System.IO.Path.Combine(_path, _fileLog);
                if (System.IO.File.Exists(pathLog))
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(pathLog);
                    if (fileInfo.Length >= 10240)
                    {
                        string newFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}&{fileInfo.LastWriteTimeUtc.ToString("yyyy-MM-dd")}&{fileInfo.LastWriteTimeUtc.ToString("HH-mm-ss")}.Log";
                        System.IO.File.Move(pathLog, System.IO.Path.Combine(_path, newFile));
                        System.IO.File.WriteAllText(pathLog, newLine);
                    }
                    else
                    {
                        System.IO.File.AppendAllLines(pathLog, new string[] { newLine });
                    }
                    RemoveOldLog();
                }
                else
                {
                    System.IO.File.WriteAllText(pathLog, newLine);
                }
            }
        }
        private void RemoveOldLog()
        {
            System.IO.DirectoryInfo direcotyInfo = new System.IO.DirectoryInfo(_path);
            System.IO.FileInfo[] fileLogList = direcotyInfo.GetFiles("*.Log", System.IO.SearchOption.TopDirectoryOnly);
            if (fileLogList.Length > 10)
            {
                foreach(var fileLogInfo in fileLogList)
                {
                    if(fileLogInfo.LastWriteTimeUtc < DateTime.UtcNow.AddMinutes(-5))
                    {
                        System.IO.File.Delete(fileLogInfo.FullName);
                    }
                }
            }
        }
    }
}
