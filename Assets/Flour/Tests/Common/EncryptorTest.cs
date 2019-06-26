using System.Linq;
using NUnit.Framework;

namespace Flour.Test
{
	public class EncryptorTest
	{
		private Encryptor encryptor;

		[SetUp]
		public void Setup()
		{
			encryptor = new Encryptor("tTAaCFdG16TOsFQEreh1K7q2OofNHjAd");
		}

		[TearDown]
		public void Dispose()
		{
			encryptor.Dispose();
		}

		[Test]
		public void TestToBytes()
		{
			byte[] testSrc = new byte[] { 0x4D, 0xA8, 0xF0, 0x2C, 0x14 };

			var eBytes = encryptor.Encrypt(testSrc);
			var dBytes = encryptor.Decrypt(eBytes);

			Assert.IsTrue(testSrc.SequenceEqual(dBytes));
		}

		[Test]
		public void TestToBytesInUsing()
		{
			byte[] testSrc = new byte[] { 0xC3, 0x8A , 0x13, 0x2D, 0xAA, 0xFF, 0x01, 0x33 };

			using (var e = new Encryptor("5mqcUvTMPEXxlsqBpROg7CxhmutAL5M4"))
			{
				var eBytes = e.Encrypt(testSrc);
				var dBytes = e.Decrypt(eBytes);

				Assert.IsTrue(testSrc.SequenceEqual(dBytes));
			}
		}

		[Test]
		public void TestToString()
		{
			var testSrc = "これはテスト用の文字列です。";

			var eText = encryptor.Encrypt(testSrc);
			var dText = encryptor.Decrypt(eText);

			Assert.IsTrue(string.Equals(testSrc, dText));
		}

		[Test]
		public void TestToStringInUsing()
		{
			var testSrc = "これはテスト用の文字列かもしれません。";

			using (var e = new Encryptor("jcPsII6mudEQmMuC8oDhOlFy5piIlQkD"))
			{
				var eText = e.Encrypt(testSrc);
				var dText = e.Decrypt(eText);

				Assert.IsTrue(string.Equals(testSrc, dText));
			}
		}
	}
}
