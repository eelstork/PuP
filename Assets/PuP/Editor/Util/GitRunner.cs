using System;
using UnityEngine;
using System.Diagnostics;

// Adapted from: https://stewmcc.com/post/git-commands-from-unity/
namespace Activ.PuP{
public static class GitRunner{

	static readonly string[] bogusErrorHints = {
		"create a merge request",
		"Cloning into"
	};

	public static string Cmd(string workdir, string gitCommand){

		string output = "no-git";
		string errorOutput = "no-git";

		if(workdir != null && workdir.Length > 0){
			gitCommand = $"-C {workdir} {gitCommand}";
		}
		var processInfo = new ProcessStartInfo("git", gitCommand){
			CreateNoWindow = true,
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true
		};
		Process process = new Process { StartInfo = processInfo };
		try{
			process.Start();
		}catch(Exception e){
			// For now just assume its failed cause it can't find git.
			UnityEngine.Debug.LogError("Git may not be setup correctly");
			throw e;
		}
		// Read the results back from the process so we can get the output
		// and check for errors
		output = process.StandardOutput.ReadToEnd();
		errorOutput = process.StandardError.ReadToEnd();
		process.WaitForExit();
		process.Close();
		// Check for failure due to no git setup in the project itself
		// or other fatal errors from git.
		if (output.Contains("fatal") || output == "no-git") {
			throw new Exception(
			    $"Command: git {gitCommand} Failed\n"
				                                  + output + errorOutput);
		}
		// Log any errors.
		if (errorOutput != "" && !HintsBogusError(errorOutput)){
	        UnityEngine.Debug.LogWarning("Git error: " + errorOutput);
		}
		return output;

	}

	public static bool HintsBogusError(string arg){
		foreach(var e in bogusErrorHints){
			if(arg.Contains(e)) return true;
		}
		return false;
	}

}}
