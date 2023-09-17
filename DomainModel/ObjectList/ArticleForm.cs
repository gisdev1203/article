namespace DomainModel.ObjectList
{
    public class ArticleForm
    {
        public int Id { get; set; }
        public string Type { get; set; }
        //TODO: We will deserialize to a DTO later (form_definition json NOT NULL)
        public string FormDefinition { get; set; }         
    }
}
