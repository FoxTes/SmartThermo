using System;

namespace SmartThermo.Services.DeviceConnector.Models
{
    public class LimitTriggerEventArgs : EventArgs
    {
        public int UpperValue { get; set; }
        public int LowerValue { get; set; }
    }
}