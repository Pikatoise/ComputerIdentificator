using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ComputersCourseWork.Windows
{
    public partial class InventoryNumWindow : Window
    {
        public string? InventoryNum = null;
     
        public InventoryNumWindow(string inventoryNum)
        {
            InitializeComponent();

            TBoxInventoryNum.Text = inventoryNum;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TBoxInventoryNum.Text.Length <= 25)
            {
                InventoryNum = TBoxInventoryNum.Text;

                Close();
            }
            else
            {
                MessageBox.Show("Максимальная длина номера - 25 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }
        }

        private void TBoxInventoryNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TBoxInventoryNum.Text.Length >= 25)
            {
                TBoxInventoryNum.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                TBoxInventoryNum.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }
}
