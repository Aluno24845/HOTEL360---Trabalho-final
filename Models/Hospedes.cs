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

        public string NIF { get; set; }

        /* ************************************************
        * Vamos criar as Relações (FKs) com outras tabelas
        * *********************************************** */

        // relacionamento do tipo N-M, SEM atributos do relacionamento
        public ICollection<Reservas> ListaReservas { get; set; }
        
    }
}
