using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOTEL360___Trabalho_final.Models{
    public class Reservas {

        /// <summary>
        /// Construtor da classe Reservas 
        /// </summary>
        public Reservas() {
            ListaReservasServicos = new HashSet<Reservas_Servicos>();
            ListaRececcionistas = new HashSet<Reccecionistas>();
            ListaHospedes = new HashSet<Hospedes>();
        }

        /// <summary>
        /// Chave Primária 
        /// </summary>
        [Key] 
        public int Id { get; set; }

        /// <summary>
        /// Valor já pago pelo Hospede
        /// </summary>
        public decimal ValorPago { get; set; }

        /// <summary>
        /// Data em que foi feita a reserva
        /// </summary>
        [Display(Name = "Data da reserva")]
        [DisplayFormat(ApplyFormatInEditMode = true,
                     DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime DataReserva {  get; set; }

        /// <summary>
        /// Data de entrada no Quarto
        /// </summary>
        [Display(Name = "Data do Check-IN")]
        [DisplayFormat(ApplyFormatInEditMode = true,
                     DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime DataCheckIN { get; set; }

        /// <summary>
        /// Data de saída do Quarto
        /// </summary>
        [Display(Name = "Data do Check-OUT")]
        [DisplayFormat(ApplyFormatInEditMode = true,
                     DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime DataCheckOUT { get; set; }

        /* ************************************************
       * Vamos criar as Relações (FKs) com outras tabelas
       * *********************************************** */

        // relacionamento do tipo N-1
        [ForeignKey(nameof(Quarto))] // anotação que liga QaurtoFK a Quarto
        public int QuartoFK { get; set; } // Será FK para a tabela Quartos
        public Quartos Quarto { get; set; }  // em rigor esta instrução seria a única necessária


        // relacionamento do tipo N-M, SEM atributos do relacionamento
        /// <summary>
        /// Lista dos Reccecionistas associadas à Reserva
        /// </summary>
        public ICollection<Reccecionistas> ListaRececcionistas { get; set; }

        // relacionamento do tipo N-M, SEM atributos do relacionamento
        /// <summary>
        /// Lista dos Hospedes associados à Reserva
        /// </summary>
        public ICollection<Hospedes> ListaHospedes { get; set; }

        // relacionamento do tipo N-M, COM atributos do relacionamento
        // não vou referenciar a tabela 'final',
        // mas a tabela no 'meio' do relacionamento
        // vamos representar o relacionamento N-M à custa
        // de dois relacionamentos do tipo 1-N
        /// <summary>
        /// Lista das Reservas_Servicos associadas à Reserva
        /// </summary>
        public ICollection<Reservas_Servicos> ListaReservasServicos { get; set; }

    }
}
