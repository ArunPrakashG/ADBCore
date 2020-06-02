using System.Collections.Generic;

namespace ADBCore.Models {
	internal struct InternalCommandResult {
		internal readonly string Command;
		internal readonly int CommandProcessExitCode;
		internal readonly List<string>? ErrorOutput;
		internal readonly List<string>? GenericOutput;

		internal bool IsErrorReceived => ErrorOutput != null && ErrorOutput.Count > 0;
		internal bool IsOutputReceived => GenericOutput != null && GenericOutput.Count > 0;

		internal InternalCommandResult(string _command, int _exitCode, List<string>? _errorOutput, List<string>? _genericOutput) {
			Command = _command;
			ErrorOutput = _errorOutput;
			GenericOutput = _genericOutput;
		}
	}
}
