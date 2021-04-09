using System.Windows.Media;

namespace SmartThermo.Modules.Dialog.SettingsSensor.Models
{
    public class MessageLogger
    {
        public string Message { get; }

        public Brush Color { get; }

        public MessageLogger(string message, Brush color)
        {
            Message = message;
            Color = color;
        }
    }
}
