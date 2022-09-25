using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto.Models
{
    public class Post
    {
        /// <summary>
        /// Identificador do Post
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Texto do Post
        /// </summary>
        public string Texto { get; set; }

        /// <summary>
        /// Fotografias do Post
        /// </summary>
        public string? Fotografia { get; set; }

        /// <summary>
        /// Fotografias do Post
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// A que Newsletter pertence este Post
        /// </summary>
        [ForeignKey(nameof(Newsletter))]
        public int NewsletterFk { get; set; }
        public Newsletter? Newsletter { get; set; }

    }
}
