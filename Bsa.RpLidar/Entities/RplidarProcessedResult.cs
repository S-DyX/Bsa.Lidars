namespace Bsa.RpLidar.Entities
{
	public sealed class RplidarProcessedResult
	{
		public bool IsStartAngleSyncQ6;

		public bool IsRpLidarRespMeasurementSyncBitExp;

		public RplidarResponseUltraCapsuleMeasurementNodes Value;

		public byte[] RemainderData;
	}
}
