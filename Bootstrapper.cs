using System;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfTemplate.Helpers;
using WpfTemplate.Resources;

namespace WpfTemplate
{
	public class Bootstrapper
	{
		[STAThread]
		public static void Main()
		{
			var sw = Stopwatch.StartNew();

			// Use multicore JIT.
			// Simple test: x86: ~18% faster startup, x64: ~12% faster startup.
			try
			{
				var profileDir = BGJitUtils.GetFolder();
				Directory.CreateDirectory(profileDir);
				ProfileOptimization.SetProfileRoot(profileDir);
				ProfileOptimization.StartProfile("startup.profile");
			}
			catch
			{
			}
			
			bool readSettings = (Keyboard.Modifiers & ModifierKeys.Shift) == 0;
			if (!readSettings)
				readSettings = AskReadSettings();

			new App(readSettings, sw).Run();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static bool AskReadSettings()
		{
			bool readSettings;
			// Need to use DefaultDesktopOnly or the dlg box is shown in the background...
			var res = MessageBox.Show(Texts.Prompt_ReadSettings, RunData.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
			readSettings = res != MessageBoxResult.No;
			return readSettings;
		}
	}
}