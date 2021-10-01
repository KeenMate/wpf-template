using System.Reflection;

namespace WpfTemplate.Structural.MEF
{
	/// <summary>
	/// Caches the info so we don't have to call <see cref="Module.GetTypes"/> which is very slow
	/// </summary>
	abstract class ResourceManagerTokenCache
	{
		public abstract bool TryGetResourceManagerGetMethodMetadataToken(Assembly assembly, out int getMethodMetadataToken);
		public abstract void SetResourceManagerGetMethodMetadataToken(Assembly assembly, int getMethodMetadataToken);
	}
}