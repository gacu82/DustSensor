using DustSensor.Sensors;
using System;
using System.IO.Ports;
using System.Linq;

namespace DustSensor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Dust Sensor");
            var comPorts = SerialPort.GetPortNames();
            string selectedCom = null;
            while(!comPorts.Contains(selectedCom))
            {
                Console.WriteLine("Select port");
                PrintComPorts(comPorts);
                var selectedPos = Console.ReadLine();
                if (!int.TryParse(selectedPos, out int index)) continue;
                if (index < 1 || index > comPorts.Length) continue;

                selectedCom = comPorts[index-1];
            }

            var com = new SerialPort(selectedCom, 9600, Parity.None, 8, StopBits.One);
            var sensor = new SDS011();

            var engine = new Engine(com, sensor, Console.Out, 25, 50);
            engine.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            engine.Stop();
        }

        static void PrintComPorts(string[] ports)
        {
            var i = 1;
            foreach(var p in ports) Console.WriteLine($"{i++} - {p}");
        }
    }
}
