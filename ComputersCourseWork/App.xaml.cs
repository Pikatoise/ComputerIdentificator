using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace ComputersCourseWork
{
    public partial class App : Application
    {
        public static string ConnectionString
        {
            get
            {
                string conn = "";

                if (!File.Exists("config.txt"))
                {
                    string defaultConn = "server=localhost;user=root;password=admin;database=computers;";

                    using (StreamWriter sw = new StreamWriter("config.txt", false))
                        sw.Write(defaultConn);

                    conn = defaultConn;
                }
                else
                {
                    using (StreamReader sr = new StreamReader("config.txt"))
                        conn = sr.ReadToEnd();
                }

                conn = conn.Trim();

                return conn;
            }

            set
            {
                using (StreamWriter sw = new StreamWriter("config.txt", false))
                    sw.Write(value);

                DbComputers = new ComputersContext(value);
            }
        }
        public static ComputersContext DbComputers;

        public App()
        {
            DbComputers = new ComputersContext(ConnectionString);
        }
    }
}
