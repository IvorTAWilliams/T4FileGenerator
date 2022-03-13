using T4FileGenerator.BaseTemplate;
using T4FileGenerator.Extensions;
using T4FileGenerator.Models;

namespace T4FileGenerator.Services;

public class TemplateGenerator
{
	private readonly TemplateGeneratorConfiguration _configuration;
	private readonly FileManager _fileManager;
	public List<OutputFile> OutputFiles;

	public TemplateGenerator(TemplateGeneratorConfiguration configuration)
	{
		_configuration = configuration;
		var fileManagerConfiguration = new FileManagerConfiguration
		{
			TargetPath = _configuration.TargetPath,
			OverwriteHuman = _configuration.OverwriteHuman
		};
		_fileManager = new FileManager(fileManagerConfiguration);
		_fileManager.HumanFiles = _fileManager.LoadFiles(excludedDirs: new[] {_configuration.GeneratedFolder});
		_fileManager.OriginalFiles = _fileManager.LoadFiles(subPath: _configuration.GeneratedFolder);
		OutputFiles = new List<OutputFile>();
	}

	public void Generate<T>(T model)
	{
		OutputFiles.AddRange(Task.WhenAll(GenerateInternal(model)).Result.ToList());
	}

	private IEnumerable<Task<OutputFile>> GenerateInternal<T>(T model)
	{
		foreach (var template in TemplateFinder.GetTemplates<BaseTemplate<T>>(_configuration.ExecutingAssembly))
		{
			yield return Task.Run(() => template.InjectModel(model).GenerateOutputFile(_configuration.TargetPath));
		}
	}
	
	public void Generate<T>(IEnumerable<T> models)
	{
		OutputFiles.AddRange(Task.WhenAll(GenerateInternal(models)).Result.ToList());
	}

	private IEnumerable<Task<OutputFile>> GenerateInternal<T>(IEnumerable<T> models)
	{
		return from model in models
			from template in TemplateFinder.GetTemplates<BaseTemplate<T>>(_configuration.ExecutingAssembly)
			select Task.Run(() => template.InjectModel(model).GenerateOutputFile(_configuration.TargetPath));
	}
	
	public void WriteOutputFiles()
	{
		_fileManager.GeneratedFiles = OutputFiles;
		_fileManager.OutputFiles = _fileManager.ComputeOutputFiles();
		_fileManager.WriteFiles(_fileManager.GeneratedFiles, _configuration.GeneratedFolder);
		_fileManager.WriteFiles(_fileManager.OutputFiles);
		_fileManager.CleanOldGeneratedFiles();
	}
}