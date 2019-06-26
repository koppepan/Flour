using NUnit.Framework;

namespace Flour.Test
{
	class UserPrefsTest
	{
		private enum TestDataKey
		{
			TestValue1,
			TestValue2,
			TestValue3,
		}

		private UserPrefs<TestDataKey> userPrefs;
		private string DefaultUser = "testDefault";
		private string TestUser = "testUser";

		[SetUp]
		public void Setup()
		{
			userPrefs = new UserPrefs<TestDataKey>(DefaultUser);
		}
		[TearDown]
		public void Dispose()
		{
			userPrefs.DeleteUser(DefaultUser);
			userPrefs.DeleteUser(TestUser);
			userPrefs.Dispose();
		}

		[Test]
		public void TestToInt()
		{
			var testValue = 153;

			userPrefs.SetInt(TestDataKey.TestValue1, testValue);
			Assert.IsTrue(userPrefs.GetInt(TestDataKey.TestValue1) == testValue);

			userPrefs.DeleteKey(TestDataKey.TestValue1);
			Assert.IsFalse(userPrefs.HasKey(TestDataKey.TestValue1));
		}

		[Test]
		public void TestToFloat()
		{
			var testValue = 142.43f;

			userPrefs.SetFloat(TestDataKey.TestValue2, testValue);
			Assert.IsTrue(userPrefs.GetFloat(TestDataKey.TestValue2) == testValue);

			userPrefs.DeleteKey(TestDataKey.TestValue2);
			Assert.IsFalse(userPrefs.HasKey(TestDataKey.TestValue2));
		}

		[Test]
		public void TestToString()
		{
			var testString = "UserPrefsのTestです。";

			userPrefs.SetString(TestDataKey.TestValue3, testString);
			Assert.IsTrue(userPrefs.GetString(TestDataKey.TestValue3) == testString);

			userPrefs.DeleteKey(TestDataKey.TestValue3);
			Assert.IsFalse(userPrefs.HasKey(TestDataKey.TestValue3));
		}

		[Test]
		public void TestToChangeUser()
		{
			var testValue1 = 123;
			var testValue2 = 234;


			// defaultUserのValue1にSet
			userPrefs.ChangeUser(DefaultUser);
			userPrefs.SetInt(TestDataKey.TestValue1, testValue1);
			Assert.IsTrue(userPrefs.GetInt(TestDataKey.TestValue1) == testValue1);

			// testUserに切り替え
			userPrefs.ChangeUser(TestUser);
			Assert.IsTrue(userPrefs.UserKey == TestUser);

			// testUserのValue1にセット
			userPrefs.SetInt(TestDataKey.TestValue1, testValue2);
			Assert.IsTrue(userPrefs.GetInt(TestDataKey.TestValue1) == testValue2);

			// defaultUserに切り替え
			userPrefs.ChangeUser(DefaultUser);
			Assert.IsTrue(userPrefs.UserKey == DefaultUser);
			Assert.IsTrue(userPrefs.GetInt(TestDataKey.TestValue1) == testValue1);

			// defaultUserのValue1を削除
			userPrefs.DeleteKey(TestDataKey.TestValue1);
			Assert.IsFalse(userPrefs.HasKey(TestDataKey.TestValue1));

			// testuserに切り替え
			userPrefs.ChangeUser(TestUser);
			Assert.IsTrue(userPrefs.HasKey(TestDataKey.TestValue1));

			// testUserのValue1を削除
			userPrefs.DeleteKey(TestDataKey.TestValue1);
			Assert.IsFalse(userPrefs.HasKey(TestDataKey.TestValue1));

			userPrefs.DeleteUser(TestUser);
		}

		[Test]
		public void TestToDeleteUser()
		{
			userPrefs.ChangeUser(TestUser);

			userPrefs.SetInt(TestDataKey.TestValue1, 15);
			userPrefs.SetFloat(TestDataKey.TestValue2, 231.2f);
			userPrefs.SetString(TestDataKey.TestValue3, "test");

			userPrefs.DeleteUser(TestUser);

			Assert.IsFalse(userPrefs.HasKey(TestDataKey.TestValue1));
			Assert.IsFalse(userPrefs.HasKey(TestDataKey.TestValue2));
			Assert.IsFalse(userPrefs.HasKey(TestDataKey.TestValue3));

			userPrefs.ChangeUser(DefaultUser);
		}
	}
}
