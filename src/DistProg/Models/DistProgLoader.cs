namespace DistProg.Models
{
    public class DistProgLoader
    {
        public static IEnumerable<DistProg> Get()
        {
            return new DistProg[]
            {
                new DistProg
                {
                    Tdunitareacode = 1,
                    CdBlock = "01",
                    NmShain = "サトウイチロウ",
                    CdDistGroup = "02001",
                    NmDistGroup = "仕分グループ1",
                    DtDelivery = "20231015",
                    DtStart = new DateTime(2023, 12, 8, 9, 1, 0),
                    DtEnd = null,
                    NuRitemcnt = 15,
                    NuOitemcnt = 100,
                    NuRps = 300,
                    NuOps = 5000,
                }
            };
        }

        public static IEnumerable<DistProg> GetUncompleteds()
        {
            return new DistProg[]
            {
                new DistProg
                {
                    CdBlock = "01",
                    NmShain = "サトウイチロウ",
                    CdDistGroup = "02001",
                     NmDistGroup = "仕分グループ1",
                     DtStart = new DateTime(2023, 12, 18, 9, 25, 0),
                     DtEnd = null,
                     NuRitemcnt = 15,
                     NuOitemcnt = 100,
                     NuRps = 300,
                     NuOps = 5000,
                }
            };
        }

        public static IEnumerable<DistProg> GetCompleteds()
        {
            return new DistProg[]
            {
                new DistProg
                {
                    Tdunitareacode = 1,
                    CdBlock = "01",
                    NmShain = "サトウイチロウ",
                    CdDistGroup = "02001",
                    NmDistGroup = "仕分グループ1",
                    DtDelivery = "20231015",
                    DtStart = new DateTime(2023, 12, 8, 9, 1, 0),
                    DtEnd = null,
                    NuRitemcnt = 15,
                    NuOitemcnt = 100,
                    NuRps = 300,
                    NuOps = 5000,
                }
            };
        }
    }
}
