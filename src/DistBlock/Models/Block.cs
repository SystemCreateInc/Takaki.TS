using Prism.Mvvm;

namespace DistBlock.Models
{
    public class Block : BindableBase
    {
        private int _seq;
        public int Seq
        {
            get => _seq;
            set => SetProperty(ref _seq, value);
        }

        private string _blockCode = string.Empty;
        public string BlockCode
        {
            get => _blockCode;
            set => SetProperty(ref _blockCode, value);
        }

        private string _start = string.Empty;
        public string Start
        {
            get => _start;
            set => SetProperty(ref _start, value);
        }

        private string _end = string.Empty;
        public string End
        {
            get => _end;
            set => SetProperty(ref _end, value);
        }
    }
}
