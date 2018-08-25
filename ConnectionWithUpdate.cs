using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateDataBase
{
    class ConnectionWithUpdate
    {
        public ConnectionWithUpdate()
        {
            ConnectionList = new Connection();
            ConnectionList.InitDBConnection();
        }
        public Connection ConnectionList { get; set; }
    }

}
