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
    public partial class PcNameWindow : Window
    {
        public string name = null;
        public PcNameWindow(string name)
        {
            InitializeComponent();
            TBoxPcName.Text = name;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string newName = TBoxPcName.Text;

            if (!string.IsNullOrWhiteSpace(newName) && newName.Length > 3 && newName.Length < 20)
            {
                if (MessageBox.Show("Вы уверены, что хотите переименовать компьютер?\nПотребуется перезагрузка", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    name = TBoxPcName.Text;
                    Close();
                }
            }
            else
                MessageBox.Show("Имя не подходит!");

        }
    }
}
