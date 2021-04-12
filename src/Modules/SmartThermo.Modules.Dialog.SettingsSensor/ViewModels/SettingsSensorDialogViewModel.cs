using Prism.Commands;
using SmartThermo.Core.Enums;
using SmartThermo.Core.Mvvm;
using SmartThermo.Modules.Dialog.SettingsSensor.Enums;
using SmartThermo.Modules.Dialog.SettingsSensor.Models;
using SmartThermo.Modules.Dialog.SettingsSensor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SmartThermo.Modules.Dialog.SettingsSensor.ViewModels
{
    public class SettingsSensorDialogViewModel : DialogViewModelBase
    {
        #region Field

        private readonly SensorTuner _sensorTuner;
        private CancellationTokenSource _cancellationTokenSource;

        private ObservableCollection<MessageLogger> _logger = new ObservableCollection<MessageLogger>();
        private string _portNameSelected;
        private List<string> _portName;
        private List<Command> _command;

        private BaudRate _baudRateSelected;
        private StopBits _stopBitsSelected;
        private Command _commandSelected;
        private string _inputValue = "0";
        private double _maxSetValue;
        private Visibility _showWakeUpIndicator = Visibility.Hidden;
        private bool _isEnable = true;

        #endregion

        #region Property

        public ObservableCollection<MessageLogger> Logger
        {
            get => _logger;
            set => SetProperty(ref _logger, value);
        }

        public string PortNameSelected
        {
            get => _portNameSelected;
            set => SetProperty(ref _portNameSelected, value);
        }

        public List<string> PortName
        {
            get => _portName;
            set => SetProperty(ref _portName, value);
        }

        public List<Command> Command
        {
            get => _command;
            set => SetProperty(ref _command, value);
        }

        public Command CommandSelected
        {
            get => _commandSelected;
            set
            {
                SetProperty(ref _commandSelected, value);
                ChangedMaxValue(value);
            }
        }

        public BaudRate BaudRateSelected
        {
            get => _baudRateSelected;
            set => SetProperty(ref _baudRateSelected, value);
        }

        public StopBits StopBitsSelected
        {
            get => _stopBitsSelected;
            set => SetProperty(ref _stopBitsSelected, value);
        }

        public string InputValue
        {
            get => _inputValue;
            set => SetProperty(ref _inputValue, value);
        }

        public double MaxSetValue
        {
            get => _maxSetValue;
            set => SetProperty(ref _maxSetValue, value);
        }

        public Visibility ShowWakeUpIndicator
        {
            get => _showWakeUpIndicator;
            set => SetProperty(ref _showWakeUpIndicator, value);
        }

        private bool IsEnable
        {
            get => _isEnable;
            set => SetProperty(ref _isEnable, value);
        }

        public DelegateCommand ExecuteCommand { get; }

        public DelegateCommand WakeUpCommand { get; }

        public DelegateCommand CancelWakeUpCommand { get; }

        #endregion

        #region Constructor

        public SettingsSensorDialogViewModel()
        {
            _sensorTuner = new SensorTuner();

            UploadingDataSources();
            SetDefaultSettings();

            ExecuteCommand = new DelegateCommand(ExecuteSumbitAsync).ObservesCanExecute(() => IsEnable);
            WakeUpCommand = new DelegateCommand(WakeUpSumbitAsync).ObservesCanExecute(() => IsEnable);
            CancelWakeUpCommand = new DelegateCommand(CancelWakeUpSumbit);

            LoggerWrite($"Старт - {DateTime.Now.ToLongTimeString()}");
        }
        #endregion

        #region Method

        private async void ExecuteSumbitAsync()
        {
            IsEnable = false;

            try
            {
                if (CommandSelected.TypeValue == typeof(byte))
                    CommandSelected.Bytes = new List<byte> { byte.Parse(InputValue) };
                else if (CommandSelected.TypeValue == typeof(ushort))
                    CommandSelected.Bytes = BitConverter.GetBytes(ushort.Parse(InputValue)).ToList();

                LoggerWrite("Попытка выполнения команды.");
                _sensorTuner.SettingPortSensor = new SettingPortSensor
                {
                    NamePort = PortNameSelected,
                    BaudRate = (int)BaudRateSelected,
                    StopBits = StopBitsSelected
                };
                var result = await _sensorTuner.ExecuteCommand(CommandSelected);

                if (string.IsNullOrEmpty(result))
                    LoggerWrite("Команда выполнена успешно!", StatusLogging.Success);
                else
                {
                    LoggerWrite($"Считанное значение - {result}.", StatusLogging.Success);
                    InputValue = result;
                }
            }
            catch (UnauthorizedAccessException)
            {
                LoggerWrite($"Не удалось открыть {PortNameSelected} порт.", StatusLogging.Failure);
            }
            catch (Exception ex)
            {
                LoggerWrite(ex.Message, StatusLogging.Failure);
            }
            finally
            {
                _sensorTuner.ClearUpPort();
            }

            IsEnable = true;
        }

        private async void WakeUpSumbitAsync()
        {
            IsEnable = false;
            ShowWakeUpIndicator = Visibility.Visible;

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                LoggerWrite("Попытка пробуждения датчика.");
                _sensorTuner.SettingPortSensor = new SettingPortSensor
                {
                    NamePort = PortNameSelected,
                    BaudRate = (int)BaudRateSelected,
                    StopBits = StopBitsSelected
                };
                await _sensorTuner.WakeUpCommand(_cancellationTokenSource.Token);
                LoggerWrite("Пробуждение датчика выполнено успешно.", StatusLogging.Success);
            }
            catch (UnauthorizedAccessException)
            {
                LoggerWrite($"Не удалось открыть {PortNameSelected} порт.", StatusLogging.Failure);
            }
            catch (TaskCanceledException)
            {
                LoggerWrite("Попытка пробуждения была отменена.", StatusLogging.Failure);
            }
            catch (Exception ex)
            {
                LoggerWrite(ex.Message, StatusLogging.Failure);
            }
            finally
            {
                _sensorTuner.ClearUpPort();
            }

            IsEnable = true;
            ShowWakeUpIndicator = Visibility.Hidden;
        }

        private void CancelWakeUpSumbit()
        {
            _cancellationTokenSource?.Cancel();
        }

        private void UploadingDataSources()
        {
            PortName = new List<string>(SerialPort.GetPortNames());

            Command = new List<Command>
            {
                new Command
                {
                    Name = "Чтение серийного номера",
                    TypeCommand = TypeCommand.Read,
                    Address = 0x01,
                    TypeValue = typeof(ushort)
                },
                new Command
                {
                    Name = "Запись серийного номера",
                    TypeCommand = TypeCommand.Write,
                    Address = 0x11,
                    TypeValue = typeof(ushort)
                },
                new Command
                {
                    Name = "Чтение температуры",
                    TypeCommand = TypeCommand.Read,
                    Address = 0x02,
                    TypeValue = typeof(byte)
                },
                new Command
                {
                    Name = "Чтение таблиц RT/R25",
                    TypeCommand = TypeCommand.Read,
                    Address = 0x03,
                    TypeValue = typeof(ushort),
                    Coefficient = 0.001d
                },
                new Command
                {
                    Name = "Запись таблиц RT/R25",
                    TypeCommand = TypeCommand.Write,
                    Address = 0x13,
                    TypeValue = typeof(ushort),
                    Coefficient = 0.001d
                },
                new Command
                {
                    Name = "Чтение порога напряжения",
                    TypeCommand = TypeCommand.Read,
                    Address = 0x04,
                    TypeValue = typeof(byte),
                    Coefficient = 0.01d
                },
                new Command
                {
                    Name = "Запись порога напряжения",
                    TypeCommand = TypeCommand.Write,
                    Address = 0x14,
                    TypeValue = typeof(byte),
                    Coefficient = 0.01d
                },
                new Command
                {
                    Name = "Чтение номера канала",
                    TypeCommand = TypeCommand.Read,
                    Address = 0x05,
                    TypeValue = typeof(byte)
                },
                new Command
                {
                    Name = "Запись номера канала",
                    TypeCommand = TypeCommand.Write,
                    Address = 0x15,
                    TypeValue = typeof(byte)
                },
                new Command
                {
                    Name = "Чтение параметра R25",
                    TypeCommand = TypeCommand.Read,
                    Address = 0x06,
                    TypeValue = typeof(ushort)
                },
                new Command
                {
                    Name = "Запись параметра R25",
                    TypeCommand = TypeCommand.Write,
                    Address = 0x16,
                    TypeValue = typeof(ushort)
                }
            };
        }

        private void SetDefaultSettings()
        {
            PortNameSelected = PortName[0];
            BaudRateSelected = BaudRate.S2400;
            StopBitsSelected = StopBits.One;

            CommandSelected = Command[0];
        }

        private void LoggerWrite(string message, StatusLogging statusLogging = StatusLogging.Info)
        {
            var brush = statusLogging switch
            {
                StatusLogging.Info => Brushes.White,
                StatusLogging.Success => Brushes.Green,
                StatusLogging.Failure => Brushes.Red,
                _ => throw new ArgumentOutOfRangeException(nameof(statusLogging), statusLogging, null),
            };
            Logger.Add(new MessageLogger(message, brush));
        }

        private void ChangedMaxValue(Command value)
        {
            if (value.TypeValue == typeof(byte))
                MaxSetValue = byte.MaxValue;
            else if (value.TypeValue == typeof(ushort))
                MaxSetValue = ushort.MaxValue;
            else
                MaxSetValue = 0;
        }

        #endregion
    }
}
