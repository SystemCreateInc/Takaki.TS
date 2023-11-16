using Dapper.FastCrud;
using DbLib.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace DbLib
{
    public static class ActionLogBuilder
    {
        public static void Create(string personid, string work, string operation, string status, object obj)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var jsonText = JsonSerializer.Serialize(obj, new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                });

                var personnm = con.Get(new PersonEntity {  Personid = personid })?.Personnm ?? "";

                con.Insert(new ActionLogEntity
                {
                    Personid = personid,
                    Personnm = personnm,
                    Work = work,
                    Operation = operation,
                    Status = status,
                    Terminal = Environment.MachineName,
                    Description = jsonText,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });
            }
        }
    }
}
