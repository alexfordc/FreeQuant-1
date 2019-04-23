﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using FreeQuant.Framework;

namespace FreeQuant.Modules
{
    public class LogModule : BaseModule {
        public override void Start() {
            FqLog.EnginLog("日志模块启动");
        }
    }

    public static class FqLog {
        //
        static FqLog() {
            EventBus.SubscribeLog<Log>(_onLog);
        }

        //
        internal static void EnginLog(string content) {
            Log log = new Log(LogType.Enginlog, content);
            EventBus.PublishLog(log);
        }
        internal static void UserLog(string content) {
            Log log = new Log(LogType.UserLog, content);
            EventBus.PublishLog(log);
        }
        //
        private static void _onLog(Log log) {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string folderName = log.Type.ToString();
            if (!string.IsNullOrEmpty(path)) {
                path = AppDomain.CurrentDomain.BaseDirectory + "\\log\\" + folderName;
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
                    string c = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "-->" + log.Content;
                    Console.WriteLine(c);
                    sw.WriteLine(c);
                    sw.Close();
                }
            }
        }
    }

    internal class Log {
        LogType type;
        string content;

        public Log(LogType type, string content) {
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

    internal enum LogType {
        Enginlog,
        UserLog
    }
}