using System.ComponentModel;

namespace PrinterBaseProject.Models // Убедитесь, что это правильное пространство имен
{
    public class TemperatureViewModel : INotifyPropertyChanged
    {
        private string _realExtruderTemperature;
        private string _realBedTemperature;

        public string RealExtruderTemperature
        {
            get => _realExtruderTemperature;
            set
            {
                if (_realExtruderTemperature != value)
                {
                    _realExtruderTemperature = value;
                    OnPropertyChanged(nameof(RealExtruderTemperature));
                }
            }
        }

        public string RealBedTemperature
        {
            get => _realBedTemperature;
            set
            {
                if (_realBedTemperature != value)
                {
                    _realBedTemperature = value;
                    OnPropertyChanged(nameof(RealBedTemperature));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}