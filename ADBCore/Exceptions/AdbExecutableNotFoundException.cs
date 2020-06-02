using System;
using System.Collections.Generic;
using System.Text;

namespace ADBCore.Exceptions {
	public class AdbExecutableNotFoundException : Exception {
		public AdbExecutableNotFoundException() :
			base($"'{Constants.ADB_EXECUTABLE_NAME}' executable not found at '{Constants.ADB_EXECUTABLE_PATH}'") { }
	}
}
