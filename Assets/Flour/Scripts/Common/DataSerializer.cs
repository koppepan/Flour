using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Flour
{
	internal sealed class DataSerializer
	{
		readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

		public string Serialize<T>(T obj)
		{
			using (var stream = new MemoryStream())
			{
				try
				{
					binaryFormatter.Serialize(stream, obj);
					return Convert.ToBase64String(stream.GetBuffer());
				}
				catch (SerializationException e)
				{
					throw new SerializationException($"Failed to serialize. Reason: {e.Message}", e);
				}
			}
		}

		public T Deserialize<T>(string str)
		{
			using (var memory = new MemoryStream(Convert.FromBase64String(str)))
			{
				try
				{
					return (T)binaryFormatter.Deserialize(memory);
				}
				catch (SerializationException e)
				{
					throw new SerializationException($"Failed to deserialize. Reason: {e.Message}", e);
				}
			}
		}
	}
}
