using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Extensions;
using ReferenceLogLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistLargeGroup.Infranstructures
{
    internal static class LargeGroupRepository
    {
        internal static Models.DistLargeGroup? FindById(long id)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var entity = con.Get(new TBLARGEGROUPEntity { IDLARGEGROUP = id });
                if (entity == null)
                {
                    return null;
                }

                return new Models.DistLargeGroup
                {
                    IdLargeGroup = entity.IDLARGEGROUP,
                    CdKyoten = entity.CDKYOTEN,
                    CdLargeGroup = entity.CDLARGEGROUP,
                    CdLargeGroupName = entity.CDLARGEGROUPNAME,
                    DtTekiyoKaishi = (DateTime)entity.DTTEKIYOKAISHI.ParseNonSeparatedDate()!,
                    DtTekiyoMuko = (DateTime)entity.DTTEKIYOMUKO.ParseNonSeparatedDate()!,
                    CdHenkosha = entity.CDHENKOSHA,
                    NmHenkosha = entity.NMHENKOSHA,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt,
                };
            }
        }

        internal static void Save(Models.DistLargeGroup largeGroup)
        {
            using (var con = DbFactory.CreateConnection())
            {
                TBLARGEGROUPEntity? entity = null;
                if (largeGroup.IdLargeGroup != 0)
                {
                    entity = con.Get(new TBLARGEGROUPEntity { IDLARGEGROUP = largeGroup.IdLargeGroup });
                }

                if (entity is null)
                {
                    entity = new TBLARGEGROUPEntity
                    {
                        IDLARGEGROUP = largeGroup.IdLargeGroup,
                        CDKYOTEN = largeGroup.CdKyoten,
                        CDLARGEGROUP = largeGroup.CdLargeGroup,
                        CDLARGEGROUPNAME = largeGroup.CdLargeGroupName,
                        DTTEKIYOKAISHI = largeGroup.DtTekiyoKaishi.ToString("yyyyMMdd"),
                        DTTEKIYOMUKO = largeGroup.DtTekiyoMuko.ToString("yyyyMMdd"),
                        CDHENKOSHA = largeGroup.CdHenkosha,
                        NMHENKOSHA = largeGroup.NmHenkosha,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    con.Insert(entity);
                }
                else
                {
                    entity.CDLARGEGROUPNAME = largeGroup.CdLargeGroupName;
                    entity.DTTEKIYOMUKO = largeGroup.DtTekiyoMuko.ToString("yyyyMMdd");
                    entity.CDHENKOSHA = largeGroup.CdHenkosha;
                    entity.NMHENKOSHA = largeGroup.NmHenkosha;
                    entity.UpdatedAt = DateTime.Now;
                    con.Update(entity);
                }
            }
        }
    }
}
