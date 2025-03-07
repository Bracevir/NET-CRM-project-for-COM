using PrinterBaseProject.Models;
using System;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PrinterBaseProject
{
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        private SerialPort _serialPort;
        private DispatcherTimer _timer;
        private TemperatureManager _temperatureManager; // Новый экземпляр TemperatureManager
        private TemperatureViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            PopulateComPorts();
            InitializeSerialPort();
            AllocConsole();
            this.Loaded += MainWindow_Loaded;
            this.DataContext = new TemperatureViewModel();
            _temperatureManager = new TemperatureManager(); // Инициализация менеджера температур
            _viewModel = new TemperatureViewModel();
            DataContext = _viewModel; // Устанавливаем DataContext на ViewModel
            // Инициализация таймера для обновления реальных температур
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // Устанавливаем интервал в 1 секунду
            _timer.Tick += Timer_Tick; // Подписываемся на событие Tick
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Теперь можно безопасно обращаться к элементам управления
            RealExtruderTemperature.Text = _viewModel.RealExtruderTemperature; // Пример инициализации
            RealBedTemperature.Text = _viewModel.RealBedTemperature; // Пример инициализации
        }
        private async void Timer_Tick(object sender, EventArgs e)
        {
            await Task.Run(() => SendCommand("M105")); // Запрос текущих температур от принтера
        }

        private void PopulateComPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            ComPortComboBox.ItemsSource = ports;
            if (ports.Length > 0)
            {
                ComPortComboBox.SelectedIndex = 0; // Устанавливаем первый доступный порт по умолчанию
            }
        }

        private void InitializeSerialPort()
        {
            if (ComPortComboBox.SelectedItem != null)
            {
                _serialPort = new SerialPort((string)ComPortComboBox.SelectedItem, 250000, Parity.None, 8, StopBits.One);
                try
                {
                    _serialPort.Open();
                    //Console.WriteLine(_serialPort.ReadLine());
                    var readThread = new System.Threading.Thread(ReadResponse);
                    readThread.IsBackground = true;
                    readThread.Start();

                    _timer.Start();
                    // Выполнение сброса принтера после открытия порта
                    ResetPrinter();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения: {ex.Message}");
                }
            }
        }

        private void RefreshComPorts_Click(object sender, RoutedEventArgs e)
        {
            PopulateComPorts();
        }

        private void ComPortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _timer.Stop(); // Останавливаем таймер при закрытии порта
            }
            InitializeSerialPort();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("M25"); // Команда для паузы
        }

        private void SendGCodeButton_Click(object sender, RoutedEventArgs e)
        {
            string gcodeFilePath = @"path_to_your_gcode_file.gcode"; // Укажите путь к файлу
            if (System.IO.File.Exists(gcodeFilePath))
            {
                string[] gcodeLines = System.IO.File.ReadAllLines(gcodeFilePath);
                foreach (var line in gcodeLines)
                {
                    SendCommand(line);
                    System.Threading.Thread.Sleep(100); // Задержка между командами
                }
            }
            else
            {
                MessageBox.Show("Файл GCODE не найден.");
            }
        }

        private void MoveXPlus_Click(object sender, RoutedEventArgs e) => SendCommand("G0 X10");
        private void MoveXMinus_Click(object sender, RoutedEventArgs e) => SendCommand("G0 X-10");
        private void MoveYPlus_Click(object sender, RoutedEventArgs e) => SendCommand("G0 Y10");
        private void MoveYMinus_Click(object sender, RoutedEventArgs e) => SendCommand("G0 Y-10");
        private void MoveZPlus_Click(object sender, RoutedEventArgs e) => SendCommand("G0 Z10");
        private void MoveZMinus_Click(object sender, RoutedEventArgs e) => SendCommand("G0 Z-10");

        private void HomeXYZButton_Click(object sender, RoutedEventArgs e)
        {
            SendCommand("G28"); // Команда для парковки X Y Z
        }

        private async void SetExtruderTemperature_Click(object sender, RoutedEventArgs e)
        {
            int temperature = (int)ExtruderTemperatureSlider.Value;
            _temperatureManager.SetTemperature(0, temperature);
            await Task.Run(() => SendCommand($"M104 S{temperature}")); // Отправляем команду асинхронно
        }

        private async void SetBedTemperature_Click(object sender, RoutedEventArgs e)
        {
            int temperature = (int)BedTemperatureSlider.Value;
            _temperatureManager.SetTemperature(-1, temperature);
            await Task.Run(() => SendCommand($"M140 S{temperature}")); // Отправляем команду асинхронно
        }

        private void ExtruderTemperatureSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Проверка на null перед обновлением текста
            if (ExtruderTemperatureValue != null)
            {
                ExtruderTemperatureValue.Text = ((int)e.NewValue).ToString();
            }
        }

        private void BedTemperatureSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Проверка на null перед обновлением текста
            if (BedTemperatureValue != null)
            {
                BedTemperatureValue.Text = ((int)e.NewValue).ToString();
            }
        }

        private async void SendCommand(string command)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    await Task.Run(() => _serialPort.WriteLine(command));
                    Console.WriteLine($"Команда отправлена: {command}"); // Для отладки
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при отправке команды: {ex.Message}");
                    MessageBox.Show($"Ошибка при отправке команды: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("COM-порт не открыт.");
            }
        }

        private async void ReadResponse()
        {
            Console.WriteLine("Запуск потока чтения...");
            while (_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    string response = await Task.Run(() => _serialPort.ReadLine());
                    Console.WriteLine($"Ответ от принтера: {response}"); // Выводим полный ответ

                    // Проверяем на пустую строку
                    if (string.IsNullOrWhiteSpace(response))
                    {
                        Console.WriteLine("Получен пустой ответ от принтера.");
                        continue; // Пропускаем итерацию
                    }

                    // Удаляем префикс "ok" и лишние пробелы
                    if (response.StartsWith("ok"))
                    {
                        try
                        {
                            response = response.Substring(3).Trim(); // Удаляем "ok" и пробелы
                        }
                        catch (Exception ex)
                        { 
                            Console.WriteLine(ex.Message + "Строка слишком короткая для удаления ok");
                        }
                        
                    }
                    else
                    {
                        
                    }

                    // Разделяем строку на части по пробелам
                    var parts = response.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    float extruderTemperature = 0;
                    float bedTemperature = 0;
                    bool extruderFound = false;
                    bool bedFound = false;

                    foreach (var part in parts)
                    {
                        if (part.StartsWith("T:"))
                        {
                            // Извлекаем температуру экструдерa
                            if (part.Length > 2) // Проверяем длину перед использованием Substring
                            {
                                string ExtrParse = part.Substring(2).Trim(); // Убираем префикс T:
                                //Console.WriteLine($"Температура экструдерa (до парсинга): '{ExtrParse}'");

                                // Используем InvariantCulture для парсинга
                                if (float.TryParse(ExtrParse, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out extruderTemperature))
                                {
                                    extruderFound = true;
                                    Console.WriteLine($"Температура экструдерa успешно распарсена: {extruderTemperature}");
                                }
                                else
                                {
                                    Console.WriteLine($"Не удалось распарсить температуру экструдерa: '{ExtrParse}'");
                                }
                            }
                        }
                        else if (part.StartsWith("B:"))
                        {
                            // Извлекаем температуру стола
                            if (part.Length > 2) // Проверяем длину перед использованием Substring
                            {
                                string BedParse = part.Substring(2).Trim(); // Убираем префикс B:
                                //Console.WriteLine($"Температура стола (до парсинга): '{BedParse}'");

                                // Используем InvariantCulture для парсинга
                                if (float.TryParse(BedParse, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out bedTemperature))
                                {
                                    bedFound = true;
                                    Console.WriteLine($"Температура стола успешно распарсена: {bedTemperature}");
                                }
                                else
                                {
                                    Console.WriteLine($"Не удалось распарсить температуру стола: '{BedParse}'");
                                }
                            }
                        }
                    }

                    if (extruderFound && bedFound)
                    {
                        // Обновляем свойства ViewModel через Dispatcher
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _viewModel.RealExtruderTemperature = extruderTemperature + " °C";
                            _viewModel.RealBedTemperature = bedTemperature + " °C";
                            Console.WriteLine($"Реальная температура экструдерa: {_viewModel.RealExtruderTemperature}");
                            Console.WriteLine($"Реальная температура стола: {_viewModel.RealBedTemperature}");
                        }));
                    }
                    else
                    {
                        Console.WriteLine("Не удалось найти температуры экструдерa или стола.");
                        Console.WriteLine($"Части ответа: {string.Join(", ", parts)}"); // Выводим части для отладки
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при чтении ответа: {ex.Message}");
                    MessageBox.Show($"Ошибка при чтении ответа: {ex.Message}");
                    break; // Выход из цикла в случае ошибки
                }
            }
            Console.WriteLine("Поток чтения завершен."); // Для отладки
        }
        private void ResetPrinter()
        {
            try
            {
                SendCommand("M999"); // Пример команды для сброса принтера
                Console.WriteLine("Команда сброса отправлена."); // Для отладки
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке команды сброса: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _timer.Stop();
            }
            base.OnClosed(e);
        }
    }
}