using DbLib.Defs;
using DbLib.Extensions;
using Prism.Mvvm;

namespace SeatMapping.Models
{
    public class SeatMapping : BindableBase
    {
        private string _tdunitaddrcode = string.Empty;
        public string Tdunitaddrcode
        {
            get => _tdunitaddrcode;
            set => SetProperty(ref _tdunitaddrcode, value);
        }

        private RemoveType _removeType;
        public RemoveType RemoveType
        {
            get => _removeType;
            set => SetProperty(ref _removeType, value);
        }

        public string DispRemoveType => EnumExtensions.GetDescription(RemoveType);
    }
}
