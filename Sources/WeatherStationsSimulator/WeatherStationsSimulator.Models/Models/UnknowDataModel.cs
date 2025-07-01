using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherStationsSimulator.Models.Models
{
    public class UnknowDataModel : BaseModel
    {
        #region Fields
        private readonly Random _random = new Random();
        private readonly byte[] _randomeBytes = { 0x00, 0x01 };
        #endregion

        #region Methods
        public override byte[] CreateTitle(int dataSize)
        {
            var labelTime = CreatelabelTime();
            var type = new byte[] { 0x00, 0x14 };
            var number = new byte[] { 0x00, 0x00 };
            var dataSizePac = new byte[] { 0x00, (byte)dataSize };

            var result = labelTime.Concat(type).Concat(number).Concat(dataSizePac).ToArray();
            return result;
        }

        public override byte[] CreateData(int dataSize)
        {
            var listBytes = new List<byte>();
            for (var i = 0; i < dataSize; i++)
            {
                listBytes.Add(_randomeBytes[_random.Next(0, 1)]);
            }
            return listBytes.ToArray();
        }
        #endregion
    }
}
