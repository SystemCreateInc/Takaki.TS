using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using System.Runtime.Intrinsics.X86;

namespace WorkReport.Models
{
    public class WorkReportLoader
    {
        public static IEnumerable<WorkReport> Get(string startDate, string endDate)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBREPORTEntity>(s => s
                .Where($"{nameof(startDate):P}<={nameof(TBREPORTEntity.DTDELIVERY):C} and {nameof(TBREPORTEntity.DTDELIVERY):C}<{nameof(endDate):P}")
                .WithParameters(new { startDate, endDate }))
                    .Select(x => new WorkReport
                    {
                        DtDelivery = x.DTDELIVERY,
                        DtStart = x.DTSTART,
                        DtEnd = x.DTEND,
                        CdDistGroup = x.CDDISTGROUP,
                        CdBlock = x.CDBLOCK,
                        NmIdle = x.NMIDLE,
                        NmSyain = x.NMSYAIN,
                        NmWorktime = x.NMWORKTIME,
                        NmItemcnt = x.NMITEMCNT,
                        Shopcnt = x.NMSHOPCNT,
                        NmDistcnt = x.NMDISTCNT,
                        NmCheckcnt = x.NMCHECKCNT,
                        NmChecktime = x.NMCHECKTIME,
                    })
                    .OrderBy(x => x.DtDelivery)
                    .ThenBy(x => x.DtStart);
            }
        }
    }
}
