using System;
using System.Diagnostics;
using System.Threading;

namespace Assets.Scripts.Common
{
    public class Executor
    {
        private readonly TimeSpan _timeout;
        private readonly TimeSpan _pollingInterval;
        private readonly Stopwatch _stopwatch;
        private bool _isSatisfied = true;

        private Executor(TimeSpan timeout)
            : this(timeout, TimeSpan.FromSeconds(1))
        {
        }

        private Executor(TimeSpan timeout, TimeSpan pollingInterval)
        {
            _timeout = timeout;
            _pollingInterval = pollingInterval;
            _stopwatch = Stopwatch.StartNew();
        }

        public static Executor WithTimeout(TimeSpan timeout, TimeSpan pollingInterval)
        {
            return new Executor(timeout, pollingInterval);
        }

        public static Executor WithTimeout(TimeSpan timeout)
        {
            return new Executor(timeout);
        }

        public Executor WaitFor(Func<bool> condition)
        {
            if (!_isSatisfied)
            {
                return this;
            }

            while (!condition())
            {
                var sleepAmount = Min(_timeout - _stopwatch.Elapsed, _pollingInterval);

                if (sleepAmount < TimeSpan.Zero)
                {
                    _isSatisfied = false;
                    break;
                }

                Thread.Sleep(sleepAmount);
            }

            return this;
        }

        public bool IsSatisfied
        {
            get { return _isSatisfied; }
        }

        public void EnsureSatisfied()
        {
            if (!_isSatisfied)
            {
                throw new TimeoutException();
            }
        }

        public void EnsureSatisfied(string message)
        {
            if (!_isSatisfied)
            {
                throw new TimeoutException(message);
            }
        }

        public static bool SpinWait(Func<bool> condition, TimeSpan timeout)
        {
            return SpinWait(condition, timeout, TimeSpan.FromSeconds(1));
        }

        public static bool SpinWait(Func<bool> condition, TimeSpan timeout, TimeSpan pollingInterval)
        {
            return WithTimeout(timeout, pollingInterval).WaitFor(condition).IsSatisfied;
        }

        public static bool Try(Action action)
        {
            Exception exception;

            return Try(action, out exception);
        }

        public static bool Try(Action action, out Exception exception)
        {
            try
            {
                action();
                exception = null;

                return true;
            }
            catch (Exception e)
            {
                exception = e;

                return false;
            }
        }

        public static Func<bool> MakeTry(Action action)
        {
            return () => Try(action);
        }

        private static T Min<T>(T left, T right) where T : IComparable<T>
        {
            return left.CompareTo(right) < 0 ? left : right;
        }
    }
}