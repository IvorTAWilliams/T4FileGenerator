using T4FileGenerator.Helpers;
using T4FileGenerator.Models;

namespace T4FileGenerator.Services
{
	public class FileManager
	{
		private readonly FileManagerConfiguration _configuration;
		public List<OutputFile> OriginalFiles { get; set; }
		public List<OutputFile> HumanFiles { get; set; }
		public List<OutputFile> GeneratedFiles { get; set; }
		public List<OutputFile> OutputFiles { get; set; }

		private List<string> _excludedDirs = new()
		{
			"node_modules",
			@"\\obj\\",
			@"\\.git\\",
			"model.yaml",
			".db"
		};

		public FileManager(FileManagerConfiguration configuration)
		{
			_configuration = configuration;
		}

		public List<OutputFile> LoadFiles(string path = null, IEnumerable<string> excludedDirs = null)
		{
			var excludeDirs = excludedDirs != null ? excludedDirs.Concat(_excludedDirs).ToList() : _excludedDirs;
			var directoryWalker = new DirectoryWalker(excludeDirs);
			var fileNames = directoryWalker.WalkDownDirectory(path);
			return fileNames
				.Select(fileName => new OutputFile
				{
					Contents = File.ReadAllText(fileName),
					FileName = fileName,
					RelativeFileName = Path.GetRelativePath(path, fileName)
				})
				.ToList();
		}

		private bool ComparePaths(string path1, string path2)
		{
			if (path1 == null || path2 == null)
			{
				return false;
			}

			return StringHelpers.NormalizePath(path1) ==
			       StringHelpers.NormalizePath(path2);
		}

		public List<OutputFile> ComputeOutputFiles()
		{
			var outputFiles = new List<OutputFile>();
			var outputFilesTasks = new List<Task<OutputFile>>();
			foreach (var generatedFile in GeneratedFiles)
			{
				var humanFile =
					HumanFiles.FirstOrDefault(x => ComparePaths(x.RelativeFileName, generatedFile.RelativeFileName));
				var originalFile = OriginalFiles.FirstOrDefault(x => ComparePaths(x.RelativeFileName, generatedFile.RelativeFileName));
				// if overwrite human, then overwrite human
				if (_configuration.OverwriteHuman)
				{
					outputFiles.Add(generatedFile);
				}
				// if the file exists as original, human and generated, do a patch
				else if (humanFile != null && originalFile != null)
				{
					outputFilesTasks.Add(FileMerger.Merge(originalFile, humanFile, generatedFile));
				}
				else if (humanFile == null && originalFile != null && _configuration.RestoreDeletedFiles)
				{
					outputFiles.Add(generatedFile);
				}
				// if it's a brand new file
				else if (humanFile == null)
				{
					outputFiles.Add(generatedFile);
				}
			}

			outputFiles.AddRange(Task.WhenAll(outputFilesTasks).Result);
			return outputFiles;
		}

		public void WriteFiles(List<OutputFile> outputFiles, string subPath = null)
		{
			foreach (var outputFile in outputFiles)
			{
				var outputPath = subPath != null
					? Path.Combine(subPath, outputFile.RelativeFileName)
					: Path.Combine(outputFile.RelativeFileName);
				var fileInfo = new FileInfo(outputPath);

				if (fileInfo.Directory != null && !fileInfo.Exists)
				{
					Directory.CreateDirectory(fileInfo.Directory.FullName);
				}

				File.WriteAllText(outputPath, outputFile.Contents);
			}
		}

		public void CleanOldGeneratedFiles(string path, List<string>? cleaningExclude = null)
		{
			foreach (var originalFile in OriginalFiles)
			{
				// if it existed as an original file but not a newly generated one
				var generatedFile = GeneratedFiles.FirstOrDefault(x => ComparePaths(x.RelativeFileName, originalFile.RelativeFileName));
				if (generatedFile == null && cleaningExclude is not null && !cleaningExclude.Contains(originalFile.FileName))
				{
					File.Delete(originalFile.FileName);
					File.Delete(Path.Combine(path, originalFile.RelativeFileName));
				}
			}
		}
	}
}