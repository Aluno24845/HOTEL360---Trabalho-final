using System.ComponentModel.DataAnnotations;

namespace HOTEL360___Trabalho_final.Models
{
    /// <summary>
    /// Reccecionistas é uma extensão de Utilizadores 
    /// </summary>
    public class Reccecionistas : Utilizadores {
        /// <summary>
        /// Construtor da classe Receccionistas
        /// </summary>
        public Reccecionistas()  {
            ListaReservas = new HashSet<Reservas>();
        }

        /// <summary>
        /// Numero do Reccecionista
        /// </summary>
        [Display(Name = "Número do Reccecionista")]
        public int NumReccecionista { get; set; }

        /* ************************************************
        * Vamos criar as Relações (FKs) com outras tabelas
        * *********************************************** */

        // relacionamento do tipo N-M, SEM atributos do relacionamento
        /// <summary>
        /// Lista das Reservas associadas ao Reccecionista
        /// </summary>
        public ICollection<Reservas> ListaReservas { get; set; }

    }
}
