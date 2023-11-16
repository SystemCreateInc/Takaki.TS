using Prism.Mvvm;
using System.Collections.Generic;

namespace SearchBoxLib.Models
{
    public class Row : BindableBase
    {
        private IEnumerable<Content>? _contents;
        public IEnumerable<Content> Contents
        {
            get => _contents!;
            set => SetProperty(ref _contents, value);
        }
    }
}
