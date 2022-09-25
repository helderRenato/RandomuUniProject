using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto.Models
{
    public class Newsletter
    {
        //Construtor
        public Newsletter()
        {
            ListaDePosts = new HashSet<Post>(); 
        }

        /// <summary>
        /// Identificador da Newsletter
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// A que universidade pertence a newsletter
        /// </summary>
        [ForeignKey(nameof(Universidade))]
        public int UniversidadeFk { get; set; }

        /// <summary>
        /// Lista de Posts associadas a newsletter
        /// </summary>
        public ICollection<Post> ListaDePosts { get; set; }
    }
}
