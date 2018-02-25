using System;
using System.IO;
using System.IO.Ports;
using System.Linq;

namespace DustSensor
{
    class Engine
    {
        private readonly SerialPort _serialPort;
        private readonly ISensor _sensor;
        private readonly TextWriter _logWriter;
        private readonly double _pm25Limit;
        private readonly double _pm10Limit;

        public Engine(SerialPort serialPort, 
            ISensor sensor, TextWriter logWriter, double pm25Limit, double pm10Limit)
        {
            _serialPort = serialPort;
            _sensor = sensor;
            _logWriter = logWriter;
            _pm25Limit = pm25Limit;
            _pm10Limit = pm10Limit;
            _sensor.DataCollected += _sensorDataCollected;
            _sensor.ChecksumError += _sensorChecksumError;
            _sensor.SyncError += _sensorSyncError;
        }

        private void _sensorSyncError(object sender, EventArgs e)
        {
            Console.WriteLine("Sync error");
        }

        private void _sensorChecksumError(object sender, EventArgs e)
        {
            _logWriter.WriteLine("Checksum error");
        }

        private void _sensorDataCollected(object sender, SensorData e)
        {
            var pm25Level = Math.Ceiling(e.PM25 / _pm25Limit * 100);
            var pm10Level = Math.Ceiling(e.PM10 / _pm10Limit * 100);
            _logWriter.WriteLine($"{DateTime.Now} PM2.5: {e.PM25}µg/m3 ({pm25Level}%), PM10: {e.PM10}µg/m3 ({pm10Level}%)");
        }

        private void _serialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = sender as SerialPort;
            var buffer = new byte[serialPort.BytesToRead];
            var countRead = serialPort.Read(buffer, 0, buffer.Length);
            _sensor.Feed(buffer.Take(countRead).ToArray());
        }

        public void Start()
        {
            _serialPort.DataReceived += _serialPortDataReceived;
            if (!_serialPort.IsOpen) _serialPort.Open();
        }

        public void Stop()
        {
            if (_serialPort.IsOpen) _serialPort.Close();
        }
    }
}
