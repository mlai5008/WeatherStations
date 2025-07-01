using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using WpfWeatherStations.Common.Commands;
using WpfWeatherStations.Common.Interfaces;
using WpfWeatherStations.Models.Models;
using WpfWeatherStations.ViewModels.ViewModels.Base;

namespace WpfWeatherStations.ViewModels.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private readonly IFileDialogService _fileDialogService;
        private readonly IDispatcherService _dispatcherService;
        private string _text = "waiting...";
        private ObservableCollection<DataModel> _dataCollection = new ObservableCollection<DataModel>();
        #endregion

        #region Constructor
        public MainViewModel(IFileDialogService fileDialogService, IDispatcherService dispatcherService)
        {
            _fileDialogService = fileDialogService;
            _dispatcherService = dispatcherService;

            OpenFileDialogCommand = new RelayCommand(OnOpenFileDialogExecuted);
            SaveFileDialogCommand = new RelayCommand(OnSaveFileDialogExecuted);
            
            TemperatureCollection = new ObservableCollection<DataModel>();
            HumidityCollection = new ObservableCollection<DataModel>();
            PressureCollection = new ObservableCollection<DataModel>();
            WindSpeedCollection = new ObservableCollection<DataModel>();

            Task.Run(() => InitializeService());
        }
        #endregion

        #region Properties
        public string Text
        {
            get { return _text; }
            set { SetValue(ref _text, value); }
        }

        public ObservableCollection<DataModel> DataCollection
        {
            get { return _dataCollection; }
            set { SetValue(ref _dataCollection, value); }
        }

        public ObservableCollection<DataModel> TemperatureCollection { get; set; }
        public ObservableCollection<DataModel> HumidityCollection { get; set; }
        public ObservableCollection<DataModel> PressureCollection { get; set; }
        public ObservableCollection<DataModel> WindSpeedCollection { get; set; }
        #endregion

        #region Commands
        public RelayCommand OpenFileDialogCommand { get; }
        public RelayCommand SaveFileDialogCommand { get; }
        #endregion

        #region Methods
        private void OnOpenFileDialogExecuted(object obj)
        {
            string filePath = _fileDialogService.OpenFileDialog();
            string readText = File.ReadAllText(filePath);
            var dataLineElements = readText.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None).ToList();
            CreateDataCollection(dataLineElements);
        }

        private void OnSaveFileDialogExecuted(object obj)
        {
            string filePath = _fileDialogService.SaveFileDialog();
            string newTxtFile = string.Join(Environment.NewLine, DataCollection.Select(i => i.FullPackage));
            File.WriteAllText(filePath, newTxtFile);
        }

        private Task InitializeService()
        {
            IPHostEntry ipHost = Dns.Resolve("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

            Socket socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketListener.Bind(ipEndPoint);
            socketListener.Listen(10);
            Socket handler = socketListener.Accept();
            try
            {
                while (true)
                {
                    byte[] byteData = new byte[0];
                    while (true)
                    {
                        byte[] bytes = new byte[1024];
                        int byteRec = handler.Receive(bytes);
                        byteData = bytes.Take(byteRec).ToArray();
                        if (byteData.First() != 0)
                            break;
                    }
                    if (CheckSumBytesPackage(byteData))
                    {
                        var dataB = ParseByteData(byteData);
                        Text = dataB.FullPackage;
                        _dispatcherService.UpdateCollection(DataCollection, dataB);
                    }
                }
            }
            catch (Exception e)
            {
                // logs(e)
            }
            finally
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            return Task.FromResult(7);
        }

        private bool CheckSumBytesPackage(byte[] byteData)
        {
            var length = byteData.Length;
            ushort sum = 0;
            foreach (byte b in byteData.Take(length-2).Skip(4))
                sum += b;
            var sumBytes = new byte[] { (byte)(sum & 255), (byte)~sum };
            var isCheck = (byteData.Last() == sumBytes.Last()) && (byteData[length - 2] == sumBytes.First());
            return isCheck;
        }

        private string ParsePaket(byte[] byteData)
        {
            return string.Join("", byteData.Select(c => ((int)c).ToString("X")).ToArray());
        }

        private DataModel ParseByteData(byte[] byteData)
        {
            var sizeData = byteData[15];
            var dataArray = byteData.Skip(16).Take(sizeData).ToArray();
            var data = ParsePaket(dataArray);

            var model = new DataModel
            {
                Type = byteData[11],
                Value = data,
                Time = DateTime.Now.ToString("HH:mm:ss"),
                FullPackage = ParsePaket(byteData)
            };
            if (model.Type == 1)
            {
                UpdateCollection(TemperatureCollection, model);
            }
            if (model.Type == 2)
            {
                UpdateCollection(HumidityCollection, model);
            }
            if (model.Type == 3)
            {
                UpdateCollection(PressureCollection, model);
            }
            if (model.Type == 16)
            {
                UpdateCollection(WindSpeedCollection, model);
            }

            return model;
        }

        private void UpdateCollection<T>(ObservableCollection<T> collection, T model)
        {
            _dispatcherService.UpdateCollection<T>(collection, model);
        }

        private void CreateDataCollection(List<string> newData)
        {
            var listData = newData.Select(data => new DataModel
            {
                FullPackage = data
            }).ToList();

            DataCollection = new ObservableCollection<DataModel>(listData);
        }
        #endregion
    }
}