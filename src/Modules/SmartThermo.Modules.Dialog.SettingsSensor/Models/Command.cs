using SmartThermo.Modules.Dialog.SettingsSensor.Enums;
using System;
using System.Collections.Generic;

namespace SmartThermo.Modules.Dialog.SettingsSensor.Models
{
    public class Command
    {
        public string Name { get; set; }

        public TypeCommand TypeCommand { get; set; }

        public byte Address { get; set; }

        public Type TypeValue { get; set; }

        public List<byte> Bytes { get; set; }

        public double? Coefficient { get; set; } = 1d;
    }
}
