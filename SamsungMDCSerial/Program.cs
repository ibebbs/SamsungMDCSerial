using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SamsungMDCSerial
{
    class Program
    {
        private static byte CalculateChecksum(byte[] message)
        {
            IEnumerable<byte> source = (message[0] == 0xAA) ? message.Skip(1) : message;

            var value = source.Sum(v => v).ToString("X2");

            var sum = Convert.ToByte(value.Substring(value.Length - 2, 2), 16);

            return sum;
        }

        private static byte[] CreateMessage(byte id, byte command, params byte[] data)
        {
            byte[] message = new byte[5 + data.Length];
            message[0] = 0xAA;
            message[1] = (byte)command;
            message[2] = id;
            message[3] = (byte)data.Length;

            if (data.Length > 0)
            {
                Array.Copy(data, 0, message, 4, data.Length);
            }

            message[4 + data.Length] = CalculateChecksum(message);

            return message;
        }

        private static System.IO.Ports.SerialPort OpenPort(string comPort)
        {
            var port = new System.IO.Ports.SerialPort(comPort, 9800, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            port.Open();

            return port;
        }

        private static int TurnOn(On opts)
        {
            using (var port = OpenPort(opts.ComPort))
            {
                var message = CreateMessage(opts.DisplayId, Command.PowerControl, Power.On);
                port.Write(message, 0, message.Length);

                var response = new byte[1024];
                int read = port.Read(response, 0, 1024);

                return 0;
            }
        }

        private static int TurnOff(Off opts)
        {
            using (var port = OpenPort(opts.ComPort))
            {
                var message = CreateMessage(opts.DisplayId, Command.PowerControl, Power.Off);
                port.Write(message, 0, message.Length);

                var response = new byte[1024];
                int read = port.Read(response, 0, 1024);

                return 0;
            }
        }

        static int Main(string[] args)
        {
            return Parser.Default
                .ParseArguments<On, Off>(args)
                .MapResult(
                    (On opts) => TurnOn(opts),
                    (Off opts) => TurnOff(opts),
                    errs => 1);
        }
    }
}
