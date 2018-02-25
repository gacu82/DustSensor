using System;

namespace DustSensor
{
    static class Extensions
    {
        public static byte LowerByte(this Int16 value)
        {
            return (byte)(value & 0xff);
        }

        public static byte UpperByte(this Int16 value)
        {
            return (byte)(value >> 8);
        }
    }
}
