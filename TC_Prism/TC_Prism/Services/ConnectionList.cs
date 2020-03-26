using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TC_Prism.Models;

namespace TC_Prism.Services
{
    public interface IConnectionList
    {
        List<Connection> GetAll();
    }
    class ConnectionList : IConnectionList
    {
        public List<Connection> GetAll()
        {
            return new List<Connection>() {
                new Connection { Name="Local connection", HostName="localhost", IPAddress="127.0.0.1", AdsAddress="172.20.136.133.1.1"} };
        }
    }
}
