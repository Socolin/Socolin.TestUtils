// 
// Exceptions.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

namespace Sharpen
{
	internal class VirtualMachineError : Error
	{
	}

	internal class StackOverflowError : VirtualMachineError
	{
	}

	internal class BrokenBarrierException : Exception
	{
	}

	internal class BufferUnderflowException : Exception
	{
	}

	internal class CharacterCodingException : Exception
	{
	}

	internal class DataFormatException : Exception
	{
	}

	internal class EOFException : Exception
	{
		public EOFException ()
		{
		}

		public EOFException (string msg) : base(msg)
		{
		}
	}

	internal class Error : Exception
	{
		public Error ()
		{
		}

		public Error (Exception ex) : base("Runtime Exception", ex)
		{
		}

		public Error (string msg) : base(msg)
		{
		}

		public Error (string msg, Exception ex) : base(msg, ex)
		{
		}
	}

	internal class ExecutionException : Exception
	{
		public ExecutionException (Exception inner): base ("Execution failed", inner)
		{
		}
	}

	internal class InstantiationException : Exception
	{
	}

	internal class InterruptedIOException : Exception
	{
		public InterruptedIOException (string msg) : base(msg)
		{
		}
	}

	internal class MissingResourceException : Exception
	{
	}

	internal class NoSuchAlgorithmException : Exception
	{
	}

	internal class NoSuchElementException : Exception
	{
	}

	internal class NoSuchMethodException : Exception
	{
	}

	internal class OverlappingFileLockException : Exception
	{
	}

	internal class ParseException : Exception
	{
		public ParseException ()
		{
		}

		public ParseException (int errorOffset) : base($"Msg: msg. Error Offset: {errorOffset}")
		{ 
		}
	}

	internal class RuntimeException : Exception
	{
		public RuntimeException ()
		{
		}

		public RuntimeException (Exception ex) : base("Runtime Exception", ex)
		{
		}

		public RuntimeException (string msg) : base(msg)
		{
		}

		public RuntimeException (string msg, Exception ex) : base(msg, ex)
		{
		}
	}

	internal class StringIndexOutOfBoundsException : Exception
	{
	}

	internal class UnknownHostException : Exception
	{
	}

	internal class UnsupportedEncodingException : Exception
	{
	}

	internal class URISyntaxException : Exception
	{
		public URISyntaxException (string s, string msg) : base(s + " " + msg)
		{
		}
	}

	internal class ZipException : Exception
	{
	}

	internal class GitException : Exception
	{
	}
	
	class ConnectException: Exception
	{
		public ConnectException (string msg): base (msg)
		{
		}
	}
	
	class KeyManagementException: Exception
	{
	}
	
	class IllegalCharsetNameException: Exception
	{
		public IllegalCharsetNameException (string msg): base (msg)
		{
		}
	}
	
	class UnsupportedCharsetException: Exception
	{
		public UnsupportedCharsetException (string msg): base (msg)
		{
		}
	}
}

