// ReSharper disable All
namespace Sharpen
{
    using System.IO;

	internal class ByteArrayOutputStream : OutputStream
	{
		public ByteArrayOutputStream ()
		{
			Wrapped = new MemoryStream ();
		}

		public ByteArrayOutputStream (int bufferSize)
		{
			Wrapped = new MemoryStream (bufferSize);
		}

		public long Size ()
		{
			return ((MemoryStream)Wrapped).Length;
		}

		public byte[] ToByteArray ()
		{
			return ((MemoryStream)Wrapped).ToArray ();
		}
		
		public override void Close ()
		{
			// Closing a ByteArrayOutputStream has no effect.
		}
		
		public override string ToString ()
		{
			return System.Text.Encoding.UTF8.GetString (ToByteArray ());
		}
	}
}
