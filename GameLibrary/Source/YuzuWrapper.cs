using System.IO;
using Yuzu;
using Yuzu.Binary;
using Yuzu.Json;

namespace OceanSplash
{
	public static class YuzuWrapper
	{
		public static readonly CommonOptions defaultYuzuCommonOptions = new CommonOptions { AllowUnknownFields = true };
		public static readonly JsonSerializeOptions defaultYuzuJSONOptions = new JsonSerializeOptions { Unordered = true };

		public enum Format
		{
			JSON,
			Binary
		}

		public static void WriteObject<T>(Stream stream, T instance, Format format)
		{
			AbstractWriterSerializer ys = null;
			if (format == Format.Binary) {
				WriteYuzuBinarySignature(stream);
				ys = new BinarySerializer { Options = defaultYuzuCommonOptions };
			} else if (format == Format.JSON) {
				ys = new JsonSerializer {
					Options = defaultYuzuCommonOptions,
					JsonOptions = defaultYuzuJSONOptions
				};
			}
			ys.ToStream(instance, stream);
		}

		public static void WriteObjectToFile<T>(string path, T instance, Format format)
		{
			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None)) {
				WriteObject(stream, instance, format);
			}
		}

		public static T ReadObject<T>(Stream stream, object obj = null)
		{
			var ms = new MemoryStream();
			stream.CopyTo(ms);
			ms.Seek(0, SeekOrigin.Begin);
			stream = ms;
			Yuzu.Deserializer.AbstractReaderDeserializer yd = null;
			if (CheckYuzuBinarySignature(stream)) {
				yd = new BinaryDeserializer { Options = defaultYuzuCommonOptions };
			} else {
				yd = new JsonDeserializer {
					JsonOptions = defaultYuzuJSONOptions,
					Options = defaultYuzuCommonOptions
				};
			}
			var bd = yd as BinaryDeserializer;
			if (obj == null) {
				return (bd != null) ? bd.FromReader<T>(new BinaryReader(stream)) : yd.FromStream<T>(stream);
			} else {
				return (bd != null) ? (T)bd.FromReader(obj, new BinaryReader(stream)) : (T)yd.FromStream(obj, stream);
			}
		}

		private static Stream GenerateStreamFromString(string s)
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(s);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		public static string WriteObjectToString<T>(T instance, Format format)
		{
			using (var stream = GenerateStreamFromString("")) {
				WriteObject(stream, instance, format);
				var reader = new StreamReader(stream);
				stream.Seek(0, SeekOrigin.Begin);
				return reader.ReadToEnd();
			}
		}

		public static T ReadObjectFromString<T>(string source, object obj = null)
		{
			using (Stream stream = GenerateStreamFromString(source)) {
				return ReadObject<T>(stream, obj);
			}
		}

		public static T ReadObjectFromFile<T>(string path, object obj = null) where T : new()
		{
			using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				return ReadObject<T>(stream, obj);
			}
		}

		private static void WriteYuzuBinarySignature(Stream s)
		{
			var bw = new BinaryWriter(s);
			bw.Write(0xdeadbabe);
		}

		public static bool CheckYuzuBinarySignature(Stream s)
		{
			uint signature;
			try {
				var br = new BinaryReader(s);
				signature = br.ReadUInt32();
			} catch {
				s.Seek(0, SeekOrigin.Begin);
				return false;
			}
			var r = signature == 0xdeadbabe;
			if (!r) {
				s.Seek(0, SeekOrigin.Begin);
			}
			return r;
		}

		private static void CopyTo(this Stream input, Stream output)
		{
			byte[] buffer = new byte[16 * 1024];
			int bytesRead;

			while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0) {
				output.Write(buffer, 0, bytesRead);
			}
		}
	}
}