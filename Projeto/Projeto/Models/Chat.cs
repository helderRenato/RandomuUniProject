using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Projeto.Models
{
    public class Chat
    {
        //Identificação do chat criado
        [Key]
        public int Id { get; set; }

        //Originador da mensagem no chat
        [ForeignKey(nameof(Estudante))]
        public string Sender { get; set; }

        //Destinatário da Mensagem 
        [ForeignKey(nameof(Estudante))]
        public string Receiver { get; set; }

    }
}
