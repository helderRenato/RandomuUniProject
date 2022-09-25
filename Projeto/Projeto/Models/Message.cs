using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// As mensagens não são guardadas permanentemente na base de dados
namespace Projeto.Models
{
    public class Message
    {
        //Identificação do chat criado
        public int Id { get; set; }

        //Originador da mensagem no chat
        [ForeignKey(nameof(Estudante))]
        public string Sender { get; set; }

        //Destinatário da Mensagem 
        [ForeignKey(nameof(Estudante))]
        public string Receiver { get; set; }

        //A mensagem que será enviada
        public string Mensagem { get; set; }

    }
}
