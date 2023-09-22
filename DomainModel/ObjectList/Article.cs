namespace DomainModel.ObjectList
{
    public class Article
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public bool Is_Deleted { get; set; }
        public int? Article_Form_Id { get; set; }
        public string Form_Data { get; set; }
    }
}
