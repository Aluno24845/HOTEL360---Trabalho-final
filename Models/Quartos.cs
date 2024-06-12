using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace HOTEL360___Trabalho_final.Models{
    /// <summary>
    /// Classe para descrever os Quartos existentes no Hotel 
    /// </summary>
    public class Quartos {

        /// <summary>
        /// Construtor da classe Quartos 
        /// </summary>
        public Quartos() {
            ListaReservas = new HashSet<Reservas>();
        }

        /// <summary>
        /// Chave Primária
        /// </summary>
        [Key] 
        public int Id { get; set; }

        /// <summary>
        /// Nome do Quarto 
        /// </summary>
        [StringLength(100)]
        public string Nome { get; set; }

        /// <summary>
        /// Capacidade do Quarto
        /// </summary>
        public int Capacidade { get; set; }

        /// <summary>
        /// Preço do Quarto por noite
        /// </summary>
        public decimal Preco { get; set; }

        /// <summary>
        /// Descrição do Quarto
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Nome do ficheiro que contém a imagem do Quarto
        /// </summary>
        [StringLength(50)] // define o tamanho máximo como 50 caracteres
        public string? Imagem { get; set; } // o ? torna o preenchimento facultativo

        /* ************************************************
       * Vamos criar as Relações (FKs) com outras tabelas
       * *********************************************** */

        // relacionamento com as Reservas
        /// <summary>
        /// Lista das Reservas associadas ao Quarto
        /// </summary>
        public ICollection<Reservas> ListaReservas { get; set; }
                
    }
}
