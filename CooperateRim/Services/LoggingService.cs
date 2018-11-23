using HugsLib.Utils;

namespace CooperateRim.Services
{
    public class LoggingService
    {
        public ModLogger _logger;

        public LoggingService(ModLogger logger)
        {
            _logger = logger;
        }
        
        public void Message(string message)
        {
            if (_logger is ModLogger)
            {
                _logger.Message(message);
            }
        }

        public void Error(string error)
        {
            if (_logger is ModLogger)
            {
                _logger.Error(error);
            }
        }
    }
}
