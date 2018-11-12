using CooperateRim.Services;
using HugsLib.Utils;

namespace CooperateRim.Utilities
{
    public static class RimLog
    {
        public static ModLogger modLogger;
        public static LoggingService _logger;

        public static void Init(ModLogger logger)
        {
            modLogger = logger;
            _logger = new LoggingService(modLogger);
        }

        public static void Message(string message)
        {
            _logger.Message(message);
        }

        public static void Error(string error)
        {
            _logger.Error(error);
        }
    }
}
