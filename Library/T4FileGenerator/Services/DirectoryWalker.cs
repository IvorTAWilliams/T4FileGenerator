using System.Text.RegularExpressions;
using static System.IO.File;

namespace T4FileGenerator.Services
{
	public class DirectoryWalker
	{
		private readonly List<string> _excludeDirs;

		public DirectoryWalker(List<string> excludeDirs)
		{
			_excludeDirs = excludeDirs;
		}
		
		public List<string> WalkDownDirectory(string dir)
		{
			var filePaths = new List<string>();
			if (!Directory.Exists(dir))
			{
				return new List<string>();
			}
			var validDirItems = Directory
				.GetFileSystemEntries(dir)
				.Where(x => !_excludeDirs.Exists(e => Regex.IsMatch(x, e, RegexOptions.IgnoreCase)));
			
			foreach (var item in validDirItems)
			{
				if (GetAttributes(item).HasFlag(FileAttributes.Directory))
				{
					filePaths.AddRange(WalkDownDirectory(item));
				}
				else
				{
					filePaths.Add(item);
				}
			}
			return filePaths;
		}
	}
}