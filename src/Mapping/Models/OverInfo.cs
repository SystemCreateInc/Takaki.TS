﻿using DbLib.Defs;
using Prism.Mvvm;

namespace Mapping.Models
{
    public class OverInfo : BindableBase
    {
        public OverInfo(Dist dist, DistGroup distgroup)
        {
            CdShukkaBatch = dist.CdShukkaBatch;
            NmShukkaBatch = dist.NmShukkaBatch;

            CdDistGroup = distgroup.CdDistGroup;
            NmDistGroup = distgroup.NmDistGroup;

            CdCourse = dist.CdSumCourse;
            CdRoute = dist.CdSumRoute;
            CdTokuisaki = dist.CdTokuisaki;
            NmTokuisaki = dist.NmTokuisaki;

            CdHimban = dist.CdHimban;
            NmHinSeishikimei = dist.NmHinSeishikimei;
            CdGtin13 = dist.CdGtin13;
            Ops = dist.Ops;
        }

        private string _cdDistGroup = string.Empty;
        public string CdDistGroup
        {
            get => _cdDistGroup;
            set => SetProperty(ref _cdDistGroup, value);
        }

        private string _nmDistGroup = string.Empty;
        public string NmDistGroup
        {
            get => _nmDistGroup;
            set => SetProperty(ref _nmDistGroup, value);
        }
        private string _cdShukkaBatch = string.Empty;
        public string CdShukkaBatch
        {
            get => _cdShukkaBatch;
            set => SetProperty(ref _cdShukkaBatch, value);
        }

        private string _nmShukkaBatch = string.Empty;
        public string NmShukkaBatch
        {
            get => _nmShukkaBatch;
            set => SetProperty(ref _nmShukkaBatch, value);
        }

        private string _cdTokuisaki = string.Empty;
        public string CdTokuisaki
        {
            get => _cdTokuisaki;
            set => SetProperty(ref _cdTokuisaki, value);
        }

        private string _nmTokuisaki = string.Empty;
        public string NmTokuisaki
        {
            get => _nmTokuisaki;
            set => SetProperty(ref _nmTokuisaki, value);
        }

        private string _cdCourse = string.Empty;
        public string CdCourse
        {
            get => _cdCourse;
            set => SetProperty(ref _cdCourse, value);
        }

        private int _cdRoute = 0;
        public int CdRoute
        {
            get => _cdRoute;
            set => SetProperty(ref _cdRoute, value);
        }

        private string _cdHimban = string.Empty;
        public string CdHimban
        {
            get => _cdHimban;
            set => SetProperty(ref _cdHimban, value);
        }

        private string _nmHinSeishikimei = string.Empty;
        public string NmHinSeishikimei
        {
            get => _nmHinSeishikimei;
            set => SetProperty(ref _nmHinSeishikimei, value);
        }

        private string _cdGtin13 = string.Empty;
        public string CdGtin13
        {
            get => _cdGtin13;
            set => SetProperty(ref _cdGtin13, value);
        }

        private int _ops = 0;
        public int Ops
        {
            get => _ops;
            set => SetProperty(ref _ops, value);
        }
    }
}
