using T4FileGenerator.BaseTemplate;
using T4FileGenerator.Extensions;
using T4FileGenerator.Models;

namespace T4FileGenerator.Services;

public class TemplateGenerator
{
	private readonly TemplateGeneratorConfiguration _configuration;
	public FileManager FileManager;
	public List<OutputFile> OutputFiles;

	public TemplateGenerator(TemplateGeneratorConfiguration configuration)
	{
		_configuration = configuration;
		var fileManagerConfiguration = new FileManagerConfiguration
		{
			OverwriteHuman = _configuration.OverwriteHuman,
			RestoreDeletedFiles = _configuration.RestoreDeletedFiles,
		};
		FileManager = new FileManager(fileManagerConfiguration);
		FileManager.HumanFiles = FileManager.LoadFiles(Path.Combine(configuration.RootDir, configuration.RelativeOutputFolder));
		FileManager.OriginalFiles = FileManager.LoadFiles(Path.Combine(_configuration.RootDir, _configuration.RelativeGeneratedFolder, _configuration.RelativeOutputFolder));
		OutputFiles = new List<OutputFile>();
	}

	public void Generate<T>(T model)
	{
		OutputFiles.AddRange(Task.WhenAll(GenerateInternal(model)).Result.Where(x => x.DoGenerate != false).ToList());
	}

	private IEnumerable<Task<OutputFile>> GenerateInternal<T>(T model)
	{
		foreach (var template in TemplateFinder.GetTemplates<BaseTemplate<T>>(_configuration.ExecutingAssembly))
		{
			yield return Task.Run(() => template.InjectModel(model).GenerateOutputFile(Path.Combine(_configuration.RootDir, _configuration.RelativeOutputFolder)));
		}
	}
	
	public void Generate<T>(IEnumerable<T> models)
	{
		OutputFiles.AddRange(Task.WhenAll(GenerateInternal(models)).Result.Where(x => x.DoGenerate != false ).ToList());
	}

	private IEnumerable<Task<OutputFile>> GenerateInternal<T>(IEnumerable<T> models)
	{
		return from model in models
			from template in TemplateFinder.GetTemplates<BaseTemplate<T>>(_configuration.ExecutingAssembly)
			select Task.Run(() => template.InjectModel(model).GenerateOutputFile(Path.Combine(_configuration.RootDir, _configuration.RelativeOutputFolder)));
	}
	
	public void WriteOutputFiles(List<string>? cleaningExclude = null)
	{
		var generatedPath = Path.Combine(_configuration.RootDir, _configuration.RelativeGeneratedFolder, _configuration.RelativeOutputFolder);
		FileManager.GeneratedFiles = OutputFiles;
		FileManager.OutputFiles = FileManager.ComputeOutputFiles();
		FileManager.WriteFiles(FileManager.GeneratedFiles, generatedPath);
		FileManager.WriteFiles(FileManager.OutputFiles, Path.Combine(_configuration.RootDir, _configuration.RelativeOutputFolder));
		FileManager.CleanOldGeneratedFiles(generatedPath);
	}
}