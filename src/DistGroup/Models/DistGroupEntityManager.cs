﻿using Dapper.FastCrud;
using DbLib.DbEntities;
using DbLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakakiLib.Models;
using System.Data;
using Microsoft.Extensions.FileSystemGlobbing;
using DbLib.Defs;

namespace DistGroup.Models
{
    internal class DistGroupEntityManager
    {
        internal static void Regist(DistGroupInfo targetInfo, ShainInfo shainInfo)
        {
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                var entity = new TBDISTGROUPEntity
                {
                    CDKYOTEN = targetInfo.CdKyoten,
                    CDDISTGROUP = targetInfo.CdDistGroup,
                    NMDISTGROUP = targetInfo.NmDistGroup,
                    CDBINSUM = (short)targetInfo.CdBinSum,

                    DTTEKIYOKAISHI = targetInfo.Tekiyokaishi,
                    DTTEKIYOMUKO = targetInfo.TekiyoMuko,
                    CDHENKOSHA = shainInfo.HenkoshaCode,
                    NMHENKOSHA = shainInfo.HenkoshaName,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                con.Insert(entity, x => x.AttachToTransaction(tr));

                DeleteAllBatch(tr, entity.IDDISTGROUP);
                InsertBatchs(tr, entity.IDDISTGROUP, targetInfo.Batches);

                tr.Commit();
            }
        }

        internal static void Update(DistGroupInfo targetInfo, ShainInfo shainInfo)
        {
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                var entity = GetEntity(tr, targetInfo.IdDistGroup);

                if (entity is null)
                {
                    throw new Exception("更新対象のデータが見つかりません");
                }

                entity.CDBINSUM = (short)targetInfo.CdBinSum;

                entity.DTTEKIYOKAISHI = targetInfo.Tekiyokaishi;
                entity.DTTEKIYOMUKO = targetInfo.TekiyoMuko;
                entity.UpdatedAt = DateTime.Now;

                con.Update(entity, x => x.AttachToTransaction(tr));

                DeleteAllBatch(tr, entity.IDDISTGROUP);
                InsertBatchs(tr, entity.IDDISTGROUP, targetInfo.Batches);

                tr.Commit();
            }
        }

        private static TBDISTGROUPEntity? GetEntity(System.Data.IDbTransaction tr, long targetId)
        {
            return tr.Connection!.Get(new TBDISTGROUPEntity { IDDISTGROUP = targetId }, x => x.AttachToTransaction(tr));
        }

        private static void DeleteAllBatch(IDbTransaction tr, long idDistGroup)
        {
            tr.Connection!.BulkDelete<TBDISTGROUPSHUKKABATCHEntity>(s => s
                .AttachToTransaction(tr)
                .Where($"{nameof(TBDISTGROUPSHUKKABATCHEntity.IDDISTGROUP):C} = @idDistGroup")
                .WithParameters(new { idDistGroup }));

            tr.Connection!.BulkDelete<TBDISTGROUPLARGEGROUPEntity>(s => s
                .AttachToTransaction(tr)
                .Where($"{nameof(TBDISTGROUPLARGEGROUPEntity.IDDISTGROUP):C} = @idDistGroup")
                .WithParameters(new { idDistGroup }));

            tr.Connection!.BulkDelete<TBDISTGROUPCOURSEEntity>(s => s
                .AttachToTransaction(tr)
                .Where($"{nameof(TBDISTGROUPCOURSEEntity.IDDISTGROUP):C} = @idDistGroup")
                .WithParameters(new { idDistGroup }));
        }

        private static void InsertBatchs(System.Data.IDbTransaction tr, long iDDISTGROUP, List<BatchInfo> batches)
        {
            var sequence = 1;
            var timeStamp = DateTime.Now;

            foreach (var batchInfo in batches)
            {
                var batchEntity = new TBDISTGROUPSHUKKABATCHEntity
                {
                    IDDISTGROUP = iDDISTGROUP,
                    CDSHUKKABATCH = batchInfo.PadBatch,
                    NUSHUKKABATCHSEQ = sequence,
                    CreatedAt = timeStamp,
                    UpdatedAt = timeStamp,
                };

                tr.Connection!.Insert(batchEntity, x => x.AttachToTransaction(tr));

                var lDistEntity = new TBDISTGROUPLARGEGROUPEntity
                {
                    IDDISTGROUP = iDDISTGROUP,
                    CDLARGEGROUP = batchInfo.PadLarge,
                    NULARGEGROUPSEQ = sequence,
                    CreatedAt= timeStamp,
                    UpdatedAt= timeStamp,
                };

                tr.Connection!.Insert(lDistEntity, x => x.AttachToTransaction(tr));

                sequence++;

                // バッチ毎にコース登録
                InsertCourse(tr, iDDISTGROUP, batchInfo.PadBatch, batchInfo.Courses.ToList(), timeStamp);
            }
        }

        private static void InsertCourse(System.Data.IDbTransaction tr, long iDDISTGROUP, string cdShukkaBatch, List<Course> courses, DateTime timeStamp)
        {
            var sequence = 1;

            foreach(var course in courses)
            {
                var entity = new TBDISTGROUPCOURSEEntity
                {
                    IDDISTGROUP = iDDISTGROUP,
                    CDSHUKKABATCH = cdShukkaBatch,
                    CDCOURSE = course.PadCourse,
                    NUCOURSESEQ = sequence,
                    CreatedAt = timeStamp,
                    UpdatedAt= timeStamp,
                };

                tr.Connection!.Insert(entity, x => x.AttachToTransaction(tr));

                sequence++;
            }

        }
    }
}
