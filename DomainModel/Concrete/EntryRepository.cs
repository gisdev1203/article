using DomainModel.Abstract;
using DomainModel.ObjectList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace DomainModel.Concrete
{
    public class EntryRepository : BaseRepository, IEntryRepository
    {
        public List<TestObject> GetDataFromTestTable(int id)
        {
            string sql = "SELECT id, name FROM test_table WHERE id = @A";
            return Query<TestObject>(sql, new { A = id });
        }
    }
}
