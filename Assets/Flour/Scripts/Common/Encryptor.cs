using System.Text;
using System.Security.Cryptography;

namespace Flour
{
	public class Encryptor : System.IDisposable
	{
		RijndaelManaged rijndael;

		public Encryptor(string pass)
		{
			rijndael = new RijndaelManaged();

			rijndael.Mode = CipherMode.CBC;
			rijndael.Padding = PaddingMode.PKCS7;

			rijndael.KeySize = 128;
			rijndael.BlockSize = 128;

			var salt = Encoding.UTF8.GetBytes("7jVoKSwiQJKOh27Fhe1Rw8rSRUAUwd4C");
			var deriveBytes = new Rfc2898DeriveBytes(pass, salt);
			deriveBytes.IterationCount = 1000;

			rijndael.Key = deriveBytes.GetBytes(rijndael.KeySize / 8);
			rijndael.IV = deriveBytes.GetBytes(rijndael.BlockSize / 8);
		}

		public void Dispose()
		{
			rijndael.Dispose();
		}

		public byte[] Encrypt(byte[] src)
		{
			using (var encryptor = rijndael.CreateEncryptor())
			{
				return encryptor.TransformFinalBlock(src, 0, src.Length);
			}
		}
		public string Encrypt(string src)
		{
			var srcBytes = Encoding.UTF8.GetBytes(src);
			return System.Convert.ToBase64String(Encrypt(srcBytes));
		}


		public byte[] Decrypt(byte[] src)
		{
			using (var decryptor = rijndael.CreateDecryptor())
			{
				return decryptor.TransformFinalBlock(src, 0, src.Length);
			}
		}
		public string Decrypt(string src)
		{
			var srcBytes = System.Convert.FromBase64String(src);
			return Encoding.UTF8.GetString(Decrypt(srcBytes));
		}
	}
}
