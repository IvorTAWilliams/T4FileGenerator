using System.Reflection;

namespace T4FileGenerator.Models;

public class TemplateGeneratorConfiguration
{
	public TemplateGeneratorConfiguration(Assembly executingAssembly, string rootDir, string relativeOutputFolder, bool restoreDeletedFiles = false, string relativeGeneratedFolder = "__generated__", bool overwriteHuman = false)
	{
		ExecutingAssembly = executingAssembly;
		RootDir = rootDir;
		RelativeOutputFolder = relativeOutputFolder;
		RelativeGeneratedFolder = relativeGeneratedFolder;
		OverwriteHuman = overwriteHuman;
		RestoreDeletedFiles = restoreDeletedFiles;
	}

	public Assembly ExecutingAssembly { get; set; }
	public string RootDir { get; }
	public string RelativeOutputFolder { get; }
	public string RelativeGeneratedFolder { get; set; }
	public bool OverwriteHuman { get; set; } = false;
	public bool RestoreDeletedFiles { get; set; } = false;
}