using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using WpfTemplate.Structural;
using WpfTemplate.Structural.MEF;

namespace WpfTemplate
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly AppCommandLineArgs args;
		private readonly Task initializeMEFTask;
		private readonly Stopwatch startupStopwatch;
		private readonly MEFMagician mefMagician;

		public App(bool readSettings, Stopwatch startupStopwatch)
		{
			args = new AppCommandLineArgs();

			mefMagician = new MEFMagician(GetAssemblies(), Assembly.GetExecutingAssembly().FullName);

			// PERF: Init MEF on a BG thread. Results in slightly faster startup, eg. InitializeComponent() becomes a 'free' call on this UI thread
			initializeMEFTask = Task.Run(() => mefMagician.InitializeMEF(readSettings, useCache: readSettings));
			this.startupStopwatch = startupStopwatch;

		}

		private Assembly[] GetAssemblies()
		{
			var list = new List<Assembly>();
			list.Add(GetType().Assembly);
			
			return list.ToArray();
		}
	}
}
