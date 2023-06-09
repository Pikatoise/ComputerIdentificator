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
    public partial class DbSettingsWindow : Window
    {
        public DbSettingsWindow()
        {
            InitializeComponent();

            ParseConn();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            App.ConnectionString = $"server={TBoxServer.Text};user={TBoxUser.Text};password={TBoxPassword.Text};database={TBoxDbName.Text};";

            Close();
        }

        void ParseConn()
        {
            string conn = App.ConnectionString;

            string[] connParams = conn.Split(new char[] { ';' });

            if (connParams.Length > 0)
            {
                TBoxServer.Text = connParams[0].Replace("server=",string.Empty);

                if (connParams.Length > 1)
                    TBoxUser.Text = connParams[1].Replace("user=", string.Empty);

                if (connParams.Length > 2)
                    TBoxPassword.Text = connParams[2].Replace("password=", string.Empty);

                if (connParams.Length > 3)
                    TBoxDbName.Text = connParams[3].Replace("database=", string.Empty);
            }
        }
    }
}
