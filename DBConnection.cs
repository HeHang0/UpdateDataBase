using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateDataBase
{
    public class DBConnection : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string DBServerName { get; set; } = string.Empty;
        public string DBName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsChecked { get; set; } = true;

        public override string ToString()
        {
            return string.Format(@"Data Source = {0};Initial Catalog = {1};User Id = {2};Password = {3};", DBServerName, DBName, UserName, Password);
        }
    }
}
