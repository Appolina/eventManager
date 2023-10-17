using System;
using System.Collections.Generic;

namespace EventManager.WPFExtentions
{
    public class ValidationErrors
    {
        private readonly Dictionary<string, string> validationErrors = new Dictionary<string, string>();

        public bool IsValid
        {
            get
            {
                return this.validationErrors.Count < 1;
            }
        }

        public string this[string index]
        {
            get { return this.validationErrors.ContainsKey(index) ? this.validationErrors[index] : string.Empty; }
            set { this.validationErrors[index] = value; }
        }

        internal void Clear()
        {
            validationErrors.Clear();
        }
    }
}
