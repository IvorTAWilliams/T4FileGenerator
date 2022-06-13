using T4FileGenerator.BaseTemplate;
using T4FileGenerator.Models;

namespace T4FileGenerator.Extensions;

public static class TemplateExtensions
{
	public static OutputFile GenerateOutputFile<T>(this BaseTemplate<T> template, string targetPath)
	{
		var contents = template.TransformText();
		return new OutputFile{Contents = contents, DoGenerate = template.DoGenerate, RelativeFileName = template.RelativeFilePath, FileName = Path.Combine(targetPath, template.RelativeFilePath)};
	}
}