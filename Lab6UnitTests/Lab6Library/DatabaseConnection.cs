using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6Library
{
    public sealed class DatabaseConnection
    {
        public int Execute(string query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            throw new InvalidOperationException("Don't connect to db in unit tests!");
        }
    }
}
