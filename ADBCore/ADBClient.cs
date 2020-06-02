using System;
using System.Text;

namespace ADBCore
{
	public sealed class ADBClient
	{
		public static byte[] FormAdbRequest(string req) {
			string resultStr = string.Format("{0}{1}", req.Length.ToString("X4"), req);
			byte[] result = Encoding.ASCII.GetBytes(resultStr);
			return result;
		}
	}
}
