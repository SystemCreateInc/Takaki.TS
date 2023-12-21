using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportLib.Repository;

namespace ExportLib
{
    public interface IExportRepositoryFactory
    {
        IExportRepository Create();
    }
}
