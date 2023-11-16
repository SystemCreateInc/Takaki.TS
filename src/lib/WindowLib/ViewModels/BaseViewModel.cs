using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowLib.ViewModels
{
    public class BaseViewModel : BindableBase, INotifyDataErrorInfo
    {

        private Dictionary<string, string> Errors = new();
        public bool HasErrors => Errors.Select(x => string.IsNullOrEmpty(x.Value)).Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IRegionManager RegionManager { get; private set; }
        public IDialogService DialogService{ get; private set; }

        public BaseViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            RegionManager = regionManager;
            DialogService = dialogService;
        }

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
