using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto.Models
{
    public class Universidade
    {
        /// <summary>
        ///Identificador e chave primária da universidade
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///Id da Universidade que se refere ao variável identificadora na base de dados de autenticação (Identity)
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        ///Nome da universidade 
        /// </summary>
        public string? Nome { get; set; }

        /// <summary>
        ///Newsletter da universidade
        /// </summary>
        [ForeignKey(nameof(Newsletter))]
        public int? NewsletterFk { get; set; }

    }
}
