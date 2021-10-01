using System.Collections.Generic;

namespace WpfTemplate.Structural.Interfaces
{
	/// <summary>
	/// Application command line arguments
	/// </summary>
	public interface IAppCommandLineArgs
	{
		/// <summary>Settings filename</summary>
		string SettingsFilename { get; }

		/// <summary>Language, either human readable or a language guid
		/// (<see cref="IDecompiler.GenericGuid"/> or <see cref="IDecompiler.UniqueGuid"/>)</summary>
		string Language { get; }

		/// <summary>Culture</summary>
		string Culture { get; }

		/// <summary>Full screen</summary>
		bool? FullScreen { get; }

		/// <summary>Show start up time</summary>
		bool ShowStartupTime { get; }


		/// <summary>
		/// Returns true if the argument is present
		/// </summary>
		/// <param name="argName">Argument name, eg. <c>--my-arg</c></param>
		/// <returns></returns>
		bool HasArgument(string argName);

		/// <summary>
		/// Gets the argument value or null if the argument isn't present
		/// </summary>
		/// <param name="argName">Argument name, eg. <c>--my-arg</c></param>
		/// <returns></returns>
		string GetArgumentValue(string argName);

		/// <summary>
		/// Gets all user arguments and values
		/// </summary>
		/// <returns></returns>
		IEnumerable<(string argument, string value)> GetArguments();
	}
}