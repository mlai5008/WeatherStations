using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherStationsSimulator.Models.Models
{
    public abstract class BaseModel
    {
        #region Fields
        private readonly Random _random = new Random();
        private readonly byte[] _randomeBytesLabelTime = {0x01, 0x93, 0xC0, 0x7F, 0x9F, 0xE4};
        #endregion

        #region Methods
        public abstract byte[] CreateTitle(int dataSize);
        public abstract byte[] CreateData(int dataSize);

        public virtual byte[] CreateSynchromarker()
        {
            return new byte[] {0x1A, 0xCF, 0xCF, 0x1D};
        }

        public virtual byte[] ComputeChecksumBytes(byte[] data)
        {
            ushort sum = 0;
            foreach (byte b in data)
                sum += b;
            return new byte[] {(byte) (sum & 255), (byte) ~sum};
        }

        public virtual byte[] CreatelabelTime()
        {
            var listBytes = new List<byte>();
            for (var i = 0; i < 6; i++)
            {
                listBytes.Add(_randomeBytesLabelTime[_random.Next(0, 5)]);
            }
            return listBytes.ToArray();
        }

        public virtual byte[] CreatePaket()
        {
            var dataSize = _random.Next(2, 5);
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
