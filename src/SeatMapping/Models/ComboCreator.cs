namespace SeatMapping.Models
{
    public class ComboCreator
    {
        public static List<Combo> Create()
        {
            return new Combo[]
            {
                new Combo { Index = 0, Id = 0, Name = "1" },
                new Combo { Index = 1, Id = 1, Name = "2" },
            }.ToList();
        }
    }
}
