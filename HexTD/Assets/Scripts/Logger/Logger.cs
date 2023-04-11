using System;
using UnityEngine;

namespace Logger
{
    internal static class Logger<T> where T : struct, Enum
    {
        public static LogLevel LogLevel { get; set; }
        public static T SupportedTags { get; set; }

        public static void Trace(T tag, object message, string context)
        {
            if (IsEnabled(LogLevel.Trace))
                Log(LogType.Log, tag, message, context);
        }

        public static void Trace<TObject>(T tag, object message, TObject context)
            where TObject : UnityEngine.Object
        {
            if (IsEnabled(LogLevel.Trace))
                Log(LogType.Log, tag, message, context);
        }

        public static void Trace(T tag, object message, object context = default)
        {
            if (IsEnabled(LogLevel.Trace))
                Log(LogType.Log, tag, message, context);
        }

        public static void Debug(T tag, object message, string context)
        {
            if (IsEnabled(LogLevel.Debug))
                Log(LogType.Log, tag, message, context);
        }

        public static void Debug<TObject>(T tag, object message, TObject context)
            where TObject : UnityEngine.Object
        {
            if (IsEnabled(LogLevel.Debug))
                Log(LogType.Log, tag, message, context);
        }

        public static void Debug(T tag, object message, object context = default)
        {
            if (IsEnabled(LogLevel.Debug))
                Log(LogType.Log, tag, message, context);
        }

        public static void Info(T tag, object message, string context)
        {
            if (IsEnabled(LogLevel.Info))
                Log(LogType.Log, tag, message, context);
        }

        public static void Info<TObject>(T tag, object message, TObject context)
            where TObject : UnityEngine.Object
        {
            if (IsEnabled(LogLevel.Info))
                Log(LogType.Log, tag, message, context);
        }

        public static void Info(T tag, object message, object context = default)
        {
            if (IsEnabled(LogLevel.Info))
                Log(LogType.Log, tag, message, context);
        }

        public static void Warn(T tag, object message, string context)
        {
            if (IsEnabled(LogLevel.Warn))
                Log(LogType.Warning, tag, message, context);
        }

        public static void Warn<TObject>(T tag, object message, TObject context)
            where TObject : UnityEngine.Object
        {
            if (IsEnabled(LogLevel.Warn))
                Log(LogType.Warning, tag, message, context);
        }

        public static void Warn(T tag, object message, object context = default)
        {
            if (IsEnabled(LogLevel.Warn))
                Log(LogType.Warning, tag, message, context);
        }

        public static void Error(T tag, object message, string context)
        {
            if (IsEnabled(LogLevel.Error))
                Log(LogType.Error, tag, message, context);
        }

        public static void Error<TObject>(T tag, object message, TObject context)
            where TObject : UnityEngine.Object
        {
            if (IsEnabled(LogLevel.Error))
                Log(LogType.Error, tag, message, context);
        }

        public static void Error(T tag, object message, object context = default)
        {
            if (IsEnabled(LogLevel.Error))
                Log(LogType.Error, tag, message, context);
        }

        public static void Fatal(T tag, object message, string context)
        {
            if (IsEnabled(LogLevel.Fatal))
                Log(LogType.Exception, tag, message, context);
        }

        public static void Fatal<TObject>(T tag, object message, TObject context)
            where TObject : UnityEngine.Object
        {
            if (IsEnabled(LogLevel.Fatal))
                Log(LogType.Exception, tag, message, context);
        }

        public static void Fatal(T tag, object message, object context = default)
        {
            if (IsEnabled(LogLevel.Fatal))
                Log(LogType.Exception, tag, message, context);
        }

        private static bool IsEnabled(LogLevel level) => LogLevel != LogLevel.Off && level >= LogLevel;

        private static bool IsSupported(T tag) => SupportedTags.HasFlag(tag);

        private static void Log(LogType logType, T tag, object message, string context)
        {
            if (!IsSupported(tag))
                return;
            if (string.IsNullOrWhiteSpace(context))
                UnityEngine.Debug.unityLogger.Log(logType, $"[{tag}] {message}");
            else
                UnityEngine.Debug.unityLogger.Log(
                    logType,
                    context,
                    $"[{tag}] {message}"
                );
        }

        private static void Log<TObject>(LogType logType, T tag, object message, TObject context)
            where TObject : UnityEngine.Object
        {
            if (!IsSupported(tag))
                return;
            if (context != null)
                UnityEngine.Debug.unityLogger.Log(logType, context.GetType().Name, $"[{tag}] {message}", context);
            else
                UnityEngine.Debug.unityLogger.Log(logType, $"[{tag}] {message}");
        }

        private static void Log(LogType logType, T tag, object message, object context)
        {
            if (!IsSupported(tag))
                return;
            if (context != null)
                UnityEngine.Debug.unityLogger.Log(logType, context.GetType().Name, $"[{tag}] {message}");
            else
                UnityEngine.Debug.unityLogger.Log(logType, $"[{tag}] {message}");
        }
    }
}