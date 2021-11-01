﻿using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml;

using log4net;

using Microsoft.Extensions.Logging;

namespace WebStore.Logger
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog _Log;

        public Log4NetLogger(string Category, XmlElement Configuration)
        {
            var loggerRepository = LogManager
               .CreateRepository(
                    Assembly.GetEntryAssembly(),
                    typeof(log4net.Repository.Hierarchy.Hierarchy));

            _Log = LogManager.GetLogger(loggerRepository.Name, Category);

            log4net.Config.XmlConfigurator.Configure(Configuration);
        }

        public IDisposable BeginScope<TState>(TState state) => null;
        
        public bool IsEnabled(LogLevel Level) =>
            Level switch
            {
                LogLevel.None => false,
                LogLevel.Trace => _Log.IsDebugEnabled,
                LogLevel.Debug => _Log.IsDebugEnabled,
                LogLevel.Information => _Log.IsInfoEnabled,
                LogLevel.Warning => _Log.IsWarnEnabled,
                LogLevel.Error => _Log.IsErrorEnabled,
                LogLevel.Critical => _Log.IsFatalEnabled,
                _ => throw new InvalidEnumArgumentException(nameof(Level), (int)Level, typeof(LogLevel))
            };

        public void Log<TState>(
            LogLevel Level,
            EventId Id,
            TState State,
            Exception Error,
            Func<TState, Exception, string> Formatter)
        {
            if (Formatter is null)
                throw new ArgumentNullException(nameof(Formatter));

            if (!IsEnabled(Level))
                return;

            var logString = Formatter(State, Error);
            if (string.IsNullOrEmpty(logString) && Error is null)
                return;

            switch (Level)
            {
            default:
                throw new InvalidEnumArgumentException(nameof(Level), (int)Level, typeof(LogLevel));

            case LogLevel.None:
                break;

            case LogLevel.Trace:
            case LogLevel.Debug:
                _Log.Debug(logString);
                break;

            case LogLevel.Information:
                _Log.Info(logString);
                break;

            case LogLevel.Warning:
                _Log.Warn(logString);
                break;

            case LogLevel.Error:
                _Log.Error(logString, Error);
                break;

            case LogLevel.Critical:
                _Log.Fatal(logString, Error);
                break;
            }
        }
    }
}