using ADBCore.Exceptions;
using ADBCore.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ADBCore {
	public sealed class ADBServer {
		private readonly string ADBPath;

		public ADBServer(string? _adbPath) {
			if (string.IsNullOrEmpty(_adbPath) || !File.Exists(_adbPath)) {
				SearchCurrentDirectoryForADB(out _adbPath);

				if (string.IsNullOrEmpty(_adbPath)) {
					throw new AdbExecutableNotFoundException();
				}
			}

			ADBPath = _adbPath;
		}

		private InternalCommandResult InternalCommandExe(string _cmd) {
			if (string.IsNullOrEmpty(_cmd)) {
				return default;
			}

			ProcessStartInfo psi = new ProcessStartInfo(ADBPath, _cmd) {
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				UseShellExecute = false,
				RedirectStandardError = true,
				RedirectStandardOutput = true
			};

			InternalCommandResult result;
			using (Process process = Process.Start(psi)) {
				string? errorOutput = process.StandardError.ReadToEnd();
				string? genericOutput = process.StandardOutput.ReadToEnd();

				List<string> errorOutputList = new List<string>();
				List<string> genericOutputList = new List<string>();

				if (!string.IsNullOrEmpty(errorOutput)) {
					errorOutputList.AddRange(errorOutput.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
				}

				if (!string.IsNullOrEmpty(genericOutput)) {
					genericOutputList.AddRange(genericOutput.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
				}

				// get the return code from the process
				if (!process.WaitForExit(5000)) {
					process.Kill();
				}

				result = new InternalCommandResult(_cmd, process.ExitCode, errorOutputList, genericOutputList);
			}

			return result;
		}

		private bool SearchCurrentDirectoryForADB(out string? adbDirectoryPath) {
			adbDirectoryPath = null;
			string currentDir = Directory.GetCurrentDirectory();

			foreach (string dir in Directory.GetDirectories(currentDir)) {
				if (string.IsNullOrEmpty(dir)) {
					continue;
				}

				DirectoryInfo dirInfo = new DirectoryInfo(dir);
				if (dirInfo.Name.Equals(Constants.ADB_DIRECTORY, StringComparison.OrdinalIgnoreCase)) {
					int fileExistCount = 0;
					foreach (string file in Directory.GetFiles(dir)) {
						if (string.IsNullOrEmpty(file)) {
							continue;
						}

						try {
							if (new FileInfo(file).Name.Equals(Constants.ADB_EXECUTABLE_NAME, StringComparison.OrdinalIgnoreCase)) {
								fileExistCount++;
							}
							else if (new FileInfo(file).Name.Equals(Constants.ADB_WIN_API_DLL, StringComparison.OrdinalIgnoreCase)) {
								fileExistCount++;
							}
							else if (new FileInfo(file).Name.Equals(Constants.ADB_WIN_USB_API_DLL, StringComparison.OrdinalIgnoreCase)) {
								fileExistCount++;
							}
						}
						catch { continue; }
					}

					if (fileExistCount == 3) {
						adbDirectoryPath = dir;
						return true;
					}
				}
			}

			return false;
		}
	}
}
