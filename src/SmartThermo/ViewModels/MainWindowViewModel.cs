using System;
using Prism.Mvvm;
using SmartThermo.Enums;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading;
using Prism.Commands;
using Prism.Events;
using SmartThermo.Service;
using SmartThermo.Models;

namespace SmartThermo.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Field

        private readonly ISerialPortService _serialPortService;

        private bool _statusConnect;

        private ObservableCollection<string> _ports = new ObservableCollection<string>();
        private string _portSelected;
        private string _loggerText;

        private BaudRate _baudRateSelected;
        private Parity _paritySelected;
        private DataBits _dataBitsSelected;
        private StopBits _stopBitsSelected;

        #endregion

        #region Property

        public bool StatusConnect
        {
            get => _statusConnect;
            set => SetProperty(ref _statusConnect, value);
        }

        public ObservableCollection<string> Ports
        {
            get => _ports;
            set => SetProperty(ref _ports, value);
        }

        public string PortSelected
        {
            get => _portSelected;
            set => SetProperty(ref _portSelected, value);
        }

        public string LoggerText
        {
            get => _loggerText;
            set => SetProperty(ref _loggerText, value);
        }

        public BaudRate BaudRateSelected
        {
            get => _baudRateSelected;
            set => SetProperty(ref _baudRateSelected, value);
        }

        public Parity ParitySelected
        {
            get => _paritySelected;
            set => SetProperty(ref _paritySelected, value);
        }

        public DataBits DataBitsSelected
        {
            get => _dataBitsSelected;
            set => SetProperty(ref _dataBitsSelected, value);
        }

        public StopBits StopBitsSelected
        {
            get => _stopBitsSelected;
            set => SetProperty(ref _stopBitsSelected, value);
        }

        public DelegateCommand GetSerialPortCommand { get; }

        public DelegateCommand ChangedStatusConnectCommand { get; }

        public DelegateCommand WriteDataCommand { get; }

        public DelegateCommand ReadDataCommand { get; }

        #endregion

        public MainWindowViewModel(ISerialPortService serialPortService)
        {
            _serialPortService = serialPortService;
            _serialPortService.StatusConnectChanged += StatusConnectChanged;

            Ports.AddRange(_serialPortService.GetPortNames());

            PortSelected = Ports[0];
            BaudRateSelected = BaudRate.S9600;
            ParitySelected = Parity.None;
            DataBitsSelected = DataBits.B8;
            StopBitsSelected = StopBits.One;

            GetSerialPortCommand = new DelegateCommand(GetSerialPortSumbit);
            ChangedStatusConnectCommand = new DelegateCommand(ChangeStatusConnectSumbit);
            WriteDataCommand = new DelegateCommand(WriteDataSumbit);
            ReadDataCommand = new DelegateCommand(ReadDataSumbit);
        }

        private void WriteDataSumbit()
        {
            try
            {
                _serialPortService.WriteData(1);
            }
            catch (Exception ex)
            {
                LoggerText += ex.Message + "\n";
            }
        }

        private  void ReadDataSumbit()
        {
            try
            {
                var test = _serialPortService.ReadData(1);

                LoggerText += test[0].ToString();
            }
            catch (Exception ex)
            {
                LoggerText += ex.Message + "\n";
            }
        }

        #region Method

        private void StatusConnectChanged(object sender, StatusConnect e)
        {
            if (e == Enums.StatusConnect.Open)
            {
                StatusConnect = true;
            }
            else
            {
                StatusConnect = false;
            }
        }

        private void GetSerialPortSumbit()
        {
            Ports.Clear();
            Ports.AddRange(_serialPortService.GetPortNames());

            PortSelected = Ports[0];
        }

        private void ChangeStatusConnectSumbit()
        {
            if (_statusConnect)
            {
                try
                {
                    _serialPortService.Close();

                    LoggerText += $"Порт {_serialPortService.PortName} успешно закрыт." + "\n";

                }
                catch (Exception ex)
                {
                    LoggerText += $"Ошибка при закрытии {_portSelected} порта." + "\n";
                    LoggerText += ex.Message + "\n";
                }
            }
            else
            {
                try
                {
                    _serialPortService.Open(new SerialPortParam()
                    {
                        Name = PortSelected,
                        BaudRate = BaudRateSelected,
                        DataBits = DataBitsSelected,
                        Parity = ParitySelected,
                        StopBits = StopBitsSelected
                    });
                    LoggerText += $"Порт {_serialPortService.PortName} успешно открыт." + "\n";
                }
                catch (Exception ex)
                {
                    LoggerText += $"Ошибка открытия порта {_serialPortService.PortName}." + "\n";
                    LoggerText += ex.Message + "\n";
                }
            }
        }


        #endregion
    }
}
