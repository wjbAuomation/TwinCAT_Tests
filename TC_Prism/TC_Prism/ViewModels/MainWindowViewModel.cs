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
using System.Threading;

namespace TC_Prism.ViewModels
{
    class MainWindowViewModel : BindableBase
    {

        private TcAdsClient _tcAds = null;
        AdsStream readStream = new AdsStream(sizeof(UInt32));

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
                _tcAds = new TcAdsClient();

                Task resultString = Task.Factory.StartNew(() =>
                 {
                     ConnectADS(_tcAds, _selectedConnection.AdsAddress);
                 });
            }
        }

        private void ConnectADS(TcAdsClient adsClient, string adsAddress)
        {

            adsClient.Connect(adsAddress, 851);
            if (adsClient.IsConnected)
            {
                Console.WriteLine("Connected");
                adsClient.AdsNotification += Client_AdsNotification;

                int notificationHandle = 0;
                try
                {
                    notificationHandle = adsClient.AddDeviceNotification("MAIN.Blinker", readStream, AdsTransMode.OnChange, 100, 0, null);
                    Thread.Sleep(10000);
                }
                finally
                {
                    adsClient.DeleteDeviceNotification(notificationHandle);
                    adsClient.AdsNotification -= Client_AdsNotification;
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

        private void Client_AdsNotification(object sender, AdsNotificationEventArgs e)
        {
            int offset = (int)e.DataStream.Position;
            int length = (int)e.DataStream.Length;

            e.DataStream.Position = offset;
            AdsBinaryReader reader = new AdsBinaryReader(e.DataStream);

            // Read the Unmarshalled data
            //byte[] data = reader.ReadBytes(length);

            // Or here we know about UDINT type --> can be marshalled as UINT32
            uint nCounter = reader.ReadUInt32();
            Console.WriteLine(DateTime.Now.ToString()+" "+nCounter.ToString());
        }

    }
}
