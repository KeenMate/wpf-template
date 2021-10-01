using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WpfTemplate.Structural.Interfaces;

namespace WpfTemplate.Structural
{
	sealed class AppCommandLineArgs : IAppCommandLineArgs
	{
		const char ARG_SEP = ':';

		/// <summary>
		/// --run-locally that ensures isolation from user's app data folder
		/// </summary>
		public bool RunningLocally { get; }
		public string InstanceName { get; }
		public string SettingsFilename { get; }
		public string Language { get; }
		public string Culture { get; }
		public bool? FullScreen { get; }
		public bool ShowStartupTime { get; }

		readonly Dictionary<string, string> userArgs = new Dictionary<string, string>();

		public AppCommandLineArgs()
			: this(Environment.GetCommandLineArgs().Skip(1).ToArray())
		{
		}

		public AppCommandLineArgs(string[] args)
		{
			SettingsFilename = null;
			Language = string.Empty;
			Culture = string.Empty;
			FullScreen = null;
			ShowStartupTime = false;

			bool canParseCommands = true;
			for (int i = 0; i < args.Length; i++)
			{
				var arg = args[i];
				var next = i + 1 < args.Length ? args[i + 1] : string.Empty;

				if (canParseCommands && arg.Length > 0 && arg[0] == '-')
				{
					switch (arg)
					{
						case "--":
							canParseCommands = false;
							break;

						case "--run-locally":
							RunningLocally = true;
							InstanceName = next;
							i++;
							break;
						case "--settings-file":
							SettingsFilename = GetFullPath(next);
							i++;
							break;

						case "-l":
						case "--language":
							Language = next;
							i++;
							break;

						case "--culture":
							Culture = next;
							i++;
							break;

						case "--full-screen":
							FullScreen = true;
							break;

						case "--not-full-screen":
							FullScreen = false;
							break;

						case "--show-startup-time":
							ShowStartupTime = true;
							break;

						default:
							int sepIndex = arg.IndexOf(ARG_SEP);
							string argName, argValue;
							if (sepIndex < 0)
							{
								argName = arg;
								argValue = string.Empty;
							}
							else
							{
								argName = arg.Substring(0, sepIndex);
								argValue = arg.Substring(sepIndex + 1);
							}
							if (!userArgs.ContainsKey(argName))
								userArgs.Add(argName, argValue);
							break;
					}
				}
			}
		}

		static string GetFullPath(string file)
		{
			try
			{
				return Path.GetFullPath(file);
			}
			catch
			{
			}
			return file;
		}

		static bool TryParseUInt32(string s, out uint value)
		{
			if (uint.TryParse(s, out value))
				return true;
			if (int.TryParse(s, out var value2))
			{
				value = (uint)value2;
				return true;
			}
			if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase) || s.StartsWith("&H", StringComparison.OrdinalIgnoreCase))
			{
				s = s.Substring(2);
				if (uint.TryParse(s, NumberStyles.HexNumber, null, out value))
					return true;
			}
			return false;
		}

		static bool TryParseUInt64(string s, out ulong value)
		{
			if (ulong.TryParse(s, out value))
				return true;
			if (long.TryParse(s, out var value2))
			{
				value = (ulong)value2;
				return true;
			}
			if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase) || s.StartsWith("&H", StringComparison.OrdinalIgnoreCase))
			{
				s = s.Substring(2);
				if (ulong.TryParse(s, NumberStyles.HexNumber, null, out value))
					return true;
			}
			return false;
		}

		public bool HasArgument(string argName) => userArgs.ContainsKey(argName);

		public string GetArgumentValue(string argName)
		{
			userArgs.TryGetValue(argName, out var value);
			return value;
		}

		public IEnumerable<(string argument, string value)> GetArguments() => userArgs.Select(a => (a.Key, a.Value));
	}
}