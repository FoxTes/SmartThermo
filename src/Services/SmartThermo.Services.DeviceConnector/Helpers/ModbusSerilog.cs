using Microsoft.Extensions.Logging;
using NModbus;
using NModbus.Logging;

namespace SmartThermo.Services.DeviceConnector.Helpers
{
    public class ModbusSerilog : ModbusLogger, IModbusLogger
    {
        private readonly ILogger _logger;

        public ModbusSerilog(LoggingLevel minimumLoggingLevel, ILogger logger) : base(minimumLoggingLevel)
        {
            _logger = logger;
        }

        protected override void LogCore(LoggingLevel level, string message)
        {
            _logger.LogInformation(message + "\n");
        }
    }
}
