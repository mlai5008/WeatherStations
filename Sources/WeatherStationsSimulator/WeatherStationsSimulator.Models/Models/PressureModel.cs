using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherStationsSimulator.Models.Models
{
    public class PressureModel : BaseModel
    {
        #region Fields
        private readonly Random _random = new Random();
        private readonly byte[] _randomeBytes = { 0x14, 0xA1, 0x12, 0xB5, 0x1D, 0xB1, 0xFD, 0x35, 0xD4 };
        #endregion

        #region Methods
        public override byte[] CreateTitle(int dataSize)
        {
            var labelTime = CreatelabelTime();
            var type = new byte[] { 0x00, 0x03 };
            var number = new byte[] { 0x13, 0x11 };
            var dataSizePac = new byte[] { 0x00, (byte)dataSize };

            var result = labelTime.Concat(type).Concat(number).Concat(dataSizePac).ToArray();
            return result;
        }

        public override byte[] CreateData(int dataSize)
        {
            var listBytes = new List<byte>();
            for (var i = 0; i < dataSize; i++)
            {
                listBytes.Add(_randomeBytes[_random.Next(0, 8)]);
            }
            return listBytes.ToArray();
        }

        public override byte[] CreatePaket()
        {
            var dataSize = 2;
            var sm = CreateSynchromarker();
            var t = CreateTitle(dataSize);
            var d = CreateData(dataSize);
            var baseData = t.Concat(d).ToArray();
            var crc = ComputeChecksumBytes(baseData);

            return sm.Concat(t).Concat(d).Concat(crc).ToArray();
        }
        #endregion
    }
}
