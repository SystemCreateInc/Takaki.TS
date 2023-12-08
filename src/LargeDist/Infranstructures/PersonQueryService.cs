using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using LargeDist.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LargeDist.Infranstructures
{
    internal class PersonQueryService
    {
        internal static IEnumerable<Person> GetAll()
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBMSHAINEntity>(s => s
                    .OrderBy($"{nameof(TBMSHAINEntity.CDSHAIN):C}"))
                    .Select(x => new Person(x.CDSHAIN, x.NMSHAIN))
                    .ToArray();
            }
        }
    }
}