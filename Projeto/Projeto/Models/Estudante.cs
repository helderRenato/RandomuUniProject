using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Projeto.Models
{
    public class Estudante
    {
        /// <summary>
        /// Identificador do estudante
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// UserName do estudante
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Id do estudadante que se refere ao variável identificadora na base de dados de autenticação (Identity)
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Fotografia de Perfil 
        /// </summary>
        public string? Fotografia { get; set; }

        /// <summary>
        /// Em que universidade está associado este estudante
        /// </summary>

        [ForeignKey(nameof(Universidade))]
        public int? UniversidadeFK { get; set; }
        public Universidade? Universidade { get; set; }
    }
}
