using Customer.Loader;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using ImTools;
using System.Transactions;

namespace Customer.Models
{
    public class CustomerManager
    {
        internal static void Regist(SumCustomer targetCustomer, ShainInfo shainInfo)
        {
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                var sumTokuiEntity = new TBSUMTOKUISAKIEntity
                {
                    CDKYOTEN = targetCustomer.CdKyoten,
                    CDSUMTOKUISAKI = targetCustomer.CdSumTokuisaki,
                    DTTEKIYOKAISHI = targetCustomer.Tekiyokaishi,
                    DTTEKIYOMUKO = targetCustomer.TekiyoMuko,
                    CDHENKOSHA = shainInfo.HenkoshaCode,
                    NMHENKOSHA = shainInfo.HenkoshaName,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                con.Insert(sumTokuiEntity, x => x.AttachToTransaction(tr));

                InsertChild(tr, sumTokuiEntity.IDSUMTOKUISAKI, targetCustomer.ChildCustomers);                

                tr.Commit();
            }
        }

        internal static void Update(SumCustomer targetCustomer, ShainInfo shainInfo)
        {
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                var sumTokuiEntity = GetEntity(tr, targetCustomer.SumTokuisakiId);

                if(sumTokuiEntity is null)
                {
                    throw new Exception("更新対象のデータが見つかりません");
                }

                sumTokuiEntity.DTTEKIYOKAISHI = targetCustomer.Tekiyokaishi;
                sumTokuiEntity.DTTEKIYOMUKO = targetCustomer.TekiyoMuko;
                sumTokuiEntity.UpdatedAt = DateTime.Now;

                con.Update(sumTokuiEntity, x => x.AttachToTransaction(tr));

                DeleteAllChild(tr, sumTokuiEntity.IDSUMTOKUISAKI);

                InsertChild(tr, sumTokuiEntity.IDSUMTOKUISAKI, targetCustomer.ChildCustomers);

                tr.Commit();
            }
        }

        private static TBSUMTOKUISAKIEntity? GetEntity(System.Data.IDbTransaction tr, long sumTokuisakiId)
        {
            return tr.Connection!.Get(new TBSUMTOKUISAKIEntity { IDSUMTOKUISAKI = sumTokuisakiId }, x => x.AttachToTransaction(tr));
        }

        private static void DeleteAllChild(System.Data.IDbTransaction tr, long sumTokuisakiId)
        {
            tr.Connection!.BulkDelete<TBSUMTOKUISAKICHILDEntity>(s => s
                .AttachToTransaction(tr)
                .Where($"{nameof(TBSUMTOKUISAKICHILDEntity.IDSUMTOKUISAKI):C} = @sumTokuisakiId")
                .WithParameters(new { sumTokuisakiId }));
        }

        private static void InsertChild(System.Data.IDbTransaction tr, long sumTokuisakiId, List<ChildCustomer> childCustomers)
        {
            foreach (var child in childCustomers)
            {
                var tokuiChildEntity = new TBSUMTOKUISAKICHILDEntity
                {
                    IDSUMTOKUISAKI = sumTokuisakiId,
                    CDTOKUISAKICHILD = child.CdTokuisakiChild,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                tr.Connection!.Insert(tokuiChildEntity, x=> x.AttachToTransaction(tr));
            }
        }
    }
}
