using System;

namespace Tools
{
	public class OperationResult<T>
	{
		private readonly T result;

		private OperationResult(string message, Exception exception)
		{
			Message = message;
			Exception = exception;
		}

		private OperationResult(T result)
		{
			this.result = result;
			IsCompleted = true;
		}

		public T Result
		{
			get
			{
				if (!IsCompleted)
					throw new InvalidOperationException("Result is undefined.");

				return result;
			}
		}

		public bool IsCompleted { get; }

		public bool IsCancelled => !IsCompleted;
		public Exception Exception { get; }
		public string Message { get; }

		public static OperationResult<T> Success(T result) => new OperationResult<T>(result);

		public static OperationResult<T> Failed(string message = null, Exception ex = null) =>
			new OperationResult<T>(message, ex);
	}
}