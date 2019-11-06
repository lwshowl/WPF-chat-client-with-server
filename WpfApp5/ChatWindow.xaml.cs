using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp5.ViewModes;
using System.Runtime.InteropServices;  //for  MarshalAs
using System.IO;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Net;
using System.Text.RegularExpressions;

namespace WpfApp5
{ //
    /// <summary>
    /// Window2.xaml 的交互逻辑
    /// </summary>
    public partial class Window2 : Window
    {
        public delegate void DeleFunc();
        public static string fileIP;
        public static string messagebuff;
        public static bool messagebufislocked;
        public static int msglistcount;
        public static string filepath;
        public static string filename;
        public static string recfilepath;
        public static string recfilename;
        public ListView[] listViews;
        public static Hashtable table = new Hashtable();      //好友的账号--->生成的序号
        public static Hashtable index_name_table = new Hashtable(); //好友的账号----->生成的姓名

        public static bool IPC_flag;
        const int WM_COPYDATA = 0x004A; //接受进程间信息的变量
        const int WM_MYSYMPLE = 0x005A;

        public static string chattarget;//保存聊天人名字
        public static Window2 Instance;

        public Window2()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            Instance = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pp = Mouse.GetPosition(e.Source as FrameworkElement);//WPF方法
            //Point ppp = (e.Source as FrameworkElement).PointToScreen(pp);//WPF方法
            if (pp.X > 0 && pp.X < 820 && pp.Y > 0 && pp.Y < 80)
            {
                if (!(pp.X > 140 && pp.X < 260 && pp.Y > 25 && pp.Y < 60))
                {
                    this.DragMove();
                }
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            StackPanel msgpane = new StackPanel();   //申请一个panel 放到listview里面
            TextBlock peopletext = new TextBlock();  //准备在panel 里面放第一个textblock
            TextBlock msgtext = new TextBlock();     // 准备在panel 里放第二个textblock

            peopletext.Text = Login.login_nick_name + "  " + DateTime.Now.ToLongTimeString().ToString();   // 显示消息的时间20:16:16;
            peopletext.FontSize = 13;
            peopletext.Foreground = new SolidColorBrush(Color.FromRgb(0, 128, 64));
            peopletext.VerticalAlignment = VerticalAlignment.Top;

            msgtext.FontSize = 13;
            msgtext.Text = "  " + SendMsgBox.Text;
            msgtext.VerticalAlignment = VerticalAlignment.Bottom;
            msgtext.TextWrapping = TextWrapping.Wrap;
            msgtext.Width = 560;

            msgpane.Children.Add(peopletext);                  //把两个textblock 放到pane里
            msgpane.Children.Add(msgtext);
            //msgpane.VerticalAlignment = VerticalAlignment.Top;
            //msgpane.HorizontalAlignment = HorizontalAlignment.Left;

            listViews[FriendList.SelectedIndex].Items.Add(msgpane);                      //把pane 添加进listview
            listViews[FriendList.SelectedIndex].ScrollIntoView(msgpane);                //view跳转到最下方的最新消息

            string target = ChatTargetBlock.Text;//消息发送目的地为选中的人的标题


            string sendMessage = "Msg" + "," + SendMsgBox.Text + "," + target.Trim();//发送到服务端的内容
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket = Login.GetSocket();
            //MessageBox.Show("我发送了消息:" + sendMessage);
            clientSocket.Send(Encoding.UTF8.GetBytes(sendMessage + "\r"));//向服务器发送数据，需要发送中文则需要使用Encoding.UTF8.GetBytes()，否则会乱码
            SendMsgBox.Text = "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyNickname.Text = Login.login_nick_name;
            messagebufislocked = false;
            Thread listenthread = new Thread(run);
            listenthread.SetApartmentState(ApartmentState.STA);
            listenthread.IsBackground = true;
            listenthread.Start();
            msglistcount = 0;
            listViews = new ListView[Buddy.buddycount];
            for (int i = 0; i < Buddy.buddycount; i++)
            {
                listViews[i] = new ListView();
                listViews[i].BorderBrush = null;
                listViews[i].HorizontalAlignment = HorizontalAlignment.Left;
                listViews[i].Height = 362;
                Thickness myThickness = new Thickness();
                myThickness.Left = 0;
                myThickness.Top = 0;
                myThickness.Right = 0;
                myThickness.Bottom = 0;
                listViews[i].Margin = myThickness;
                listViews[i].VerticalAlignment = VerticalAlignment.Top;
                listViews[i].Width = 607;
                listViews[i].Visibility = Visibility.Hidden;
                listViews[i].HorizontalContentAlignment = HorizontalAlignment.Left;

                //listViews[i]
                //GridView gv = new GridView();
                //GridViewColumn gvc = new GridViewColumn();
                //gvc.Header = "1";
                //gvc.Width = 300;
                //gv.Columns.Add(gvc);
                //listViews[i].Items.Add(gv);

                msggrid.Children.Add(listViews[i]);


            }
        }

        private void FriendList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listViews[FriendList.SelectedIndex].Visibility = Visibility.Visible;

            for (int i = 0; i < Buddy.buddycount; i++)
            {
                if (i != FriendList.SelectedIndex)
                {
                    listViews[i].Visibility = Visibility.Hidden;
                }
            }
            msgboxgrid.Visibility = Visibility.Visible;
            topline.Visibility = Visibility.Visible;
            bottomline.Visibility = Visibility.Visible;
            SendMsgBox.Visibility = Visibility.Visible;
            SendButton.Visibility = Visibility.Visible;
            NicknameBlock.Visibility = Visibility.Visible;
        }

        public void IPC_REC_MSG()
        {
            StackPanel msgpane = new StackPanel();   //申请一个panel 放到listview里面
            TextBlock peopletext = new TextBlock();  //准备在panel 里面放第一个textblock
            TextBlock msgtext = new TextBlock();     // 准备在panel 里放第二个textblock

            while (messagebufislocked == true)  //当UI 和收消息的共享资源上锁时什么都不做
            {

            }
            messagebufislocked = true;          //上锁，开始操作
            string[] account = messagebuff.Split(','); //操作
            messagebufislocked = false;       //关锁

            string realmsg = account[1];   //真正的消息
            string txtsender = account[2]; //发消息来的人

            peopletext.Text = Buddy.account_nickname_table[txtsender] + "  " + DateTime.Now.ToLongTimeString().ToString();   // 显示消息的时间20:16:16;
            peopletext.FontSize = 13;
            peopletext.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            peopletext.VerticalAlignment = VerticalAlignment.Top;

            msgtext.FontSize = 13;
            msgtext.Text = "  " + realmsg;
            msgtext.VerticalAlignment = VerticalAlignment.Bottom;
            msgtext.TextWrapping = TextWrapping.Wrap;
            msgtext.Width = 560;


            msgpane.Children.Add(peopletext);                  //把两个textblock 放到pane里
            msgpane.Children.Add(msgtext);
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                listViews[(int)table[txtsender] - 1].Items.Add(msgpane);          //把消息pane 加进去
                listViews[(int)table[txtsender] - 1].ScrollIntoView(msgpane);   //view跳转到最下方的最新消息
            }
            ));
        }

        public void SEND_FILE()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
            {
                filepath = openFileDialog.FileName;
                filename = Path.GetFileName(filepath);
                StackPanel msgpane = new StackPanel();   //申请一个panel 放到listview里面
                TextBlock peopletext = new TextBlock();  //准备在panel 里面放第一个textblock
                TextBlock filetext = new TextBlock();

                Image image = new Image();
                image.Source = new BitmapImage(new Uri("image/fileimg.jpg", UriKind.Relative));

                peopletext.Text = Login.login_nick_name + "  " + DateTime.Now.ToLongTimeString().ToString();   // 显示消息的时间20:16:16;
                peopletext.FontSize = 13;
                peopletext.Foreground = new SolidColorBrush(Color.FromRgb(0, 128, 64));
                peopletext.VerticalAlignment = VerticalAlignment.Top;

                filetext.Text = filename;  // 显示消息的时间20:16:16;
                filetext.FontSize = 13;
                filetext.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                filetext.VerticalAlignment = VerticalAlignment.Top;

                BitmapImage img = new BitmapImage(new Uri("pack://application:,,,/image/fileimg.jpg"));

                image.Width = 80;
                image.Height = 80;
                image.HorizontalAlignment = HorizontalAlignment.Left;

                msgpane.Children.Add(peopletext);                  //把两个textblock 放到pane里
                msgpane.Children.Add(image);
                msgpane.Children.Add(filetext);

                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {
                    listViews[FriendList.SelectedIndex].Items.Add(msgpane);          //把消息pane 加进去
                }
                ));

                //发送请求文件信息到对方
                string sendMessage = "PortFile" + "," + filename + "," + ChatTargetBlock.Text.Trim();//发送到服务端的内容
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket = Login.GetSocket();
                clientSocket.Send(Encoding.UTF8.GetBytes(sendMessage + "\r"));//向服务器发送数据，需要发送中文则需要使用Encoding.UTF8.GetBytes()，否则会乱码
                //clientSocket.Send(Encoding.UTF8.GetBytes("PortFile," + "null,null" + "\r"));
                Thread sendfile = new Thread(StartSend);
                sendfile.Start();

            }




        }

        public void RECIVE_FILE()
        {


            while (messagebufislocked == true)  //当UI 和收消息的共享资源上锁时什么都不做
            {

            }
            messagebufislocked = true;          //上锁，开始操作
            string[] account = messagebuff.Split(','); //操作
            messagebufislocked = false;       //关锁

            string realmsg = account[1];   //真正的消息

            string txtsender = account[2]; //发消息来的人

            StackPanel msgpane = new StackPanel();   //申请一个panel 放到listview里面
            TextBlock peopletext = new TextBlock();  //准备在panel 里面放第一个textblock
            TextBlock filetext = new TextBlock();

            msgpane.HorizontalAlignment = HorizontalAlignment.Left;

            peopletext.Text = Buddy.account_nickname_table[txtsender] + "  " + DateTime.Now.ToLongTimeString().ToString();   // 显示消息的时间20:16:16;
            peopletext.FontSize = 13;
            peopletext.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            peopletext.VerticalAlignment = VerticalAlignment.Top;

            filetext.Text = realmsg;  
            filetext.FontSize = 13;
            filetext.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            filetext.VerticalAlignment = VerticalAlignment.Top;



            Image image = new Image();
            image.Source = new BitmapImage(new Uri("image/fileimg.jpg", UriKind.Relative));
            image.HorizontalAlignment = HorizontalAlignment.Left;

            StackPanel btnpane = new StackPanel();
            btnpane.Orientation = Orientation.Horizontal;

            Button acc = new Button();
            acc.Content = "接受";
            acc.Width = 60;
            acc.Height = 30;
            acc.Click += FileACCbutton_Click;
            acc.Tag = 1;
            Button decline = new Button();
            decline.Content = "拒绝";
            decline.Width = 60;
            decline.Height = 30;
            decline.Click += FileDECbutton_Click;
            decline.Tag = 2;

            btnpane.Children.Add(acc);
            btnpane.Children.Add(decline);


            image.Width = 70;
            image.Height = 70;

            msgpane.Children.Add(peopletext);                  //把两个textblock 放到pane里
            msgpane.Children.Add(image);
            msgpane.Children.Add(filetext);
            msgpane.Children.Add(btnpane);


            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                listViews[(int)table[txtsender] - 1].Items.Add(msgpane);          //把消息pane 加进去
                //listViews[(int)table[txtsender] - 1].Items.Add(btnpane);
                listViews[(int)table[txtsender] - 1].ScrollIntoView(msgpane);   //view跳转到最下方的最新消
            }
            ));


        }

        public void run()   //接收服务器发来的消息的线程 函数
        {

            try
            {
                //不断的接收信息
                while (true)
                {
                    byte[] data = new byte[1024];
                    Login.clientSocket.Receive(data);//接收返回数据
                    string returnMsg = Encoding.UTF8.GetString(data);
                    string[] account = returnMsg.Split(','); //接受到的消息格式 类型，消息，发送给谁(或者port)
                    string msgtype = account[0];   // 消息类型，1为消息，2为文件消息

                    if (!string.IsNullOrWhiteSpace(returnMsg))
                    {
                        if (msgtype.Equals("Msg"))
                        {
                            returnMsg = returnMsg.Trim();
                            returnMsg = returnMsg.Replace("\r", "");
                            returnMsg = returnMsg.Replace("\n", "");
                            returnMsg = returnMsg.Replace("\0", "");//格式化接受的信息
                            // MessageBox.Show("收到消息" + returnMsg, Login.loginname);//提示接收到消息
                            while (messagebufislocked == true)   //当UI 和收消息的共享资源上锁时什么都不做
                            {

                            }
                            messagebufislocked = true;      //上锁
                            messagebuff = returnMsg;       //开始操作
                            messagebufislocked = false;   //关锁
                            System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
new DeleFunc(IPC_REC_MSG));
                        }
                        else if (msgtype.Equals("File")) //收到了文件消息
                        {

                            returnMsg = returnMsg.Trim();
                            returnMsg = returnMsg.Replace("\r", "");
                            returnMsg = returnMsg.Replace("\n", "");
                            returnMsg = returnMsg.Replace("\0", "");  //格式化接受的信息
                            while (messagebufislocked == true)   //当UI 和收消息的共享资源上锁时什么都不做
                            {

                            }
                            messagebufislocked = true;      //上锁
                            messagebuff = returnMsg;       //开始操作
                            messagebufislocked = false;   //关锁
                            recfilename = account[1].ToString().Trim();
                            System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
new DeleFunc(RECIVE_FILE));
                        }
                        else if (msgtype.Equals("ACCFile"))
                        {
                            returnMsg = returnMsg.Trim();
                            returnMsg = returnMsg.Replace("\r", "");
                            returnMsg = returnMsg.Replace("\n", "");
                            returnMsg = returnMsg.Replace("\0", "");  //格式化接受的信息
                            while (messagebufislocked == true)   //当UI 和收消息的共享资源上锁时什么都不做
                            {

                            }
                            messagebufislocked = true;      //上锁
                            messagebuff = returnMsg;       //开始操作
                            messagebufislocked = false;   //关锁
                            fileIP = account[1].ToString().Trim();

                            //Thread sendfile = new Thread(Send);
                            // sendfile.Start();

                        }
                    }

                }
            }
            catch (IOException e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void SendMsgBox_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {

                SendButton_Click(sender, e);          //按enter 发送消息
            }


        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            fontbutton.Background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
        }

        private void Fontbutton_MouseLeave(object sender, MouseEventArgs e)
        {
            fontbutton.Background = Brushes.Transparent;


        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window1 infowindow = new Window1();
            infowindow.ShowDialog();
            infowindow.Topmost = true;
        }

        private void Button_MouseEnter_1(object sender, MouseEventArgs e)
        {
            closebtn.Background = Brushes.Red;

        }

        private void Closebtn_MouseLeave(object sender, MouseEventArgs e)
        {
            closebtn.Background = Brushes.Transparent;
        }

        private void Button_MouseEnter_2(object sender, MouseEventArgs e)
        {
            maximizebtn.Background = new SolidColorBrush(Color.FromRgb(209, 214, 234));
        }

        private void Maximizebtn_MouseLeave(object sender, MouseEventArgs e)
        {
            maximizebtn.Background = Brushes.Transparent;
        }

        private void Button_MouseEnter_3(object sender, MouseEventArgs e)
        {
            minimizebtn.Background = new SolidColorBrush(Color.FromRgb(209, 214, 234));
        }

        private void Minimizebtn_MouseLeave(object sender, MouseEventArgs e)
        {
            minimizebtn.Background = Brushes.Transparent;
        }

        private void Maximizebtn_Click(object sender, RoutedEventArgs e)
        {
            //this.WindowState = WindowState.Maximized;
        }

        private void Button_MouseEnter_4(object sender, MouseEventArgs e)
        {
            filebtn.Background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
        }

        private void Filebtn_MouseLeave(object sender, MouseEventArgs e)
        {
            filebtn.Background = Brushes.Transparent;
        }

        private void Filebtn_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
new DeleFunc(SEND_FILE));


        }

        private void FileACCbutton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;
            button.IsEnabled = false;




            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.Description = "选择Word文档生成的文件夹";
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.ShowDialog();
            if (folderBrowserDialog.SelectedPath == string.Empty)
            {
                return;
            }
            recfilepath = folderBrowserDialog.SelectedPath;


            string sendMessage = "ACCFile" + "," +recfilename + "," + ChatTargetBlock.Text.Trim();//发送到服务端的内容
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket = Login.GetSocket();
            clientSocket.Send(Encoding.UTF8.GetBytes(sendMessage + "\r"));//向服务器发送数据，需要发送中文则需要使用Encoding.UTF8.GetBytes()，否则会乱码

            Thread filethread = new Thread(Receive);
            filethread.Start();

        }

        private void FileDECbutton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;
            MessageBox.Show("老子不收");

            string sendMessage = "Msg" + "," + "对方拒绝了你的文件请求" + "," + ChatTargetBlock.Text.Trim();//发送到服务端的内容
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket = Login.GetSocket();
            clientSocket.Send(Encoding.UTF8.GetBytes(sendMessage + "\r"));//向服务器发送数据，需要发送中文则需要使用Encoding.UTF8.GetBytes()，否则会乱码

            button.IsEnabled = false;
        }



        public static void Receive()
        {

            TcpClient client = new TcpClient("ip adress", 8086);
            NetworkStream stream = client.GetStream();
   

            byte[] buffer = new byte[1024];

            stream.Read(buffer,0,buffer.Length);
            long receive = 0L,
           length = BitConverter.ToInt64(buffer, 0);



            stream.Read(buffer,0,buffer.Length);
            string name = Encoding.Default.GetString(buffer, 0, buffer.Length);
            name = name.Replace("\0", "");

            char[] sepertor = new char[2];
            string[] account = name.Split('#');

            name = account[2]; //发消息来的人


            //string fileName = Encoding.UTF8.GetString(buffer, 0, client.Receive(buffer));
            using (FileStream writer = new FileStream(Path.Combine(recfilepath, name), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                int received;
                while (receive < length)
                {
                    received = stream.Read(buffer,0,buffer.Length);
                    writer.Write(buffer, 0, received);
                    writer.Flush(); 
                    receive += (long)received;
                }
            }
            Console.WriteLine("Receive finish.");
        }




        public static void StartSend()
        {

            NetworkStream stream = null;
            BinaryWriter sw = null;
            FileStream fsMyfile = null;
            BinaryReader brMyfile = null;
            try
            {
                TcpClient client = new TcpClient("ip adress", 8085);
                stream = client.GetStream();
                sw = new BinaryWriter(stream);
                ///取得文件名字节数组
                byte[] fileNameBytes = Encoding.Default.GetBytes(filename);
                byte[] fileNameBytesArray = new byte[1024];
                Array.Copy(fileNameBytes, fileNameBytesArray, fileNameBytes.Length);
                ///写入流
                sw.Write(fileNameBytesArray, 0, fileNameBytesArray.Length);
                sw.Flush();
                ///获取文件内容字节数组
                ///byte[] fileBytes = returnbyte(filePath);
                fsMyfile = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                brMyfile = new BinaryReader(fsMyfile);
                ///写入流
                byte[] buffer = new byte[1024];
                int count = 0;
                while ((count = brMyfile.Read(buffer, 0, 1024)) > 0)
                {
                    sw.Write(buffer, 0, count);
                    sw.Flush();
                    buffer = new byte[1024];
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.StackTrace);

            }
            catch (IOException ioe)
            {
                Console.WriteLine(ioe.StackTrace);
           
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (brMyfile != null)
                {
                    brMyfile.Close();
                }
                if (fsMyfile != null)
                {
                    fsMyfile.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }
            
        }
        }  
}

