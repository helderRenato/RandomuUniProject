import React, { Component } from "react";

 function Cabecalho() {
     return (
         <thead>
             <tr>
                 <th>Texto</th>
                 <th>Fotografia</th>
                 <th>RegistrationDate</th>
                 <th>NewsletterFK</th>
             </tr>
         </thead>
     )
 }
 
 const Corpo = (props) => {
     // iterar todos os elementos do JSON
     // e gerar as linhas da tabela
     const rows = props.dadosRecebidosIN.map((row) => {
         return (
             <tr key={row.id}>
                 <td>{row.Texto}</td>
                 <td>{row.Fotografia}</td>
                 <td>{row.RegistrationDate}</td>
                 <td>{row.NewsletterFK}</td>
                 <td>
                     <img src={'Post/' + row.Fotografia}
                          height="50" />
                 </td>
                 <td>
                     <button className="btn btn-outline-danger"
                         onClick={() => props.postARemoverOUT(row.id)}
                     >Remover</button>
                 </td>
             </tr>
         )
     })
 
     return (
         <tbody>{rows}</tbody>
     )
 }
 
 /**
  * componente Tabela
  */
 class Tabela extends Component {
     render() {
         // ler os dados que são envidados
         // para dentro da componente
         // const dadosAlunosIN=this.props.dadosAlunosIN
         const { dadosPostIN, idPostOut } = this.props
 
         return (
             <table className="table table-striped">
                 <Cabecalho />
                 <Corpo dadosRecebidosIN={dadosPostIN}
                        animalARemoverOUT={idPostOut} />
             </table>
         )
     }
 }
 
 export default Tabela