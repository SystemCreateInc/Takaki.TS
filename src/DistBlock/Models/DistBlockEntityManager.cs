using DbLib.DbEntities;
using DbLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakakiLib.Models;
using Dapper.FastCrud;
using System.Data;

namespace DistBlock.Models
{
    public class DistBlockEntityManager
    {
        internal static void Regist(DistBlockInfo targetInfo, ShainInfo shainInfo)
        {
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                var entity = new TBDISTBLOCKEntity
                {
                    CDKYOTEN = targetInfo.CdKyoten,
                    CDDISTGROUP = targetInfo.CdDistGroup,
                    DTTEKIYOKAISHI = targetInfo.Tekiyokaishi,
                    DTTEKIYOMUKO = targetInfo.TekiyoMuko,
                    CDHENKOSHA = shainInfo.HenkoshaCode,
                    NMHENKOSHA = shainInfo.HenkoshaName,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                con.Insert(entity, x => x.AttachToTransaction(tr));

                InsertBlocks(tr, entity.IDDISTBLOCK, targetInfo.Blocks.ToList());

                tr.Commit();
            }
        }

        internal static void Update(DistBlockInfo targetInfo, ShainInfo shainInfo)
        {
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                var entity = GetEntity(tr, targetInfo.DistBlockId);

                if (entity is null)
                {
                    throw new Exception("更新対象のデータが見つかりません");
                }

                entity.DTTEKIYOKAISHI = targetInfo.Tekiyokaishi;
                entity.DTTEKIYOMUKO = targetInfo.TekiyoMuko;
                entity.UpdatedAt = DateTime.Now;

                con.Update(entity, x => x.AttachToTransaction(tr));

                DeleteAllBlock(tr, entity.IDDISTBLOCK);

                InsertBlocks(tr, entity.IDDISTBLOCK, targetInfo.Blocks.ToList());

                tr.Commit();
            }
        }

        private static void InsertBlocks(IDbTransaction tr, long idDistBlock, List<Block> blocks)
        {
            var sequence = 1;

            foreach (var block in blocks)
            {
                var entity = new TBDISTBLOCKSEQEntity
                {
                    IDDISTBLOCK = idDistBlock,
                    CDBLOCK = block.PadBlock,
                    NUBLOCKSEQ = sequence,
                    CDADDRFROM = block.PadAddrFrom,
                    CDADDRTO = block.PadAddrTo,

                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                tr.Connection!.Insert(entity, x => x.AttachToTransaction(tr));
                sequence++;
            }
        }

        private static TBDISTBLOCKEntity? GetEntity(IDbTransaction tr, long idDistBlock)
        {
            return tr.Connection!.Get(new TBDISTBLOCKEntity { IDDISTBLOCK = idDistBlock }, x => x.AttachToTransaction(tr));
        }

        private static void DeleteAllBlock(IDbTransaction tr, long idDistBlock)
        {
            tr.Connection!.BulkDelete<TBDISTBLOCKSEQEntity>(s => s
                .AttachToTransaction(tr)
                .Where($"{nameof(TBDISTBLOCKSEQEntity.IDDISTBLOCK):C} = @idDistBlock")
                .WithParameters(new { idDistBlock }));
        }
    }
}
