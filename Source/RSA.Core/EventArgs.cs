namespace RSA.Core
{
    using System;

    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T value)
        {
            this.Value = value;
        }

        public T Value { get; set; }
    }
}