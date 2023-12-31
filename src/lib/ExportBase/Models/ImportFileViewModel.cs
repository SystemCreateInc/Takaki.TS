﻿using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportBase.Models
{
    public class ExportFileViewModel : BindableBase
    {
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

    }
}
