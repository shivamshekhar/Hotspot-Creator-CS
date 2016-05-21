using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hotspot_Creator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            showHotspotStatus();
        }

        private int substringIndex(String str, String substr)
        {
            int index = 0,j;
            Char[] strarr = new Char[1000];
            Char[] substrarr = new Char[1000];
            strarr = str.ToCharArray();
            substrarr = substr.ToCharArray();
            if (str.Contains(substr) == true)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (strarr[i] == substrarr[0])
                    {
                        for (j = 0; j < substr.Length; j++)
                        {
                            if (strarr[i + j] != substrarr[j])
                                break;
                        }
                        if (j == substr.Length)
                        {
                            index = i;
                            break;
                        }
                    }

                }
            }
            return index;
        }

        private void hotspotstate(string str)
        {
            if(str.Contains("Status                 : Not started") == true && str.Contains("Status                 : Started") == false)
            {
                Uri source = new Uri(@"pack://application:,,,/Assets/0.png");
                BitmapImage img = new BitmapImage(source);
                statusimage.Source = img;
            }

            else if(str.Contains("Status                 : Started") == true && str.Contains("Status                 : Not started") == false)
            {
                Uri source = new Uri(@"pack://application:,,,/Assets/1.png");
                BitmapImage img = new BitmapImage(source);
                statusimage.Source = img;
            }

            
        }

        private void showHotspotStatus()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd");
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardInput = true;
            var proc = Process.Start(startInfo);
            proc.StandardInput.WriteLine(@"netsh wlan show hostednetwork");
            proc.StandardInput.WriteLine("exit");
            string s = proc.StandardOutput.ReadToEnd();
            status_box.Text = extractinfo(s);
            hotspotstate(s);
        }

        private string extractinfo(string str)
        {
            char[] sarr = new Char[1000];
            int lowerindex, upperindex,k = 0;
            sarr = str.ToCharArray();
            int tempindex = substringIndex(str, "Hosted");
            lowerindex = tempindex;
            tempindex = str.Length - 1;
            while (true)
            {
                if (sarr[tempindex - k] == ':')
                {
                    tempindex = tempindex - k;
                    break;
                }
                else
                    k++;
            }
            upperindex = tempindex;
            string status = str.Substring(lowerindex, upperindex - lowerindex - 2);
            return status;

        }

        
        private async void stopbutton_Click(object sender, RoutedEventArgs e)
        {
            String command;
            command = "/C netsh wlan stop hostednetwork";
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            Process.Start(startInfo);
            await Task.Delay(500);
            
            showHotspotStatus();
        }

        private async void startbutton_Click(object sender, RoutedEventArgs e)
        {
            String command;
            command = "/C netsh wlan start hostednetwork";
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            Process.Start(startInfo);
            await Task.Delay(500);
            
            showHotspotStatus();
        }

        private void refreshbutton_Click(object sender, RoutedEventArgs e)
        {
            showHotspotStatus();
        }

        private async void createhotspotbutton_Click(object sender, RoutedEventArgs e)
        {
            String command;
            command = "/C netsh wlan set hostednetwork ";
            if (comboBox.SelectedIndex == 0)
            {
                command = command + "mode=allow ";
            }
            else
            {
                command = command + "mode=disallow ";
            }

            command = command + "ssid=" + namebox.Text.Replace(' ','_') + " key=" + passwordbox.Password;

            if (namebox.Text.Length == 0)
                errormsg(0);
            else if (passwordbox.Password.Length < 8)
                errormsg(1);
            else if (comboBox.SelectedIndex == -1)
                errormsg(2);
            else
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = command;
                Process.Start(startInfo);
                await Task.Delay(500);
                showHotspotStatus();
            }
            
        }

        private void errormsg(int index)
        {
            switch(index)
            {
                case 0:
                    MessageBox.Show("Please enter the name of the hotspot!", "Error");
                    break;
                case 1:
                    MessageBox.Show("The password must be at least 8 characters long!", "Error");
                    break;
                case 2:
                    MessageBox.Show("Please select a mode option!", "Error");
                    break;
            }
        }
    }
}
