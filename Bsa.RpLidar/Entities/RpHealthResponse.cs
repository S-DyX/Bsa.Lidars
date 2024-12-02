namespace Bsa.RpLidar.Entities
{
	public class RpHealthResponse : IDataResponse
	{
		public RpDataType Type  => RpDataType.GetHealth;

		/// <summary>
		/// Status Code
		/// 0x0 = OK, 0x1 = Warning, 0x2 = Error
		/// </summary>
		public int Status { get; set; }

		/// <summary>
		/// Error Code
		/// </summary>
		public int ErrorCode { get; set; }
	}
}
