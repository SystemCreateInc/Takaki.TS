using ExportLib.Infranstructures;
using ExportLib.Repository;

namespace ExportLib
{
    public class ExportRepositoryFactory : IExportRepositoryFactory
    {
        public IExportRepository Create()
        {
            return new ExportRepository();
        }
    }
}
