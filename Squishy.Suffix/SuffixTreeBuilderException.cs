using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Squishy.Suffix
{
	public class SuffixTreeBuilderException : Exception
	{
		public SuffixTreeBuilderException()
		{
		}

		public SuffixTreeBuilderException(string message)
			: base(message)
		{
		}

		public SuffixTreeBuilderException(string message, params object[] args)
			: base(string.Format(message, args))
		{
		}

		public SuffixTreeBuilderException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected SuffixTreeBuilderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
