using DbLib.Defs;
using DbLib.Extensions;

namespace SeatMapping.Models
{
    public class Combo
    {
        public int Index { get; set; }
        public string Id { get; set; } = string.Empty;
        public int UnitType { get; set; }

        public string Name => $"{Id}";
    }
}
