namespace HOTEL360___Trabalho_final.Models
{
    public class Hospedes {
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
