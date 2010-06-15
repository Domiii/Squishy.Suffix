using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Domi.Suffix
{
	public class SuffixBuildException : Exception
	{
		public SuffixBuildException()
		{
		}

		public SuffixBuildException(string message)
			: base(message)
		{
		}

		public SuffixBuildException(string message, params object[] args)
			: base(string.Format(message, args))
		{
		}

		public SuffixBuildException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected SuffixBuildException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
