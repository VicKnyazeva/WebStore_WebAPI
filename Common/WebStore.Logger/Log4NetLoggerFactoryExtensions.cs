using System;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.Logging;

namespace WebStore.Logger
{
    public static class Log4NetLoggerFactoryExtensions
    {
        private static string CheckFilePath(string FilePath)
        {
            if (FilePath is not { Length: > 0 })
                throw new ArgumentException("Не указан путь к файлу");

            if (Path.IsPathRooted(FilePath))
                return FilePath;

            var assembly = Assembly.GetEntryAssembly();
            var dir = Path.GetDirectoryName(assembly!.Location);
            return Path.Combine(dir!, FilePath);
        }

        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string configurationFile = "log4net.config")
        {
            factory.AddProvider(new Log4NetLoggerProvider(CheckFilePath(configurationFile)));
            return factory;
        }
    }
}