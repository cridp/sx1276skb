using System;
using System.Collections.Generic;
using System.Threading;

namespace SemtechLib.General
{
	public sealed class QueueTS<T> : Queue<T>, IDisposable
	{
		private object sync;

		private bool isDisposed;

		public new int Count
		{
			get
			{
				lock (sync)
				{
					return base.Count;
				}
			}
		}

		public QueueTS()
		{
			sync = new object();
			isDisposed = false;
		}

		public QueueTS(IEnumerable<T> collection)
			: base(collection)
		{
			sync = new object();
			isDisposed = false;
		}

		public QueueTS(int capacity)
			: base(capacity)
		{
			sync = new object();
			isDisposed = false;
		}

		public new void Clear()
		{
			lock (sync)
			{
				base.Clear();
			}
		}

		public new T Dequeue()
		{
			lock (sync)
			{
				return base.Dequeue();
			}
		}

		public new void Enqueue(T item)
		{
			lock (sync)
			{
				base.Enqueue(item);
				Monitor.Pulse(sync);
			}
		}

		public void Dispose()
		{
			if (!isDisposed)
			{
				isDisposed = true;
				lock (sync)
				{
					base.Clear();
					Monitor.PulseAll(sync);
				}
			}
		}
	}
}
