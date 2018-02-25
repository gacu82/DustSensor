using System;

namespace DustSensor.Sensors
{
    class SDS011 : ISensor
    {
        private const byte HEADER = 0xAA;
        private const byte TAIL = 0xAB;

        private bool sync = false;
        private byte[] payload = new byte[8];
        private int index;

        public event EventHandler<SensorData> DataCollected;
        public event EventHandler ChecksumError;
        public event EventHandler SyncError;

        public void Feed(byte[] data)
        {
            foreach(var b in data)
            {
                if(!sync)
                {
                    if(b == HEADER)
                    {
                        sync = true;
                        index = 0;
                    }
                }
                else
                {
                    if (index == payload.Length)
                    {
                        sync = false;
                        if (b == TAIL)
                        {
                            var msg = ReadPayload(payload);
                            if (msg.ISChecksumOk())
                            {
                                DataCollected(this, new SensorData(msg.PM25 / 10.0, msg.PM10 / 10.0));
                            }
                            else
                            {
                                ChecksumError(this, null);
                            }
                        }
                        else
                        {
                            SyncError(this, null);
                        }
                    }
                    else
                    {
                        payload[index] = b;
                        index++;
                    }
                }
            }
        }

        private SDS11Message ReadPayload(byte[] data)
        {
            var msg = new SDS11Message();
            msg.Commander = data[0];
            msg.PM25 = BitConverter.ToInt16(data, 1);
            msg.PM10 = BitConverter.ToInt16(data, 3);
            msg.Id = BitConverter.ToInt16(data, 5);
            msg.Checksum = data[7];
            return msg;
        }
    }

    class SDS11Message
    {
        public byte Commander;
        public Int16 PM25;
        public Int16 PM10;
        public Int16 Id;
        public byte Checksum;

        public bool ISChecksumOk()
        {
            return (byte)(PM25.LowerByte() +
                PM25.UpperByte() +
                PM10.LowerByte() +
                PM10.UpperByte() +
                Id.LowerByte() +
                Id.UpperByte()) == Checksum;
        }
    }
}
