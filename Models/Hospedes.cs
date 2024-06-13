using System.ComponentModel.DataAnnotations;

namespace HOTEL360___Trabalho_final.Models
{
    /// <summary>
    /// Hospedes é uma extensão de Utilizadores 
    /// </summary>
    public class Hospedes : Utilizadores{

        /// <summary>
        /// Construtor da classe Hospedes
        /// </summary>
        public Hospedes() {
            ListaReservas = new HashSet<Reservas>();
        }

        /// <summary>
        /// NIF do Hospede
        /// </summary>
        [Display(Name = "Número de contribuinte")]
        [StringLength(9)]
        [RegularExpression("[1235679][0-9]{8}",
            ErrorMessage = "O {0}  deve começar com 1,2,3,5,6,7,9 e só aceita 9 digitos")]
        public string NIF { get; set; }

        /* ************************************************
        * Vamos criar as Relações (FKs) com outras tabelas
        * *********************************************** */

        // relacionamento do tipo N-M, SEM atributos do relacionamento
        /// <summary>
        /// Lista das Reservas associadas ao Hospede
        /// </summary>
        public ICollection<Reservas> ListaReservas { get; set; }
        
    }
}
