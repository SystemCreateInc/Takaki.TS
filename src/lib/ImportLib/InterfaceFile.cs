
using DbLib.Defs;

namespace ImportLib
{
    public record InterfaceFile(DataType DataType, string Name, int? SortOrder, string FileName, int? ExpDays);
}
