using DbLib.Defs;

namespace SeatMapping.Models
{
    public class SeatMappingLoader
    {
        public static IEnumerable<SeatMapping> Get()
        {
            return new SeatMapping[]
            {
                new SeatMapping { Tdunitaddrcode = "0001", RemoveType = RemoveType.Include },
                new SeatMapping { Tdunitaddrcode = "0002", RemoveType = RemoveType.Include },
                new SeatMapping { Tdunitaddrcode = "0003", RemoveType = RemoveType.Include },
                new SeatMapping { Tdunitaddrcode = "0004", RemoveType = RemoveType.Include },
                new SeatMapping { Tdunitaddrcode = "0005", RemoveType = RemoveType.Remove },
                new SeatMapping { Tdunitaddrcode = "0006", RemoveType = RemoveType.Include },
                new SeatMapping { Tdunitaddrcode = "0007", RemoveType = RemoveType.Remove },
                new SeatMapping { Tdunitaddrcode = "0008", RemoveType = RemoveType.Include },
            };
        }
    }
}
