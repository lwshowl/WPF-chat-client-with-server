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

namespace WpfApp5
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Button_MouseEnter_1(object sender, MouseEventArgs e)
        {
            closebtn.Background = Brushes.Red;

        }

        private void Closebtn_MouseLeave(object sender, MouseEventArgs e)
        {
            closebtn.Background = Brushes.Transparent;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void SelectPeopleBtn_Click(object sender, RoutedEventArgs e)
        {
            Window3 edit = new Window3();
            edit.ShowDialog();
            edit.Topmost = true;
        }
    }
}
