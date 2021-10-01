using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Composition;
using System.Windows.Threading;
using WpfTemplate.Helpers;
using WpfTemplate.Structural.Settings;


namespace WpfTemplate.Structural.MEF
{
	public class MEFMagician
	{
		private readonly Assembly[] assemblies;
		private readonly string exeFilename;
		bool writingCachedMefFile;
		private long resourceManagerTokensOffset;
		readonly ResourceManagerTokenCacheImpl resourceManagerTokenCacheImpl;


		public MEFMagician(Assembly[] assemblies, string exeFilename)
		{
			this.assemblies = assemblies;
			this.exeFilename = exeFilename;
			resourceManagerTokenCacheImpl = new ResourceManagerTokenCacheImpl();
		}

		public ExportProvider InitializeMEF(bool readSettings, bool useCache)
		{
			var resolver = Resolver.DefaultInstance;

			var factory = TryCreateExportProviderFactoryCached(resolver, useCache, out resourceManagerTokensOffset) 
			              ?? CreateExportProviderFactorySlow(resolver);
			var exportProvider = factory.CreateExportProvider();

			exportProvider.GetExportedValue<ServiceLocator>().SetExportProvider(Dispatcher.CurrentDispatcher, exportProvider);
			if (readSettings)
			{
				var settingsService = exportProvider.GetExportedValue<ISettingsService>();
				try
				{
					new XmlSettingsReader(settingsService).Read();
				}
				catch
				{
				}
			}

			return exportProvider;
		}

		static bool IsFileIOException(Exception ex) => ex is IOException || ex is UnauthorizedAccessException || ex is SecurityException;


		IExportProviderFactory? TryCreateExportProviderFactoryCached(Resolver resolver, bool useCache, out long resourceManagerTokensOffset)
		{
			resourceManagerTokensOffset = -1;
			if (!useCache)
				return null;
			try
			{
				return TryCreateExportProviderFactoryCachedCore(resolver, out resourceManagerTokensOffset);
			}
			catch (Exception ex)
			{
				Debug.Fail(ex.ToString());
				return null;
			}
		}

		IExportProviderFactory? TryCreateExportProviderFactoryCachedCore(Resolver resolver, out long resourceManagerTokensOffset)
		{
			Debug2.Assert(assemblies is not null);
			resourceManagerTokensOffset = -1;
			var filename = GetCachedCompositionConfigurationFilename();
			if (!File.Exists(filename))
				return null;

			Stream? cachedStream = null;
			try
			{
				try
				{
					cachedStream = File.OpenRead(filename);
					if (!new CachedMefInfo(assemblies.ToArray(), cachedStream)
						.CheckFile(resourceManagerTokenCacheImpl, out resourceManagerTokensOffset))
						return null;
				}
				catch (Exception ex) when (IsFileIOException(ex))
				{
					return null;
				}
				try
				{
					return new CachedComposition().LoadExportProviderFactoryAsync(cachedStream, resolver).Result;
				}
				catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && IsFileIOException(ex.InnerExceptions[0]))
				{
					return null;
				}
			}
			finally
			{
				cachedStream?.Dispose();
			}

			
		}

		IExportProviderFactory CreateExportProviderFactorySlow(Resolver resolver)
		{
			var discovery = new AttributedPartDiscoveryV1(resolver);
			var parts = discovery.CreatePartsAsync(assemblies).Result;
			Debug.Assert(parts.ThrowOnErrors() == parts);

			var catalog = ComposableCatalog.Create(resolver).AddParts(parts);
			var config = CompositionConfiguration.Create(catalog);
			// If this fails/throws, one of the following is probably true:
			//	- you didn't build all projects or all files aren't in the same output dir
			//	- netcoreapp: dnSpy isn't the startup project (eg. dnSpy-x86 is)
			Debug.Assert(config.ThrowOnErrors() == config);

			writingCachedMefFile = true;
			Task.Run(() => SaveMefStateAsync(config)).ContinueWith(t => {
				var ex = t.Exception;
				Debug2.Assert(ex is null);
				writingCachedMefFile = false;
			}, CancellationToken.None);

			return config.CreateExportProviderFactory();
		}



		async Task SaveMefStateAsync(CompositionConfiguration config)
		{
			Debug2.Assert(assemblies is not null);
			string filename = GetCachedCompositionConfigurationFilename();
			bool fileCreated = false;
			bool deleteFile = true;
			try
			{
				using (var cachedStream = File.Create(filename))
				{
					fileCreated = true;
					long resourceManagerTokensOffsetTmp;
					new CachedMefInfo(assemblies, cachedStream).WriteFile(resourceManagerTokenCacheImpl.GetTokens(assemblies), out resourceManagerTokensOffsetTmp);
					await new CachedComposition().SaveAsync(config, cachedStream);
					resourceManagerTokensOffset = resourceManagerTokensOffsetTmp;
					deleteFile = false;
				}
			}
			catch (IOException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
			catch (SecurityException)
			{
			}
			finally
			{
				if (fileCreated && deleteFile)
				{
					try
					{
						File.Delete(filename);
					}
					catch { }
				}
			}
		}

		void ResourceManagerTokenCacheImpl_TokensUpdated(object? sender, EventArgs e) => OnTokensUpdated();

		void OnTokensUpdated()
		{
			if (writingCachedMefFile)
				Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(OnTokensUpdated));
			else
				UpdateResourceManagerTokens();
		}

		void UpdateResourceManagerTokens()
		{
			Debug2.Assert(assemblies is not null);
			var tokensOffset = resourceManagerTokensOffset;
			if (tokensOffset < 0)
				return;
			string filename = GetCachedCompositionConfigurationFilename();
			if (!File.Exists(filename))
				return;
			bool deleteFile = true;
			try
			{
				using (var cachedStream = File.OpenWrite(filename))
				{
					new CachedMefInfo(assemblies, cachedStream).UpdateResourceManagerTokens(tokensOffset, resourceManagerTokenCacheImpl);
					deleteFile = false;
				}
			}
			catch (IOException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
			catch (SecurityException)
			{
			}
			if (deleteFile)
			{
				try
				{
					File.Delete(filename);
				}
				catch { }
			}
		}


		string GetCachedCompositionConfigurationFilename()
		{
			var profileDir = BGJitUtils.GetFolder();
			return Path.Combine(profileDir, $"{exeFilename}-mef-info.bin");
		}
	}
}