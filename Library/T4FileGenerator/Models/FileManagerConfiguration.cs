﻿namespace T4FileGenerator.Models;

public class FileManagerConfiguration
{
	public string TargetPath { get; set; }
	public bool OverwriteHuman { get; set; }
	public bool RestoreDeletedFiles { get; set; } = false;
}