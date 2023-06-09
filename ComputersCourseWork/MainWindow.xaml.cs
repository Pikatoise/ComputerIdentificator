using ComputersCourseWork.Windows;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static ComputersCourseWork.Network;

namespace ComputersCourseWork
{
    public partial class MainWindow : Window
    {
        Computer? currentComputer = null;
        List<Computerspec> computerspecs = new List<Computerspec>();
        List<Computerspec> computernetwork = new List<Computerspec>();
        bool isProxyEnabled = Network.GetProxyStatus();
        bool isNeedEnableDHCP = false;

        public MainWindow()
        {
            InitializeComponent();

            InitComputer();

            if (CheckConnection())
                InitDbInfo();
        }

        private void ButtonRefresData_Click(object sender, RoutedEventArgs e)
        {
            if (CheckConnection())
            {
                if (currentComputer == null)
                {
                    if (App.DbComputers.Computers.Where(c => c.Name.Equals(TBlockPcName.Text)).Any())
                    {
                        MessageBox.Show("Компьютер с таким именем уже зарегистрирован!\nИзмените имя пк");
                    }
                    else
                    {
                        string deviceType = (PCSpecifications.PcType)PCSpecifications.GetPcType() == PCSpecifications.PcType.Desktop ? "ПК" : "Ноутбук";

                        currentComputer = new Computer()
                        {
                            Name = TBlockPcName.Text,
                            Description = TBlockPcDescription.Text,
                            Devicetype = App.DbComputers.Devicetypes.Where(d => d.Name.Equals(deviceType)).FirstOrDefault().Id,
                            Windowskey = TBlockWindowsKey.Text,
                            LastUpdate = DateOnly.FromDateTime(DateTime.Now),
                            inventoryNum = TBlockPcInventoryNum.Text
                        };

                        App.DbComputers.Computers.Add(currentComputer);

                        App.DbComputers.SaveChanges();

                        TBlockLastUpdate.Text = currentComputer.LastUpdate.ToString();

                        currentComputer = App.DbComputers.Computers.Where(c => c.Name.Equals(currentComputer.Name)).FirstOrDefault();

                        App.DbComputers.Computerspecs.AsNoTracking();

                        foreach (Computerspec cs in computerspecs)
                        {
                            cs.Computer = currentComputer.Id;
                            App.DbComputers.Computerspecs.Add(cs);
                            App.DbComputers.SaveChanges();
                        }

                        foreach (Computerspec cs in computernetwork)
                        {
                            cs.Computer = currentComputer.Id;
                            App.DbComputers.Computerspecs.Add(cs);
                            App.DbComputers.SaveChanges();
                        }

                        MessageBox.Show("Компьютер сохранен в БД!","Успешно",MessageBoxButton.OK);
                    }
                }
                else
                {
                    string deviceType = (PCSpecifications.PcType)PCSpecifications.GetPcType() == PCSpecifications.PcType.Desktop ? "ПК" : "Ноутбук";

                    currentComputer.Name = TBlockPcName.Text;
                    currentComputer.Description = TBlockPcDescription.Text;
                    currentComputer.Devicetype = App.DbComputers.Devicetypes.Where(d => d.Name.Equals(deviceType)).FirstOrDefault().Id;
                    currentComputer.LastUpdate = DateOnly.FromDateTime(DateTime.Now);
                    currentComputer.Windowskey = TBlockWindowsKey.Text;

                    TBlockLastUpdate.Text = currentComputer.LastUpdate.ToString();

                    foreach (Computerspec cs in App.DbComputers.Computerspecs.Where(c => c.Computer == currentComputer.Id))
                        App.DbComputers.Computerspecs.Remove(cs);

                    App.DbComputers.SaveChanges();

                    InitComputer();

                    App.DbComputers.Computerspecs.AsNoTracking();

                    foreach (Computerspec cs in computerspecs)
                    {
                        cs.Computer = currentComputer.Id;
                        App.DbComputers.Computerspecs.Add(cs);
                        App.DbComputers.SaveChanges();
                    }

                    foreach (Computerspec cs in computernetwork)
                    {
                        cs.Computer = currentComputer.Id;
                        App.DbComputers.Computerspecs.Add(cs);
                        App.DbComputers.SaveChanges();
                    }

                    MessageBox.Show("Данные компьютера обновлены!", "Успешно", MessageBoxButton.OK);
                }
            }
            else
                MessageBox.Show("Отсутствует подключение к БД!");
        }

        private void PcDescription_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CheckConnection())
            {
                if (currentComputer != null)
                {
                    DescriptionWindow dw = new DescriptionWindow(TBlockPcDescription.Text);

                    dw.ShowDialog();

                    if (dw.Description != null)
                    {
                        TBlockPcDescription.Text = dw.Description;
                        currentComputer.Description = dw.Description;

                        App.DbComputers.SaveChanges();
                    }

                }
                else
                    MessageBox.Show("Компьютер отсутствует в БД!\nОбновите перед изменением!","Предупреждение",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
            else
                MessageBox.Show("Отсутствует соединение с БД!","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
        }

        private void TBlockPcName_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PcNameWindow pnw = new PcNameWindow(TBlockPcName.Text);

            pnw.ShowDialog();

            if (!string.IsNullOrWhiteSpace(pnw.name))
                PCSpecifications.ChangePcName(pnw.name);
        }

        private void ButtonEditDb_Click(object sender, RoutedEventArgs e)
        {
            DbSettingsWindow dsw = new DbSettingsWindow();

            dsw.ShowDialog();

            CheckConnection();
        }

        private void ButtonSaveProxySettings_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBoxProxyAdress.Text))
            {
                MessageBox.Show("Некорректный адрес!");
                return;
            }

            if (string.IsNullOrWhiteSpace(TBoxProxyPort.Text))
            {
                MessageBox.Show("Некорректный порт!");
                return;
            }

            Network.SetProxy(TBoxProxyAdress.Text,TBoxProxyPort.Text);

            MessageBox.Show("Новый прокси установлен!");
        }

        private void CBoxEnableProxy_Click(object sender, RoutedEventArgs e)
        {
            isProxyEnabled = !isProxyEnabled;
            SPanelProxySettings.IsEnabled = isProxyEnabled;

            Network.ChangeProxyStatus(isProxyEnabled);
        }

        private void RButtonDHCPEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (isNeedEnableDHCP)
            {
                Network.EnableDHCP();

                CardNetworkSettings.IsEnabled = false;
                ButtonSaveNetworkSettings.IsEnabled = false;
            }
        }

        private void RButtonDHCPDisabled_Checked(object sender, RoutedEventArgs e)
        {
            isNeedEnableDHCP = true;

            CardNetworkSettings.IsEnabled = true;
            ButtonSaveNetworkSettings.IsEnabled = true;
        }

        private void ButtonSaveNetworkSettings_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBoxIPV4.Text))
            {
                MessageBox.Show("Некорректный ip!");
                return;
            }

            if (string.IsNullOrWhiteSpace(TBoxSubnetMask.Text))
            {
                MessageBox.Show("Некорректая маска!");
                return;
            }

            Network.SetStaticNetwork(new StaticNetworkParams()
            {
                IP = TBoxIPV4.Text,
                DNS = TBoxDNS.Text,
                Gateway = TBoxGateway.Text,
                SubnetMask = TBoxSubnetMask.Text
            });

            MessageBox.Show("Статические настройки сети установлены!");
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as TabControl).SelectedIndex == 0)
                BorderDbData.Visibility = Visibility.Hidden;

            if (e.Source is TabControl)
            {
                if (!(BorderDbData.Visibility == Visibility.Visible))
                {
                    if ((sender as TabControl).SelectedItem != null)
                    {
                        if ((sender as TabControl).SelectedIndex == 1)
                        {
                            LBoxDevices.Items.Clear();

                            List<Computer> computers = App.DbComputers.Computers
                                .Include(c => c.DevicetypeNavigation)
                                .ToList();

                            foreach (Computer computer in computers)
                            {
                                ListBoxItem lbitem = new ListBoxItem() { Tag = computer };

                                Canvas canvas = new Canvas() { Height = 50 };

                                PackIcon icon = new PackIcon()
                                {
                                    Kind = computer.DevicetypeNavigation.Name.Equals("Ноутбук") ? PackIconKind.Laptop : PackIconKind.DesktopTowerMonitor,
                                    Width = 40,
                                    Height = 40
                                };

                                TextBlock tbName = new TextBlock()
                                {
                                    Text = computer.Name,
                                    TextWrapping = TextWrapping.Wrap,
                                    Width = 120
                                };

                                TextBlock tbDate = new TextBlock()
                                {
                                    Text = computer.LastUpdate.ToString(),
                                    FontSize = 10
                                };

                                Canvas.SetLeft(tbName, 45);
                                Canvas.SetTop(tbName, 12);
                                Canvas.SetTop(tbDate, 40);


                                canvas.Children.Add(icon);
                                canvas.Children.Add(tbName);
                                canvas.Children.Add(tbDate);

                                lbitem.Content = canvas;

                                LBoxDevices.Items.Add(lbitem);
                            }
                        }
                    }
                }
            }
        }

        private void LBoxDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LBoxDevices.SelectedItem != null)
            {
                BorderDbData.Visibility = Visibility.Visible;

                Computer selected = ((LBoxDevices.SelectedItem as ListBoxItem).Tag as Computer);
                List<Computerspec> selectedSpecs = App.DbComputers.Computerspecs.Where(c => c.Computer == selected.Id).ToList();

                IconDeviceTypeDB.Kind = selected.DevicetypeNavigation.Name.Equals("Ноутбук") ? PackIconKind.Laptop : PackIconKind.DesktopTowerMonitor;
                TBlockPcNameDB.Text = selected.Name;
                TBlockPcDescriptionDB.Text = selected.Description;
                TBlockWindowsVersionDB.Text = selectedSpecs.Find(s => s.Title.Equals("Система")).Value;
                TBlockWindowsKeyDB.Text = selected.Windowskey;
                TBlockInventoryNumDB.Text = selected.inventoryNum;

                DGridHardwareDB.ItemsSource = selectedSpecs.Where(s => !s.IsNetwork);

                TBoxIPV4DB.Text = selectedSpecs.Find(s => s.Title.Equals("IP")).Value;
                TBoxGatewayDB.Text = selectedSpecs.Find(s => s.Title.Equals("Gateway")).Value;
                TBoxSubnetMaskDB.Text = selectedSpecs.Find(s => s.Title.Equals("SubnetMask")).Value;
                TBoxDNSDB.Text = selectedSpecs.Find(s => s.Title.Equals("DNS")).Value;
                TBoxProxyAdressDB.Text = selectedSpecs.Find(s => s.Title.Equals("proxyAdress")).Value;
                TBoxProxyPortDB.Text = selectedSpecs.Find(s => s.Title.Equals("proxyPort")).Value;
            }
            else
                BorderDbData.Visibility = Visibility.Hidden;
        }
        
        private void ButtonDeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            if (CheckConnection())
            {
                if (LBoxDevices.SelectedItem != null)
                {
                    App.DbComputers.Computers.Remove((LBoxDevices.SelectedItem as ListBoxItem).Tag as Computer);

                    App.DbComputers.SaveChanges();

                    LBoxDevices.Items.Remove(LBoxDevices.SelectedItem);

                    CheckConnection();

                    InitDbInfo();
                }
            }
            else
                MessageBox.Show("Отсутствует подключение к БД!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ButtonEditInventoryNum_Click(object sender, RoutedEventArgs e)
        {
            if (CheckConnection())
            {
                if (currentComputer != null)
                {
                    InventoryNumWindow inw = new InventoryNumWindow(TBlockPcInventoryNum.Text);

                    inw.ShowDialog();

                    if (inw.InventoryNum != null)
                    {
                        TBlockPcInventoryNum.Text = inw.InventoryNum;
                        currentComputer.inventoryNum = inw.InventoryNum;

                        App.DbComputers.SaveChanges();
                    }
                }
                else
                    MessageBox.Show("Компьютер отсутствует в БД!\nОбновите перед изменением!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                MessageBox.Show("Отсутствует соединение с БД!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        bool CheckConnection()
        {
            if (App.DbComputers.Database.CanConnect())
            {
                currentComputer = App.DbComputers.Computers.Where(c => c.Name.Equals(TBlockPcName.Text)).FirstOrDefault();

                if (currentComputer != null)
                {
                    TBlockLastUpdate.Text = currentComputer.LastUpdate.ToString();
                    TBlockPcDescription.Text = currentComputer.Description;
                    TBlockPcInventoryNum.Text = currentComputer.inventoryNum;
                }
                else
                {
                    TBlockLastUpdate.Text = "--/--/----";
                    TBlockPcDescription.Text = "Отсутствует...";
                    TBlockPcInventoryNum.Text = "-";
                }

                IconDbStatus.Kind = MaterialDesignThemes.Wpf.PackIconKind.Check;
                IconDbStatus.Foreground = new SolidColorBrush(Colors.ForestGreen);

                TItemComputerDB.IsEnabled = true;

                ButtonRefreshData.IsEnabled = true;

                return true;
            }
            else
            {
                TBlockPcInventoryNum.Text = "-";
                TBlockLastUpdate.Text = "--/--/----";

                IconDbStatus.Kind = MaterialDesignThemes.Wpf.PackIconKind.Error;
                IconDbStatus.Foreground = new SolidColorBrush(Colors.Red);

                TItemComputerDB.IsEnabled = false;
             
                ButtonRefreshData.IsEnabled = false;

                return false;
            }
        }

        void InitComputer()
        {
            #region Характеристики
            computerspecs.Clear();
            computernetwork.Clear();
            
            TBlockWindowsKey.Text = PCSpecifications.GetWindowsKey();
            TBlockWindowsVersion.Text = PCSpecifications.GetOperatingSystem();
            TBlockPcName.Text = PCSpecifications.GetPcName();

            IconDeviceType.Kind = PCSpecifications.GetPcType() == PCSpecifications.PcType.Desktop ?
                MaterialDesignThemes.Wpf.PackIconKind.DesktopTowerMonitor
                :
                MaterialDesignThemes.Wpf.PackIconKind.Laptop;

            computerspecs.Add(new Computerspec() { Title = "Система", Value = TBlockWindowsVersion.Text});

            computerspecs.Add(new Computerspec() { Title = "Процессор", Value = PCSpecifications.GetProcessor() });

            foreach (string videocontroller in PCSpecifications.GetVideoControllers())
                computerspecs.Add(new Computerspec() { Title = "Видеокарта", Value = videocontroller });

            foreach (string disk in PCSpecifications.GetDisks())
                computerspecs.Add(new Computerspec() { Title = "Накопитель", Value = disk });

            foreach (string ram in PCSpecifications.GetRAMS())
                computerspecs.Add(new Computerspec() { Title = "Оперативная память", Value = ram });

            foreach (string nic in PCSpecifications.GetNICS())
                computerspecs.Add(new Computerspec() { Title = "Сетевое устройство", Value = nic });

            DGridHardware.ItemsSource = computerspecs;

            #endregion

            #region Сеть

            CBoxEnableProxy.IsChecked = isProxyEnabled;
            SPanelProxySettings.IsEnabled = isProxyEnabled;

            string fullProxy = Network.GetCurrentProxy();
            string proxyAdress = fullProxy.Split(':')[0];
            string proxyPort = fullProxy.Split(':')[1];

            computernetwork.Add(new Computerspec() { Title = "proxyAdress", Value = proxyAdress, IsNetwork = true });

            computernetwork.Add(new Computerspec() { Title = "proxyPort", Value = proxyPort, IsNetwork = true });

            TBoxProxyAdress.Text = proxyAdress;
            TBoxProxyPort.Text = proxyPort;

            bool isDHCP = Network.GetDHCPStatus();

            RButtonDHCPEnabled.IsChecked = isDHCP;
            RButtonDHCPDisabled.IsChecked = !isDHCP;

            CardNetworkSettings.IsEnabled = !isDHCP;
            ButtonSaveNetworkSettings.IsEnabled = !isDHCP;

            string currentNIC = PCSpecifications.GetNICS().FirstOrDefault("");

            StaticNetworkParams currentParams;

            if (string.IsNullOrWhiteSpace(currentNIC))
                currentParams = new StaticNetworkParams() { IP = "", SubnetMask = "", Gateway = "", DNS = "" };
            else
                currentParams = Network.GetNetworkParams(currentNIC);

            computernetwork.Add(new Computerspec() { Title = "IP", Value = currentParams.IP, IsNetwork = true });
            computernetwork.Add(new Computerspec() { Title = "SubnetMask", Value = currentParams.SubnetMask, IsNetwork = true });
            computernetwork.Add(new Computerspec() { Title = "Gateway", Value = currentParams.Gateway, IsNetwork = true });
            computernetwork.Add(new Computerspec() { Title = "DNS", Value = currentParams.DNS, IsNetwork = true });

            TBoxIPV4.Text = currentParams.IP;
            TBoxSubnetMask.Text = currentParams.SubnetMask;
            TBoxGateway.Text = currentParams.Gateway;
            TBoxDNS.Text = currentParams.DNS;

            #endregion
        }

        void InitDbInfo()
        {
            currentComputer = App.DbComputers.Computers
                .Where(c => c.Name.Equals(TBlockPcName.Text))
                .FirstOrDefault();

            if (currentComputer != null)
            {
                TBlockLastUpdate.Text = currentComputer.LastUpdate.ToString("dd/MM/yyyy");
                TBlockPcDescription.Text = currentComputer.Description;
                TBlockPcInventoryNum.Text = currentComputer.inventoryNum;
            }
            else
            {
                TBlockLastUpdate.Text = "--/--/----";
                TBlockPcDescription.Text = "Отсутствует...";
                TBlockPcInventoryNum.Text = "-";
            }
        }
    }
}
