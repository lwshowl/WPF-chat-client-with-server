using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using WpfApp5.ViewModes;

namespace WpfApp5
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int WM_NCLBUTTONDOWN = 0XA1;   //.定义鼠標左鍵按下
        private const int HTCAPTION = 2;
        Buddy buddy = new Buddy();
        public MainWindow()
        {

            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;

        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (lognameBox.Text.ToString().Equals("")||logPWDBOX.Password.ToString().Equals(""))
            {
                tipmsg.Text = "账号或密码不能为空";
            }
            else
            {
             

                Login login = new Login();
                Login.loginname = lognameBox.Text.Trim();
                Login.loginpwd = logPWDBOX.Password.Trim();
                string flag = login.Verify(Login.loginname, Login.loginpwd);

                if (flag.Equals("true"))
                {
               
                    buddy.Getbuddy();
                    tipmsg.Text = "成功";
                    this.DataContext = new MainWindowViewModel();
                    Window2 win = new Window2();
                    win.Show();
                    this.Close();
                }
                else if(flag.Equals("连接服务器失败"))
                {
                    tipmsg.Text = "连接服务器失败，服务器可能维护中";
                }
                else
                {
                    tipmsg.Text = "账号或密码错误";
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Move_MouseMove(object sender, MouseEventArgs e)
        {

            
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Point pp = Mouse.GetPosition(e.Source as FrameworkElement);//WPF方法
            //Point ppp = (e.Source as FrameworkElement).PointToScreen(pp);//WPF方法
            if (!(pp.X > 180 && pp.X < 440 && pp.Y > 200&& pp.Y < 300))
            {
                    this.DragMove();
            }

        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri));
        }
    }
}
