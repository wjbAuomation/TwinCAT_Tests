using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;
using System.IO;
using TC_Prism.Models;

namespace TC_Prism.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private Services.IConnectionList _connectionList = null;

        // constructor dependency injection
        public MainWindowViewModel(Services.IConnectionList connectionList)
        {
            _connectionList = connectionList;
        }

        public ObservableCollection<Connection> Connections { get; private set; } = new ObservableCollection<Connection>();

        private Connection _selectedConnection = null;
        public Connection SelectedConnection
        {
            get => _selectedConnection;
            set
            {
                if(SetProperty<Connection>(ref _selectedConnection, value))
                {
                    Debug.WriteLine(_selectedConnection.Name ?? "no connection selected");
                }
            }
        }

        private DelegateCommand _commandLoad = null;
        public DelegateCommand CommandLoad =>
            _commandLoad ?? (_commandLoad = new DelegateCommand(CommandLoadExecute));

        private DelegateCommand _commandConnect = null;
        public DelegateCommand CommandConnect =>
            _commandConnect ?? (_commandConnect = new DelegateCommand(CommandConnectExecute));

        private void CommandConnectExecute()
        {
            if(_selectedConnection != null)
            {
                TcAdsClient tcAds = new TcAdsClient();
                tcAds.Connect(_selectedConnection.AdsAddress, 801);
                if (tcAds.IsConnected)
                {
                    Debug.WriteLine("Connected");
                }
            }
        }

        private void CommandLoadExecute()
        {
            Connections.Clear();
            List<Connection> list = _connectionList.GetAll();

            foreach(Connection con in list)
            {
                Connections.Add(con);
            }
        }



    }
}
