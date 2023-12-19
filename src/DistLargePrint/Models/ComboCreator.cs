using DbLib.DbEntities;
using DbLib;
using Dapper.FastCrud;

namespace DistLargePrint.Models
{
    public class ComboCreator
    {
        public static List<Combo> Create(string deliveryDate)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBLARGEGROUPEntity>(s => s
                .Where($"{nameof(TBLARGEGROUPEntity.DTTEKIYOKAISHI):C}<={nameof(deliveryDate):P} and {nameof(deliveryDate):P}<{nameof(TBLARGEGROUPEntity.DTTEKIYOMUKO):C}")
                .OrderBy($"{nameof(TBLARGEGROUPEntity.CDLARGEGROUP)}")
                .WithParameters(new { deliveryDate }))
                    .Select((value, index) => new Combo
                    {
                        Index = index,
                        Name = value.CDLARGEGROUP,
                    }).ToList();
            }
        }
    }
}
