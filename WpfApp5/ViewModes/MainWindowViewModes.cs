using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp5.ViewModes
{
    class MainWindowViewModel : BindableBase
    {
        List<string> buddylist = new List<string>();
        List<string> nicklist = new List<string>();

        public class Friend
        {

            public string Nickname { get; set; }
            public BitmapImage Head { get; set; }
            public string Account { get; set; }
        }

        public DelegateCommand<object> SelectItemChangedCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }

        public MainWindowViewModel()
        {
            Head = new BitmapImage(new Uri("pack://application:,,,/images/github.png"));
            nicklist = Buddy.nicknamelist;
            buddylist = Buddy.buddylist;
            friends = new ObservableCollection<Friend>();
            for (int i = 0;i< Buddy.buddycount;i++)
            {
                Random random = new Random();
                int[] head = new int[Buddy.buddycount];
                for (int j = 0; j < Buddy.buddycount; j++)//遍历数组显示结果
                {
                    head[j] = random.Next(1, 6);
                    
                }
               
                friends.Add(new Friend() { Account=buddylist[i], Nickname =nicklist[i], Head = new BitmapImage(new Uri("pack://application:,,,/images/head"+head[i]+".jpg")) });
               // friends.Add(new Friend() { Nickname = "糖宝", Head = new BitmapImage(new Uri("pack://application:,,,/Images/head2.jpg")) });
            }


            CloseCommand = new DelegateCommand(() => {

                Application.Current.Shutdown();

            });

            SelectItemChangedCommand = new DelegateCommand<object>((p) => {

                ListView lv = p as ListView;
                Friend friend = lv.SelectedItem as Friend;
                Head = friend.Head;
                Nickname = friend.Nickname;
                Account = friend.Account;
                
            });
        }

        private ObservableCollection<Friend> friends;

        public ObservableCollection<Friend> Friends
        {
            get { return friends; }
            set { friends = value; }
        }




        private BitmapImage head;

        public BitmapImage Head
        {
            get { return head; }
            set { SetProperty(ref head, value); }
        }

        private string account;
        private string nickname;
        public string Nickname
        {
            get { return nickname; }
            set { SetProperty(ref nickname, value); }
        }
        public string Account
        {
            get { return account; }
            set { SetProperty(ref account, value); }
        }

    }
}
