using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DocumentStorage.Infrastructure.PostgreSql
{
    public abstract class BasePostgresRepository
    {
        protected string? _connectionString;

        public BasePostgresRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("documents_postgres");

            if (_connectionString is null) 
            {
                throw new Exception("Connection string must be not null");
            }
        }
    }
}