namespace HOTEL360___Trabalho_final.Models{
using System.ComponentModel.DataAnnotations.Schema;
    public class Reservas {

        /// <summary>
        /// Construtor da classe Reservas 
        /// </summary>
        public Reservas() {
            ListaReservasServicos = new HashSet<Reservas_Servicos>();
            ListaRececcionistas = new HashSet<Reccecionistas>();
            ListaHospedes = new HashSet<Hospedes>();
        }

        public int Id { get; set; }
        public decimal ValorPago { get; set; }
        public DateTime DataReserva {  get; set; }
        public DateTime DataCheckIN { get; set; }
        public DateTime DataCheckOUT { get; set; }

        /* ************************************************
       * Vamos criar as Relações (FKs) com outras tabelas
       * *********************************************** */

        // relacionamento do tipo N-1
        [ForeignKey(nameof(Quarto))] // anotação que liga QaurtoFK a Quarto
        public int QuartoFK { get; set; } // Será FK para a tabela Quartos
        public Quartos Quarto { get; set; }  // em rigor esta instrução seria a única necessária


        // relacionamento do tipo N-M, SEM atributos do relacionamento
        public ICollection<Reccecionistas> ListaRececcionistas { get; set; }

        // relacionamento do tipo N-M, SEM atributos do relacionamento
        public ICollection<Hospedes> ListaHospedes { get; set; }

        // relacionamento do tipo N-M, COM atributos do relacionamento
        // não vou referenciar a tabela 'final',
        // mas a tabela no 'meio' do relacionamento
        // vamos representar o relacionamento N-M à custa
        // de dois relacionamentos do tipo 1-N
        public ICollection<Reservas_Servicos> ListaReservasServicos { get; set; }

    }
}
