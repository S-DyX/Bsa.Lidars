using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;
using Bsa.RpLidar.Entities;

namespace Bsa.RpLidar.Helpers
{
    public static class SerialPortExtensions
    {
        public static void SendRequest(this SerialPort serialPort, Command command)
        {
            var commandByte = command.GetByte();

            byte[] bytes = { Constants.SYNC_BYTE, commandByte }; 

            serialPort.Write(bytes, 0, bytes.Length);
        }

        public static void SendCommand(this SerialPort serialPort, byte command, byte[] data = null)
        { 
            byte[] bytes;

            if ((command & 0x80) > 0 && data != null)
            {
               
                byte checksum = 0;
                checksum ^= (byte)0xA5;
                checksum ^= command;
                checksum ^= (byte)(data.Length & 0xFF);

                // calc checksum
                for (int pos = 0; pos < data.Length; pos++)
                {
                    checksum ^= (byte)(data[pos]);
                }
                var temp = new List<byte>()
                {
                    (byte)0xA5,command,(byte)data.Length
                };
                temp.AddRange(data);
                temp.Add(checksum);
                bytes = temp.ToArray();

            }
            else
            {
                bytes = new byte[]
                {
                    Constants.SYNC_BYTE, command
                };
            } 

            serialPort.Write(bytes, 0, bytes.Length);
        }
        public static byte[] Read(this SerialPort serialPort, int size, int timeout)
        { 
            var data = new byte[size];
            var sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < timeout)
            {
                if (serialPort.BytesToRead < size)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    serialPort.Read(data, 0, size); 
                    return data;
                }
            }

            return data;
        }
    }
}
