using NUnit.Framework;

namespace Flour.Test
{
	public class DataSerializerTest
	{
		private DataSerializer serializer;


		[SetUp]
		public void Setup()
		{
			serializer = new DataSerializer();
		}

		[Test]
		public void TestToDefaultType()
		{
			int testInt = 23;
			float testFloat = 123.13f;
			string testString = "test string";

			var si = serializer.Serialize(testInt);
			var sf = serializer.Serialize(testFloat);
			var ss = serializer.Serialize(testString);

			var di = serializer.Deserialize<int>(si);
			var df = serializer.Deserialize<float>(sf);
			var ds = serializer.Deserialize<string>(ss);

			Assert.IsTrue(testInt == di);
			Assert.IsTrue(testFloat == df);
			Assert.IsTrue(testString == ds);
		}

		[Test]
		public void TestToStruct()
		{
			var src = new TestStruct { text = "hoge", count = 19, value = 15.43f };

			var s = serializer.Serialize<TestStruct>(src);
			var d = serializer.Deserialize<TestStruct>(s);

			Assert.IsTrue(src.Equals(d));
		}

		[Test]
		public void TestToClass()
		{
			var src = new TestClass("fuga", 134, 1.42f);

			var s = serializer.Serialize<TestClass>(src);
			var d = serializer.Deserialize<TestClass>(s);

			Assert.IsTrue(src.Equals(d));
		}



		[System.Serializable]
		private struct TestStruct : System.IEquatable<TestStruct>
		{
			public string text;
			public int count;
			public float value;

			public bool Equals(TestStruct test)
			{
				return text == test.text && count == test.count && value == test.value;
			}
		}

		[System.Serializable]
		private class TestClass : System.IEquatable<TestClass>
		{
			private string text;
			private int count;
			private float value;

			public TestClass(string text, int count, float value)
			{
				this.text = text;
				this.count = count;
				this.value = value;
			}

			public bool Equals(TestClass test)
			{
				return text == test.text && count == test.count && value == test.value;
			}
		}
	}
}
