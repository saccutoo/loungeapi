﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Utils;

namespace Infrastructure
{
    public partial class ModelContext : DbContext
    {
        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }


        public ModelContext(string prefix, string databaseType, string connectionString)
        {
            Prefix = prefix;
            DatabaseType = databaseType;
            ConnectionString = connectionString;
        }
        public string Prefix { get; set; }
        public string DatabaseType { get; set; }
        public string ConnectionString { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                #region Config database

                switch (DatabaseType)
                {
                    case "MSSQL":
                        optionsBuilder.UseSqlServer(ConnectionString);
                        break;
                    case "Oracle":
                        optionsBuilder.UseOracle(ConnectionString);
                        break;
                    default:
                        optionsBuilder.UseSqlServer(ConnectionString);
                        break;
                }

                #endregion

                optionsBuilder.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
            }
        }
    }
    public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
        {
            var databaseType = Helpers.GetConfig("ConnectionString:DbType");
            var connectionString = Helpers.GetConfig("ConnectionString:" + databaseType);
            if (context is ModelContext dynamicContext)
            {
                return (context.GetType(), dynamicContext.Prefix, databaseType, connectionString);
            }
            return context.GetType();
        }
    }
}
