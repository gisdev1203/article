using DomainModel.ObjectList;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModel.Abstract
{
    public interface IEntryRepository
    {
        List<TestObject> GetDataFromTestTable(int id);
    }
}
