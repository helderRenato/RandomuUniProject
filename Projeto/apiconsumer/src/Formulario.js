 import React from "react";

 const EscolheNewsletter = (props) => {
     const opcoes = props.dadosNewsIN.map((row) => {
         return (<option value={row.id}>{row.nome}</option>)
     })
     return (
         <select required
             className="form-select"
             onChange={props.NewsEscolhidaOUT}>
             <option value="">Selecione, por favor, uma newsletter</option>
             {opcoes}
         </select>
     )
 }
 
 class Formulario extends React.Component {
     // criar um objeto no STATE para recolher
     // e manipular os dados do Formulário
     state = {
         Texto: "", 
         Fotografia: null, 
         RegistrationDate: "",  
         NewsletterFk: "", 
     }
 
     handleAdicao = (evento) => {
         const { name, value } = evento.target
         this.setState({ [name]: value })
     }
 
     /**
      * entrega ao React o ficheiro selecionado pelo utilizador
      * @param {*} evento 
      */
     handleFotografia= (evento) => {
         this.setState({ Fotografia: evento.target.files[0] })
     }
 
     /**
      * entrega ao React o valor escolhido pelo utilizador,
      * na dropdown
      * @param {*} evento 
      */
     handleNewsletterChange = (evento) => {
         this.setState({ NewsletterFk: evento.target.value })
     }
 
     /**
      * preparar os dados recolhidos pelo formulário 
      * para serem enviados para a API
      * @param {*} evento 
      */
     handleSubmitForm = (evento) => {
         // vamos começar por anular a ação
         // pré-definida do formulário.
         // Neste caso, não vai haver 'submit'
         evento.preventDefault();
 
         // preparar os dados para serem enviados para a API
         let dadosForm = {
             Texto: this.state.Texto, 
             Fotografia: this.state.Fotografia, 
             RegistrationDate: this.state.RegistrationDate,  
             NewsletterFk: this.state.NewsletterFk,  
         }
         // exportar os dados para fora do Formulário
         this.props.novoPostOUT(dadosForm)
         // limpar o formulario
         this.setState({
            Texto: "", 
            Fotografia: "", 
            RegistrationDate: "",  
            NewsletterFk: "", 
         })
     }
 
 
     render() {
         // ler, dentro deste método, os dados do State e do Props
         // para poderem ser utilizados
         const { Texto } = this.state;
         const { NewsIN } = this.props;
 
         return (
             <form method="Post"
                 encType="multipart/form-data"
                 onSubmit={this.handleSubmitForm}
             >
                 <div className="row">
                     <div className="col-md-4">
                         Texto: <input type="text"
                             required
                             className="form-control"
                             name="Texto"
                             value={Texto}
                             onChange={this.handleAdicao}
                         /><br />
                     </div>

                     <div className="col-md-4">
                         Fotografia: <input type="file"
                             required
                             name="fotoPost"
                             accept=".jpg,.png"
                             className="form-control"
                             onChange={this.handleFotografia} /><br />
                
                         Newsletter: <EscolheNewsletter dadosDonosIN={NewsIN}
                             NewsEscolhidaOUT={this.handleNewsletterChange} />
                         <br />
                     </div>
                 </div>
                 <input type="submit" value="Adicionar Post" className="btn btn-outline-primary" />
             </form>
         )
     }
 }
 
 export default Formulario;