using System.Reflection;

namespace T4FileGenerator.Models;

public class TemplateGeneratorConfiguration
{
	public TemplateGeneratorConfiguration(Assembly executingAssembly, string targetPath, bool restoreDeletedFiles = false, string generatedFolder = "__generated__", bool overwriteHuman = false)
	{
		ExecutingAssembly = executingAssembly;
		TargetPath = targetPath;
		GeneratedFolder = generatedFolder;
		OverwriteHuman = overwriteHuman;
		RestoreDeletedFiles = restoreDeletedFiles;
	}

	public Assembly ExecutingAssembly { get; set; }
	public string TargetPath { get; set; }
	public string GeneratedFolder { get; set; }
	public bool OverwriteHuman { get; set; } = false;
	public bool RestoreDeletedFiles { get; set; } = false;
}