namespace Projeto.Models
{

    /// <summary>
    /// ViewModel para ser usado na API dos Donos
    /// </summary>
    public class PostsViewModel
    {
        public int Id { get; set; }
        public string Texto { get; set; }
        public string Fotografia { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int NewsletterFk { get; set; }
    }

    public class NewsletterViewModel
    {
        public int Id { get; set; }
    }

}