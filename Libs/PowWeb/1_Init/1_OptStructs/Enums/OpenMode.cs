namespace PowWeb._1_Init._1_OptStructs.Enums;

/// <summary>
/// Defines how we want to open the browser
/// </summary>
public enum OpenMode
{
	/// <summary>
	/// Runs a new instance. If an existing instance is detected, it will be closed first
	/// </summary>
	Create,

	/// <summary>
	/// Connects to an existing instance. If no instance is found, it will throw an InvalidOperationException
	/// </summary>
	Connect,

	/// <summary>
	/// Tries to connect to an existing instance first. If no instance is found, create one
	/// </summary>
	ConnectOrCreate
}