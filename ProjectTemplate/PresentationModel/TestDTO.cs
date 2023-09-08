using DomainModel.ObjectList;
using System.Collections.Generic;

namespace ProjectTemplate.PresentationModel
{
    public class TestDTO
    {
        public List<TestObject> TestObjects { get; set; } = new List<TestObject>();
    }
}
