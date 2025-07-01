using System;
using Microsoft.Win32;
using WpfWeatherStations.Common.Interfaces;

namespace WpfWeatherStations.Services.Services
{
    public class FileDialogService : IFileDialogService
    {
        #region Fields
        public string FilePath { get; set; }
        #endregion

        #region Methods
        public string OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog {Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"};
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return FilePath;
            }
            return String.Empty;
        }

        public string SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return FilePath;
            }
            return String.Empty;
        }
        #endregion
    }
}