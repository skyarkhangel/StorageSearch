using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ImprovedFilter {
    public class EventArgs<T> : EventArgs {
        public EventArgs(T value) {
            this.Value = value;
        }

        public T Value { get; set; }
    }
}
