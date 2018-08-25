using ICSharpCode.AvalonEdit.Highlighting;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace UpdateDataBase
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ObservableCollection<DBConnection> ConnectionList { get; set; } = new ObservableCollection<DBConnection>();

        private DBConnection selectedConnection = new DBConnection();
        public DBConnection SelectedConnection { get; set; }
        public string SqlText { get; set; }

        public ICommand LoadDbConfigFile => new DelegateCommand(() =>
        {
            string configStr = OpenConfigFile();
            if (!string.IsNullOrWhiteSpace(configStr))
            {
                ConnectionList.Clear();
                ConnectionList.AddRange(AnalyzeDBList(configStr));
                SelectedConnection = ConnectionList.Count > 0 ? ConnectionList[0] : null;
                MessageBox.Show($"获取到数据库连接{ConnectionList.Count}条！");
            }
            else
            {
                MessageBox.Show($"已取消或未读取到数据库配置！");
            }
        });

        public ICommand LoadDbConfigText => new DelegateCommand(() =>
        {
            if (!string.IsNullOrWhiteSpace(SqlText))
            {
                ConnectionList.Clear();
                string configStr = SqlText;
                ConnectionList.AddRange(AnalyzeDBList(configStr));
                SelectedConnection = ConnectionList.Count > 0 ? ConnectionList[0] : null;
                SqlText = string.Empty;
                MessageBox.Show($"获取到数据库连接{ConnectionList.Count}条！");
            }
            else
            {
                MessageBox.Show("请输入配置文本！");
            }
        });

        private List<DBConnection> AnalyzeDBList(string config)
        {
            List<DBConnection> list = new List<DBConnection>();
            string pattern = @"connectionString=""server=(?<DBServerName>\S+);uid=(?<UserName>\S+);pwd=(?<Password>\S+);database=(?<DBName>\S+);";
            MatchCollection matches = Regex.Matches(config, pattern);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                list.Add(new DBConnection()
                {
                    DBServerName = groups["DBServerName"].Value,
                    DBName = groups["DBName"].Value,
                    UserName = groups["UserName"].Value,
                    Password = groups["Password"].Value
                });
            }
            return list;
        }

        private string OpenConfigFile()
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xml";
            ofd.Filter = "config文件|*.config|xml文件|*.xml|所有文件|*.*";
            if (ofd.ShowDialog() == true)
            {
                return File.ReadAllText(ofd.FileName);
            }
            return "";
        }

        public ICommand UpdateDataOneKey => new DelegateCommand(() =>
        {
            if (!string.IsNullOrWhiteSpace(SqlText))
            {
                StartUpdate(SqlText);
            }
            else
            {
                MessageBox.Show("请输入SQL语句后执行！");
            }
        });

        private void StartUpdate(string strSql)
        {
            var msg = string.Empty;
            List<Task> tList = new List<Task>();
            foreach (var item in ConnectionList)
            {
                if (item.IsChecked)
                {
                    Task task = Task.Run(() => {
                        int result = ExcuteCommand(item.ToString(), strSql);
                        msg += $"数据库{item.DBName}成功执行{result}条记录！\n";
                    });
                    tList.Add(task);
                }
            }
            Task.WhenAll(tList).ContinueWith(t => {
                MessageBox.Show(msg);
            });
        }

        private int ExcuteCommand(string connectionString, string strSql)
        {
            int count = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    SqlCommand updateCommand = new SqlCommand(strSql, conn);

                    count = updateCommand.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
            }
            return count;
        }
    }
}
