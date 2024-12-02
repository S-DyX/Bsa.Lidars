﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bsa.RpLidar.Entities;

namespace Bsa.RpLidar.Helpers
{
	public static class DataResponseHelper
	{
		public static int RpLidarRespMeasurementSyncBit = (0x1 << 0);
		public static int RpLidarRespMeasurementSyncBitExp = (0x1 << 15);


		public static Queue<RplidarProcessedResult> WaitUltraCapsuledNode(
			this byte[] data)
		{
			var queue = new Queue<RplidarProcessedResult>();
			if (data.Length < 132)
				return queue;
			var size = 132;
			var nodeBuffer = new byte[size];

			var pos = 0;
			if (data[0] == 0xA5 && data[1] == 0x5A)
			{
				pos = 7;
			}

			int recvPos = 0;
			var lastFoundPos = 0;
			for (; pos < data.Length; ++pos)
			{
				var currentByte = data[pos];
				switch (recvPos)
				{
					case 0: // expect the sync bit 1
						{
							var tmp = (currentByte >> 4);
							if (tmp == 0xA)
							{
								// pass
							}
							else
							{
								queue.Enqueue(new RplidarProcessedResult()
								{
									IsStartAngleSyncQ6 = false,
									Value = new RplidarResponseUltraCapsuleMeasurementNodes(),
									IsRpLidarRespMeasurementSyncBitExp = false
								});
								continue;
							}
						}
						break;
					case 1: // expect the sync bit 2
						{
							var tmp = (currentByte >> 4);
							if (tmp == 0x5)
							{
								// pass
							}
							else
							{

								queue.Enqueue(new RplidarProcessedResult()
								{
									IsStartAngleSyncQ6 = false,
									Value = new RplidarResponseUltraCapsuleMeasurementNodes(),
									IsRpLidarRespMeasurementSyncBitExp = false
								});
								recvPos = 0;
								continue;
							}
						}
						break;
				}

				nodeBuffer[recvPos] = currentByte;
				recvPos++;
				if (recvPos == size)
				{
					lastFoundPos = pos;
					recvPos = 0;

					var recvChecksum = (((nodeBuffer[1] & 0xF) << 4) | (nodeBuffer[0] & 0xF));

					byte checksum = 0;
					for (var cpos = 2; cpos < size; cpos++)
					{
						checksum ^= (byte)nodeBuffer[cpos];
					}

					if (recvChecksum == checksum)
					{
						var result = new RplidarResponseUltraCapsuleMeasurementNodes()
						{
						};
						result.s_checksum_1 = nodeBuffer[0];
						result.s_checksum_2 = nodeBuffer[1];
						var cabin = nodeBuffer.Skip(4).ToArray().FromBytes<uint>();

						var start_angle_sync_q6 = BitConverter.ToUInt16(nodeBuffer, 2);
						result.ultra_cabins = cabin.ToArray();

						result.start_angle_sync_q6 = (ushort)start_angle_sync_q6;
						// only consider vaild if the checksum matches...

						if ((start_angle_sync_q6 & DataResponseHelper.RpLidarRespMeasurementSyncBitExp) > 0)
						{
							queue.Enqueue(new RplidarProcessedResult()
							{
								IsStartAngleSyncQ6 = true,
								Value = result,
								IsRpLidarRespMeasurementSyncBitExp = false
							});
						}
						else
						{
							queue.Enqueue(new RplidarProcessedResult()
							{
								IsStartAngleSyncQ6 = true,
								Value = result,
								IsRpLidarRespMeasurementSyncBitExp = true
							});
						}

						continue;
					}

					//_is_previous_capsuledataRdy = false;
					queue.Enqueue(new RplidarProcessedResult()
					{
						IsStartAngleSyncQ6 = false,
						Value = new RplidarResponseUltraCapsuleMeasurementNodes(),
						IsRpLidarRespMeasurementSyncBitExp = false
					});
					//return RESULT_INVALID_DATA;
				}

			}

			var last = new RplidarProcessedResult()
			{
				IsStartAngleSyncQ6 = false,
				Value = new RplidarResponseUltraCapsuleMeasurementNodes(),
				IsRpLidarRespMeasurementSyncBitExp = false
			};
			if (lastFoundPos < pos - 1)
			{
				var foundPos = lastFoundPos + 1;
				last.RemainderData = data.Skip(foundPos).Take(pos - foundPos).ToArray();
			}

			queue.Enqueue(last);
			return queue;
		}

		

		public static int RPLIDAR_RESP_MEASUREMENT_ANGLE_SHIFT = 1;
		public static int RPLIDAR_VARBITSCALE_X2_SRC_BIT = 9;
		public static int RPLIDAR_VARBITSCALE_X4_SRC_BIT = 11;
		public static int RPLIDAR_VARBITSCALE_X8_SRC_BIT = 12;
		public static int RPLIDAR_VARBITSCALE_X16_SRC_BIT = 14;
		public static int RPLIDAR_VARBITSCALE_X2_DEST_VAL = 512;
		public static int RPLIDAR_VARBITSCALE_X4_DEST_VAL = 1280;
		public static int RPLIDAR_VARBITSCALE_X8_DEST_VAL = 1792;
		public static int RPLIDAR_VARBITSCALE_X16_DEST_VAL = 3328;
		public static int RPLIDAR_RESP_MEASUREMENT_QUALITY_SHIFT = 2;

		static int[] VBS_SCALED_BASE = {
			RPLIDAR_VARBITSCALE_X16_DEST_VAL,
			RPLIDAR_VARBITSCALE_X8_DEST_VAL,
			RPLIDAR_VARBITSCALE_X4_DEST_VAL,
			RPLIDAR_VARBITSCALE_X2_DEST_VAL,
			0,
		};

		static int[] VBS_SCALED_LVL = {
			4,
			3,
			2,
			1,
			0,
		};
		static uint[] VBS_TARGET_BASE =
		{
			((uint) 0x1 <<  RPLIDAR_VARBITSCALE_X16_SRC_BIT),
			((uint) 0x1 <<  RPLIDAR_VARBITSCALE_X8_SRC_BIT),
			((uint) 0x1 <<  RPLIDAR_VARBITSCALE_X4_SRC_BIT),
			((uint) 0x1 <<  RPLIDAR_VARBITSCALE_X2_SRC_BIT),
			(uint) 0
		};
		public static uint _varbitscale_decode(int scaled, out int scaleLevel)
		{
			scaleLevel = 0;
			for (var i = 0; i < VBS_TARGET_BASE.Length; i++)
			{
				int remain = (scaled - VBS_SCALED_BASE[i]);
				if (remain >= 0)
				{
					scaleLevel = VBS_SCALED_LVL[i];
					var sc = (remain << scaleLevel);
					var varbitscaleDecode = (uint)(VBS_TARGET_BASE[i] + sc);
					return varbitscaleDecode;
				}
			}
			return 0;
		}


	
		public static IDataResponse ToDataResponse(RpDataType rpDataType, byte[] dataResponseBytes)
		{
			switch (rpDataType)
			{
				case RpDataType.GetHealth:
					return ToHealthDataResponse(dataResponseBytes);

				case RpDataType.GetInfo:
					return ToInfoDataResponse(dataResponseBytes);
			}
			return null;
		}

		public static InfoDataResponse ToInfoDataResponse(byte[] data)
		{
			InfoDataResponse dataResponse = new InfoDataResponse();
			//Mode ID
			var model = data[0];
			dataResponse.ModelId = model.ToString();
			// Firmware version number, the minor value part, decimal
			var firmwareVersionMinor = data[1];
			// Firmware version number, the major value part, integer
			var firmwareVersionMajor = data[2];
			dataResponse.FirmwareVersion = firmwareVersionMajor + "." + firmwareVersionMinor;
			//Hardware version number
			var hardwareVersion = data[3];
			dataResponse.HardwareVersion = hardwareVersion.ToString();
			// 128bit unique serial number 
			byte[] serialNumber = new byte[16];
			for (int i = 4; i < 20; i++)
			{
				serialNumber[i - 4] = data[i];
			}
			string serial = BitConverter.ToString(serialNumber).Replace("-", "");
			dataResponse.SerialNumber = serial;

			return dataResponse;
		}
		public static RpHealthResponse ToHealthDataResponse(byte[] data)
		{
			RpHealthResponse response = new RpHealthResponse();
			response.Status = data[0];
			response.ErrorCode = BitConverter.ToUInt16(data, 1);
			return response;
		}

	}
}