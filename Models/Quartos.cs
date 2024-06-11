namespace HOTEL360___Trabalho_final.Models
{
    public class Quartos {

        /// <summary>
        /// Construtor da classe Quartos 
        /// </summary>
        public Quartos() {
            ListaReservas = new HashSet<Reservas>();
        }

        public int Id { get; set; }

        public int Capacidade { get; set; }

        public decimal Preco { get; set; }

        public string Descricao { get; set; }

        public string Imagem { get; set; }

        /* ************************************************
       * Vamos criar as Relações (FKs) com outras tabelas
       * *********************************************** */

        // relacionamento com as Reservas
        public ICollection<Reservas> ListaReservas { get; set; }
                
    }
}
