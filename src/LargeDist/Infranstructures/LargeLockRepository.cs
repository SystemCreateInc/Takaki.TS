using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using LargeDist.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeDist.Infranstructures
{
    internal static class LargeLockRepository
    {
        private static string Id => $"{Environment.MachineName}";

        public static void Lock(string cdLargeGroup, LargeDistItem item)
        {
            try
            {
                using var con = DbFactory.CreateConnection();
                con.Insert(new TBLARGELOCKEntity
                {
                    CDLARGEGROUP = cdLargeGroup,
                    CDSHUKKABATCH = item.CdShukkaBatch,
                    CDJUCHUBIN = item.CdJuchuBin,
                    CDHIMBAN = item.CdHimban,
                    IDPROCESS = Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    throw new Exception("既に別パソコンでスキャン済みです");
                }

                throw;
            }
        }

        public static void UnlockAll()
        {
            using var con = DbFactory.CreateConnection();
            con.BulkDelete<TBLARGELOCKEntity>(s => s
                .Where($"{nameof(TBLARGELOCKEntity.IDPROCESS):C} = {nameof(Id):P}")
                .WithParameters(new { Id }));
        }

        public static void Unlock(string cdLargeGroup, LargeDistItem item)
        {
            string cdShukkaBatch = item.CdShukkaBatch;
            string cdJuchuBin = item.CdJuchuBin;
            string cdHimban = item.CdHimban;

            using var con = DbFactory.CreateConnection();
            con.BulkDelete<TBLARGELOCKEntity>(s => s
                .Where(@$"{nameof(TBLARGELOCKEntity.IDPROCESS):C} = {nameof(Id):P} and 
                            { nameof(TBLARGELOCKEntity.CDLARGEGROUP):C} = { nameof(cdLargeGroup):P} and 
                            { nameof(TBLARGELOCKEntity.CDSHUKKABATCH):C} = { nameof(cdShukkaBatch):P} and 
                            {nameof(TBLARGELOCKEntity.CDJUCHUBIN):C} = {nameof(cdJuchuBin):P} and 
                            {nameof(TBLARGELOCKEntity.CDHIMBAN):C} = {nameof(cdHimban):P}")
                .WithParameters(new { Id, cdLargeGroup, cdShukkaBatch, cdJuchuBin, cdHimban }));
        }
    }
}
