﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FreeQuant.Framework;

namespace FreeQuant.Components
{
    [Component]
    internal class LogModule {
        public LogModule() {
            Console.WriteLine("日志组件启动");
        }

        [OnLog]
        private void OnEngineError(Error error) {
            LogUtil.EnginLog(error.Exception.Message);
        }
    }

    public class LogUtil {
        private static LogUtil mInstance = new LogUtil();
        private LogUtil()
        {
            EventBus.Register(this);
        }
        //
        public static void EnginLog(string content) {
            LogEvent logEvent = new LogEvent(LogType.Enginlog, content);
            EventBus.PostLog(logEvent);
        }
        public static void UserLog(string content) {
            LogEvent logEvent = new LogEvent(LogType.UserLog, content);
            EventBus.PostLog(logEvent);
        }
        //
        [OnLog]
        private void _onLog(LogEvent logEvent) {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string folderName = logEvent.Type.ToString();
            if (!string.IsNullOrEmpty(path)) {
                path = AppDomain.CurrentDomain.BaseDirectory + "\\logEvent\\" + folderName;
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                path = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!File.Exists(path)) {
                    FileStream fs = File.Create(path);
                    fs.Close();
                }
                if (File.Exists(path)) {
                    StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default);
                    string c = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "-->" + logEvent.Content;
                    Console.WriteLine(c);
                    sw.WriteLine(c);
                    sw.Close();
                }
            }
        }
    }

    public class LogEvent {
        LogType type;
        string content;

        public LogEvent(LogType type, string content) {
            this.type = type;
            this.content = content;
        }

        public LogType Type {
            get {
                return type;
            }
        }

        public string Content {
            get {
                return content;
            }
        }
    }

    public enum LogType {
        Enginlog,
        UserLog
    }
}
