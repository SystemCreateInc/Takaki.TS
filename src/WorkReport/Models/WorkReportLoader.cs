using System.Runtime.Intrinsics.X86;

namespace WorkReport.Models
{
    public class WorkReportLoader
    {
        public static IEnumerable<WorkReport> Get()
        {
            // fixme:読み込み機能追加
            return new WorkReport[]
            {
                new WorkReport
                {
                    DtDelivery = "20231001",
                    DtStart = DateTime.Now,
                    DtEnd = DateTime.Now.AddHours(1),
                    CdDistGroup = "21",
                    CdBlock = "1",
                    NmIdle = 600,
                    NmSyain = "佐藤一郎",
                    NmWorktime = 9000,
                    NmItemcnt = 20,
                    Shopcnt = 50,
                    NmDistcnt = 1234,
                    NmCheckcnt = 1,
                    NmChecktime = 300,
                },
            };
        }
    }
}
