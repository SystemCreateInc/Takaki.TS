using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowLib.Models
{
    public class NotifyDataError : BindableBase, INotifyDataErrorInfo
    {
        private Dictionary<string, string> Errors = new();
        public bool HasErrors => Errors.Select(x => string.IsNullOrEmpty(x.Value)).Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            return Errors.Where(x => x.Key == propertyName).Select(x => x.Value).ToArray();
        }

        public void SetError(string propertyName, string value)
        {
            Errors[propertyName] = value;
            RaiseErrorsChanged(propertyName);
        }

        public void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public void ClearErrors()
        {
            var properties = Errors.Select(x => x.Key).ToArray();
            foreach (var prop in properties)
            {
                Errors[prop] = "";
                RaiseErrorsChanged(prop);
            }
        }
    }
}
