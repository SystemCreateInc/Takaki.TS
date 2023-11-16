using Dapper.FastCrud;
using DbLib.DbEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using System.Transactions;
using System.Diagnostics;

namespace DbLib
{
    public class Settings : ISettings
    {
        private readonly IDbTransaction _dbTransaction;

        public Settings(IDbTransaction dbTransaction)
        {
            _dbTransaction = dbTransaction;
        }

        public string Get(string key, string defvalue = "")
        {
            return _dbTransaction.Connection.Find< SettingEntity>(s => s
                .AttachToTransaction(_dbTransaction)
                .Where($"{nameof(SettingEntity.Value):C} = {nameof(key):P}")
                .WithParameters(new { key }))
                .FirstOrDefault()
                ?.Data ?? defvalue;
        }

        public int GetInt(string key, int defvalue = 0)
        {
            var data = _dbTransaction.Connection.Find<SettingEntity>(s => s
                .AttachToTransaction(_dbTransaction)
                .Where($"{nameof(SettingEntity.Value):C} = {nameof(key):P}")
                .WithParameters(new { key }))
                .FirstOrDefault()
                ?.Data;
            return int.TryParse(data, out int result) ? result : defvalue;
        }

        public void Set<T>(string key, T data)
        {
            var dao = _dbTransaction.Connection.Get(new SettingEntity
            {
                Value = key,
                Id = "",
            }, s => s.AttachToTransaction(_dbTransaction));

            if (dao == null)
            {
                _dbTransaction.Connection.Insert(new SettingEntity
                {
                    Value = key,
                    Data = data.ToString(),
                    Id = "",
                }, s => s.AttachToTransaction(_dbTransaction));
            }
            else
            {
                dao.Data = data.ToString();
                _dbTransaction.Connection.Update(dao,
                    s => s.AttachToTransaction(_dbTransaction));
            }
        }
    }
}
