using System;
using UnityEngine;
using System.Diagnostics;

namespace Activ.PuP{
public static class SVNRunner{

	public static string Cmd(string workdir, string command){
		string output = "no-svn";
		string errorOutput = "no-svn";
		if(workdir != null && workdir.Length > 0){
			UnityEngine.Debug.LogError("workdir not supported for now");
	        return null;
		}
		var processInfo = new ProcessStartInfo("svn", command){
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
			UnityEngine.Debug.LogError("SVN may not be setup correctly.");
			throw e;
		}
		// Read the results back from the process so we can get the output
		// and check for errors
		output = process.StandardOutput.ReadToEnd();
		errorOutput = process.StandardError.ReadToEnd();
		process.WaitForExit();
		process.Close();
		// Check for failure due to no git setup in the project itself or
	    // other fatal errors from git.
		if (output.Contains("fatal") || output == "no-svn") {
			throw new Exception(
	            $"Command: svn {command} Failed\n" + output + errorOutput);
		}
		// Log any errors.
		if (errorOutput != "")
			UnityEngine.Debug.LogWarning("SVN error: " + errorOutput);

		return output;

	}

}}
