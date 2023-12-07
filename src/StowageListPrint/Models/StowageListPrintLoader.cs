namespace StowageListPrint.Models
{
    public class StowageListPrintLoader
    {
        public static IEnumerable<StowageListPrint> Get()
        {
            return new StowageListPrint[]
            {
                new StowageListPrint
                {
                    Tdunitcode = "0001",
                    CdShukkaBatch = "02001",
                    CdCourse = "001",
                    CdRoute = 1,
                    CdTokuisaki = "227577",
                    NmTokuisaki = "小谷SA売店",
                    LargeBox = 5,
                    SmallBox = 12,
                    BlueBox = 0,
                    EtcBox = 0,
                },
                new StowageListPrint
                {
                    Tdunitcode = "0003",
                    CdShukkaBatch = "02001",
                    CdCourse = "001",
                    CdRoute = 2,
                    CdTokuisaki = "000001",
                    NmTokuisaki = "得意先名",
                    LargeBox = 1,
                    SmallBox = 3,
                    BlueBox = 0,
                    EtcBox = 0,
                },
                new StowageListPrint
                {
                    Tdunitcode = "0004",
                    CdShukkaBatch = "02001",
                    CdCourse = "001",
                    CdRoute = 3,
                    CdTokuisaki = "000002",
                    NmTokuisaki = "得意先名",
                    LargeBox = 10,
                    SmallBox = 4,
                    BlueBox = 0,
                    EtcBox = 0,
                },
                new StowageListPrint
                {
                    Tdunitcode = "0004",
                    CdShukkaBatch = "02001",
                    CdCourse = "001",
                    CdRoute = 3,
                    CdTokuisaki = "000012",
                    NmTokuisaki = "得意先名",
                    LargeBox = 3,
                    SmallBox = 5,
                    BlueBox = 0,
                    EtcBox = 0,
                },
                new StowageListPrint
                {
                    Tdunitcode = "0009",
                    CdShukkaBatch = "02001",
                    CdCourse = "001",
                    CdRoute = 4,
                    CdTokuisaki = "000003",
                    NmTokuisaki = "得意先名",
                    LargeBox = 1,
                    SmallBox = 1,
                    BlueBox = 0,
                    EtcBox = 0,
                },
                new StowageListPrint
                {
                    Tdunitcode = "0010",
                    CdShukkaBatch = "02001",
                    CdCourse = "002",
                    CdRoute = 1,
                    CdTokuisaki = "000004",
                    NmTokuisaki = "得意先名",
                    LargeBox = 0,
                    SmallBox = 5,
                    BlueBox = 0,
                    EtcBox = 0,
                },
            };
        }
    }
}
