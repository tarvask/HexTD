using UnityEngine;

namespace Logger
{
    public static class Log
    {
        static Log()
        {
            LogLevel = (LogLevel)PlayerPrefs.GetInt("log_level", 0);
            SupportedTags = (LogTag)PlayerPrefs.GetInt("log_tags", (int)~LogTag.None);
        }

        public static LogLevel LogLevel
        {
            get => Logger<LogTag>.LogLevel;
            set
            {
                Logger<LogTag>.LogLevel = value;
                PlayerPrefs.SetInt("log_level", (int)value);
            }
        }

        public static LogTag SupportedTags
        {
            get => Logger<LogTag>.SupportedTags;
            set
            {
                Logger<LogTag>.SupportedTags = value;
                PlayerPrefs.SetInt("log_tags", (int)value);
            }
        }

        public static void Trace(LogTag tag, object message, string context)
            => Logger<LogTag>.Trace(tag, message, context);

        public static void Trace<TObject>(LogTag tag, object message, TObject context)
            where TObject : Object
            => Logger<LogTag>.Trace(tag, message, context);

        public static void Trace(LogTag tag, object message, object context = null)
            => Logger<LogTag>.Trace(tag, message, context);

        public static void Debug(LogTag tag, object message, string context)
            => Logger<LogTag>.Debug(tag, message, context);

        public static void Debug<TObject>(LogTag tag, object message, TObject context)
            where TObject : Object
            => Logger<LogTag>.Debug(tag, message, context);

        public static void Debug(LogTag tag, object message, object context = null)
            => Logger<LogTag>.Debug(tag, message, context);

        public static void Info(LogTag tag, object message, string context)
            => Logger<LogTag>.Info(tag, message, context);

        public static void Info<TObject>(LogTag tag, object message, TObject context)
            where TObject : Object
            => Logger<LogTag>.Info(tag, message, context);

        public static void Info(LogTag tag, object message, object context = null)
            => Logger<LogTag>.Info(tag, message, context);

        public static void Warn(LogTag tag, object message, string context)
            => Logger<LogTag>.Warn(tag, message, context);

        public static void Warn<TObject>(LogTag tag, object message, TObject context)
            where TObject : Object
            => Logger<LogTag>.Warn(tag, message, context);

        public static void Warn(LogTag tag, object message, object context = null)
            => Logger<LogTag>.Warn(tag, message, context);

        public static void Error(LogTag tag, object message, string context)
            => Logger<LogTag>.Error(tag, message, context);

        public static void Error<TObject>(LogTag tag, object message, TObject context)
            where TObject : Object
            => Logger<LogTag>.Error(tag, message, context);

        public static void Error(LogTag tag, object message, object context = null)
            => Logger<LogTag>.Error(tag, message, context);

        public static void Fatal(LogTag tag, object message, string context)
            => Logger<LogTag>.Fatal(tag, message, context);

        public static void Fatal<TObject>(LogTag tag, object message, TObject context)
            where TObject : Object
            => Logger<LogTag>.Fatal(tag, message, context);

        public static void Fatal(LogTag tag, object message, object context = null)
            => Logger<LogTag>.Fatal(tag, message, context);
    }
}