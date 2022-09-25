 import React from "react";
 import Tabela from "./Tabela";
 import Formulario from "./Formulario";

 async function getPosts() {
   // ler dados da API
   let dados = await fetch("api/PostAPI/");
 
   // se não se conseguir ler os dados...
   if (!dados.ok) {
     console.error(dados);
     throw new Error("Não foi possível obter os dados dos Posts. Código: ",
       dados.status)
   }
   // devolver os dados lidos
   return await dados.json();
 }
 
 /**
  * Procura os dados dos donos dos animais, através da API
  * @returns 
  */
 async function getNewsletter() {
   // ler dados da API
   let dados = await fetch("api/NewsletterAPI/");
 
   // se não se conseguir ler os dados...
   if (!dados.ok) {
     console.error(dados);
     throw new Error("Não foi possível obter os dados dos Posts. Código: ",
       dados.status)
   }
   // devolver os dados lidos
   return await dados.json();
 }

 async function apagaPost(idPost) {
 
   let formData = new FormData();
   formData.append("id", idPost);
 
   let resposta = await fetch("api/PostAPI/" + idPost,
     {
       method: "Delete",
       body: formData
     }
   );
   // validar a qualidade da resposta
   if (!resposta.ok) {
     console.error("resposta: ", resposta);
     throw new Error("Não foi possível remover o post. Código: ", resposta.status)
   }
   else {
     alert("O post foi bem apagado...");
   }
 }
 
 async function adicionaPost(post) {
   //  preparar os objeto que transporta os dados para a API
   let formData = new FormData();
   formData.append("Texto", post.Texto);
   formData.append("Fotografia", post.Fotografia);
   formData.append("RegistrationDate", post.RegistrationDate);
   formData.append("NewsletterFk", post.NewsletterFk);
   // concretizar o transporte
   let resposta = await fetch("api/PostAPI",
     {
       method: "Post",
       body: formData
     }
   );
   // validar a qualidade da resposta
   if (!resposta.ok) {
     console.error("resposta: ", resposta);
     throw new Error("Não foi possível adicionar o post. Código: ", resposta.status)
   }
 }
 
 
 class App extends React.Component {
   // dados a serem manipulados dentro do componente
   state = {
     post: [],
     newsletter: [],
   }
 
   /**
    * este método funciona como se fosse o 'startup'
    * do componente
    */
   componentDidMount() {
     this.LoadPosts();
     this.LoadNewsletters();
   }
 
   async LoadPosts() {
     try {
       let postsDaAPI = await getPosts();
       this.setState({ post: postsDaAPI })
     } catch (erro) {
       console.error("Ocorreu um erro no acesso aos dados da API", erro);
     }
   }

   async LoadNewsletters() {
     try {
       let newsDaAPI = await getNewsletter();
       this.setState({ newsletter: newsDaAPI })
     } catch (erro) {
       console.error("Ocorreu um erro no acesso aos dados da API", erro);
     }
   }
 
   handleRemovePost = async (idPost) => {
     try {
       await apagaPost(idPost);
     } catch (error) {
       console.error("Ocorreu um erro na eliminação do post")
     }
     // atualizar os dados da Tabela
     await this.LoadPosts();
   }

   handleAdicionaPost = async (novoPost) => {
     try {
       // invoca a adição do Animal
       await adicionaPost(novoPost);
     } catch (error) {
       console.error("Ocorreu um erro na adição do Post")
     }
     // atualizar os dados da Tabela
     await this.LoadPosts();
   }
 
 
 
   render() {
     // ler os dados do state, para o Render os poder utilizar
     const { post, newsletter } = this.state;
 
     return (
       <div className="container">
         <h1>Post</h1>
         <h4>Adição de novo pots:</h4>
         <Formulario NewsIN={post} novoPostOUT={this.handleAdicionaPost} />
         <br />
 
         <h4>Animais:</h4>
         <Tabela dadosPostIN={post} idPostOUT={this.handleRemovePost} />
         <br />
       </div>
     )
   }
 }
 
 
 export default App;