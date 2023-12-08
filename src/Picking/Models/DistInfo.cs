using DbLib.Defs;
using DbLib.Extensions;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TdDpsLib.Defs;

namespace Picking.Models
{
    public class DistInfo : DistBase
    {
        private int _sendstatus = 0;
        public int SendStatus
        {
            get => _sendstatus;
            set
            {
                SetProperty(ref _sendstatus, value);

                switch (value)
                {
                    case 0:
                        SendStatus_name = "";
                        break;
                    case 1:
                        SendStatus_name = "送信済";
                        break;
                }
            }
        }

        private string _sendstatus_name = "";
        public string SendStatus_name
        {
            get => _sendstatus_name;
            set => SetProperty(ref _sendstatus_name, value);
        }
    }
}
