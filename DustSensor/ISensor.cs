using System;

namespace DustSensor
{
    interface ISensor
    {
        void Feed(byte[] data);

        event EventHandler<SensorData> DataCollected;
        event EventHandler ChecksumError;
        event EventHandler SyncError;
    }

    class SensorData
    {
        public SensorData(double pm25, double pm10)
        {
            PM25 = pm25;
            PM10 = pm10;
        }

        public double PM25 { get; }
        public double PM10 { get; }
    }
}
