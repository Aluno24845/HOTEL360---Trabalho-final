using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOTEL360___Trabalho_final.Models{
    public class Reservas {

        /// <summary>
        /// Construtor da classe Reservas 
        /// </summary>
        public Reservas() {
            ListaServicos = new HashSet<Servicos>();
            
        }

        /// <summary>
        /// Chave Primária 
        /// </summary>
        [Key] 
        public int Id { get; set; }

        /// <summary>
        /// atributo auxiliar para ler o Valor pago na inteface
        /// </summary>
        [NotMapped] // não representa este atributo na BD
        [StringLength(8)]
        [Display(Name = "Valor Pago")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [RegularExpression("[0-9]+[.,]?[0-9]{0,2}",
           ErrorMessage = "só aceita digitos numéricos, separados por . ou por ,")]
        public string ValorPagoAux { get; set; }

        /// <summary>
        /// Valor já pago pelo Hospede
        /// </summary>
        [Display(Name = "Valor Pago")]
        public decimal ValorPago { get; set; }

        /// <summary>
        /// Valor total da reserva
        /// </summary>
        [Display(Name = "Valor Total")]
        public decimal ValorTotal { get; set; }

        /// <summary>
        /// Valor que falta pagar da reserva
        /// </summary>
        [Display(Name = "Valor que falta pagar")]
        public decimal ValorAPagar { get; set; }

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
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
        [DisplayFormat(ApplyFormatInEditMode = true,
                     DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime DataCheckIN { get; set; }

        /// <summary>
        /// Data de saída do Quarto
        /// </summary>
        [Display(Name = "Data do Check-OUT")]
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
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

        // relacionamento do tipo N-1
        [ForeignKey(nameof(Hospede))] // anotação que liga HospedeFK a Reserva
        public int HospedeId { get; set; } // Será FK para a tabela Reserva
        public Hospedes Hospede { get; set; }  // em rigor esta instrução seria a única necessária

        // relacionamento do tipo N-M, SEM atributos do relacionamento
        /// <summary>
        /// Lista das Servicos associadas à Reserva
        /// </summary>
        public ICollection<Servicos> ListaServicos { get; set; }

    }
}
