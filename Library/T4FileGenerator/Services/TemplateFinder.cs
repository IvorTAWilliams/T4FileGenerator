using System.Reflection;

namespace T4FileGenerator.Services;

public class TemplateFinder
{
	public static IEnumerable<T> GetTemplates<T>(Assembly executingAssembly)
	{
		return executingAssembly
			.GetTypes()
			.Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface)
			.Where(type => !type.IsAbstract)
			.Select(type => (T)Activator.CreateInstance(type)!).Where(instance => instance != null);
	}
}