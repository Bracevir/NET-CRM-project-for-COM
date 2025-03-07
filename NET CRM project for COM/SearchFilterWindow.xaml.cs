using System.Windows;

namespace CRMproject
{
    public partial class SearchFilterWindow : Window
    {
        public string FileNameFilter { get; private set; }
        public string MaterialFilter { get; private set; }
        public string EquipmentFilter { get; private set; }
        public string CostFilter { get; private set; }

        public SearchFilterWindow()
        {
            InitializeComponent();
        }

        private void ApplyFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем значения из текстовых полей
            FileNameFilter = FileNameTextBox.Text;
            MaterialFilter = MaterialTextBox.Text;
            EquipmentFilter = EquipmentTextBox.Text;
            CostFilter = CostTextBox.Text;

            this.DialogResult = true; // Устанавливаем результат диалога
            this.Close(); // Закрываем окно после применения фильтров
        }
    }
}
