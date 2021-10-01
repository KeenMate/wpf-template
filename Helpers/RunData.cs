using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using WpfTemplate.Constants;
using WpfTemplate.Extensions;

namespace WpfTemplate.Helpers
{
	public class RunData
	{
		const string SETTINGS_FILENAME = "avayaClient.xml";


		#region CLICK ONCE RELATED PROPERTIES

		// private static ApplicationDeployment AppDeployment;
		// public static bool RunningAsClickOnce { get; private set; }

		#endregion

		public static Dictionary<Window, List<string>> TitleInfos { get; } = new Dictionary<Window, List<string>>();

		public static Assembly CurrentAssembly { get; private set; }

		#region Local instance related properties

		/// <summary>
		/// When running in locally mode all data/log/etc. folders are created next to the .exe file
		/// to isolate it from the installed version respectively from user's app data folder
		/// </summary>
		public static bool RunningLocally { get; set; }
		
		/// <summary>
		/// Name of the instance directory that all data files will be created at
		/// </summary>
		public static string InstanceName { get; set; }
		
		#endregion

		#region Common directories

		/// <summary>
		/// Base directory of binaries
		/// </summary>
		public static string BinDirectory { get; private set; }

		/// <summary>
		/// Base directory of data directory. Usually %APPDATA%\{shortProductName}
		/// </summary>
		public static string DataDirectory { get; private set; }

		/// <summary>
		/// Logs folder for custom logs based in DataDirectory
		/// </summary>
		public static string LogsDirectory { get; private set; }
		public static string TempDirectory { get; private set; }

		#endregion
		
		public static string Version { get; private set; }

		public static string Release { get; private set; }

		public static string AppTitle { get; private set; }
		
		/// <summary>
		/// Safe application title is stripped and shortened to a code like version
		/// </summary>
		public static string SafeAppTitle { get; private set; }

		public static string SettingsFilename { get; private set; }


		static RunData()
		{
			CurrentAssembly = typeof(App).Assembly;
			
			// Detection of instances running in ClickOnce mode
			//try
			//{
			//	AppDeployment = ApplicationDeployment.CurrentDeployment;
			//	RunningAsClickOnce = AppDeployment.IsNotNull();
			//}
			//catch
			//{
			//	RunningAsClickOnce = false;
			//}

			Version = GetVersion();
			Release = GetRelease();
			AppTitle = ThisApp.ProductName + (Release.IsNotEmptyString() ? $" {Release}" : string.Empty);
			SafeAppTitle = AppTitle.Trim(' ').Replace(" ", "_");
		}

		public static void SetAsRunLocally(string instanceName)
		{
			RunningLocally = true;
			InstanceName = instanceName?.ToUpper();

			AppTitle = $"{InstanceName} - {AppTitle}";
			SafeAppTitle += $"-{InstanceName}";
		}


		private static string GetVersion()
		{
			//if (RunningAsClickOnce)
			//{
			//	return AppDeployment.CurrentVersion.ToString();
			//}

			return CalculateAssemblyInformationalVersion(CurrentAssembly);
		}

		private static string GetRelease()
		{
#if DEBUG
			return "Development";
#endif

			//for PROD version
			return "";
		}

		static string CalculateAssemblyInformationalVersion(Assembly asm)
		{
			var attrs = asm.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
			var attr = attrs.Length == 0 ? null : attrs[0] as AssemblyFileVersionAttribute;
			Debug.Assert(!(attr is null));
			if (!(attr is null))
				return attr.Version;
			return asm.GetName().Version.ToString();
		}

		public static void InitializeFolders()
		{
			BinDirectory = Path.GetDirectoryName(CurrentAssembly.Location);
			DataDirectory = GetDataFolder();
			TempDirectory = Path.Combine(BinDirectory, "Temp");
			
			LogsDirectory = Path.Combine(DataDirectory, "Logs");

			EnsureFolders();

			SettingsFilename = Path.Combine(DataDirectory, SETTINGS_FILENAME);
		}

		private static void EnsureFolders()
		{
			Directory.CreateDirectory(DataDirectory);
			Directory.CreateDirectory(LogsDirectory);
			Directory.CreateDirectory(TempDirectory);
		}

		public static string GetDefaultTitle(Window window, string customTitle)
		{
			StringBuilder builder = new StringBuilder(AppTitle);
			if (TitleInfos.Count > 0)
				builder.Append($" {string.Join(", ", TitleInfos[window].ToArray())}");
			builder.Append($" {Version}");

			return customTitle.IsNotEmptyString()
				? $"{customTitle} - "
				: builder.ToString();
		}

		private static void UpdateTitle(Window window, string customTitle = "") => window.Title = GetDefaultTitle(window, customTitle);

		public static void AddTitleInfo(Window window, string info = "")
		{
			if (!TitleInfos.ContainsKey(window))
				TitleInfos.Add(window, new List<string>(5));

			if (TitleInfos[window].Contains(info))
				return;

			if (info.IsNotEmptyString())
				TitleInfos[window].Add(info);

			UpdateTitle(window);
		}

		public static void RemoveTitleInfo(Window window, string info)
		{
			if (!TitleInfos.ContainsKey(window))
				TitleInfos.Add(window, new List<string>(5));

			if (TitleInfos[window].Remove(info))
				UpdateTitle(window);
		}

		public static void SetWindowTitle(Window window)
		{
			//AddTitleInfo(window, IntPtr.Size == 4 ? "32-bit" : "64-bit");

			AddTitleInfo(window);
		}

		private static string GetDataFolder()
		{
			if (RunningLocally)
				return Path.Combine(BinDirectory,
					$"__{InstanceName}");

			//if (RunningAsClickOnce)
			//{
			//	return AppDeployment.DataDirectory;
			//}

			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				$"{ThisApp.ShortProductName}_{GetRelease()}".Trim('_'));
		}

	}
}