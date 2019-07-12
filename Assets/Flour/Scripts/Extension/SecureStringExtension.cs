using System.Runtime.InteropServices;
using System.Security;

namespace Flour
{
	public static class SecureStringExtension
	{
		public static SecureString Set(this SecureString secure, string plainText)
		{
			for (int i = 0; i < plainText.Length; i++) secure.AppendChar(plainText[i]);
			return secure;
		}
		public static string ToPlainText(this SecureString secure)
		{
			return Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(secure));
		}
	}
}
