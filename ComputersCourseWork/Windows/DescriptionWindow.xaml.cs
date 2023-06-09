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
    public partial class DescriptionWindow : Window
    {
        public string? Description = null;

        public DescriptionWindow(string description)
        {
            InitializeComponent();

            TBoxDescription.Text = description;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TBoxDescription.Text.Length < 200)
            {
                Description = TBoxDescription.Text;

                Close();
            }
            else
            {
                MessageBox.Show("Максимальная длина описания - 200 символов!","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);

                return;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TBoxDescription.Text.Length >= 200)
            {
                TBoxDescription.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                TBoxDescription.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }
}
