namespace Bsa.Velodyne.Entities
{
	/// <summary>
	/// Changing these numbers will destroy old log files.
	/// </summary>
	public enum ReturnType
	{
		NAN = 0,
		Strongest = 1,
		Last = 2,
		Dual = 3,
	}

	/// <summary>
	/// Changing these numbers will destroy old log files.
	/// </summary>
	public enum VelodyneModel
	{
		NAN = 0,
		VLP_16 = 16,
		HDL_32E = 32,
	}
}
