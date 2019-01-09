using System;
using Microsoft.Extensions.Logging;

namespace Schematic.Core.Mvc
{
    internal static class LoggerExtensions
    {
        private static Action<ILogger, string, int, Exception> _logResourceCreated = LoggerMessage.Define<string, int>(
            logLevel: LogLevel.Information,
            eventId: 1000,
            formatString: "Resource created. Type: {type}. ID: {id}."
        );

        private static Action<ILogger, string, int, Exception> _logResourceDeleted = LoggerMessage.Define<string, int>(
            logLevel: LogLevel.Information,
            eventId: 1001,
            formatString: "Resource deleted. Type: {type}. ID: {id}."
        );
        
        private static Action<ILogger, string, int, Exception> _logResourceUpdated = LoggerMessage.Define<string, int>(
            logLevel: LogLevel.Information,
            eventId: 1002,
            formatString: "Resource updated. Type: {type}. ID: {id}."
        );

        public static void LogResourceCreated(this ILogger logger, Type type, int id) => 
            _logResourceCreated(logger, type.Name, id, null);

        public static void LogResourceDeleted(this ILogger logger, Type type, int id) => 
            _logResourceDeleted(logger, type.Name, id, null);

        public static void LogResourceUpdated(this ILogger logger, Type type, int id) => 
            _logResourceUpdated(logger, type.Name, id, null);
    }
}