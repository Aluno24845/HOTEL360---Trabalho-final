namespace HOTEL360___Trabalho_final.Models
{
    public class Reccecionistas  {

        public Reccecionistas()  {
            ListaReservas = new HashSet<Reservas>();
        }

        public int NumReccecionista { get; set; }

        /* ************************************************
        * Vamos criar as Relações (FKs) com outras tabelas
        * *********************************************** */

        // relacionamento do tipo N-M, SEM atributos do relacionamento
        public ICollection<Reservas> ListaReservas { get; set; }

    }
}
