using Prism.Mvvm;

namespace ImportLib.Models
{
    public class ImportFileInfo : BindableBase
    {
        private bool _selected = false;
        public bool Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string? _filePath;
        public string? FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        private long? _fileSize;
        public long? FileSize
        {
            get => _fileSize;
            set => SetProperty(ref _fileSize, value);
        }

        private DateTime? _lastWriteTime;
        public DateTime? LastWriteTime
        {
            get => _lastWriteTime;
            set => SetProperty(ref _lastWriteTime, value);
        }

        public bool IsEnabled => (FileSize ?? 0) > 0;

    }
}
