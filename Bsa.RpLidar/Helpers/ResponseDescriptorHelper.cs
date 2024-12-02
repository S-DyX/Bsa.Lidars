using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bsa.RpLidar.Entities;

namespace Bsa.RpLidar.Helpers
{
    public static class ResponseDescriptorHelper
    {
        private const int DataResponseLengthMask = 0x3FFFFFFF;
        private const int SendModeShift = 30;

        public static ResponseDescriptor ToResponseDescriptor(this byte[] data)
        {
            if (data.Length < Constants.DescriptorLength)
                throw new InvalidDataException("RESULT_INVALID_ANS_TYPE");
             
            if (!IsValid(data[0], data[1]))
            {
                throw new InvalidDataException("RESULT_INVALID_ANS_TYPE");
            }
             
            var lenAndMode = BitConverter.ToUInt32(data, 2);
            var len = lenAndMode & DataResponseLengthMask;
            var sendMode = (SendMode)(lenAndMode >> SendModeShift);

            var result = new ResponseDescriptor
            {
	            ResponseLength = (int) len, 
	            SendMode = sendMode, 
	            RpDataType = (RpDataType) data[6]
            };

            return result;
        }
        
        public static bool IsValid(byte startFlag1, byte startFlag2)
        {
            if (startFlag1 != Constants.StartFlag1 ||
                startFlag2 != Constants.StartFlag2)
            {
                return false;
            }
            return true;
        }
    }
}
