using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace ZooMania.Model.Repositories
{
    public abstract class RepositoryBase
    {
        private readonly string _connectionString;
        public RepositoryBase()
        {
            _connectionString = "Data Source=ZooManiaDB.sqlite";
        }
        protected SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }


    }
}
