using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOTEL360___Trabalho_final.Models
{
    public class Servicos {

        /// <summary>
        /// Construtor da classe Servicos
        /// </summary>
        public Servicos() {
            ListaReservasServicos = new HashSet<Reservas_Servicos>();
        }

        /// <summary>
        /// Chave Primária
        /// </summary>
        [Key] 
        public int Id { get; set; }

        /// <summary>
        /// Nome do Serviço
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
        [StringLength(255)]
        public string Nome { get; set; }

        /// <summary>
        /// Descrição do Serviço
        /// </summary>
        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
        [StringLength(255)]
        public string Descricao {  get; set; }

        /// <summary>
        /// Preço do Serviço
        /// </summary>
        public decimal Preco { get; set; }

        /* ************************************************
       * Vamos criar as Relações (FKs) com outras tabelas
       * *********************************************** */

        // relacionamento do tipo N-M, COM atributos do relacionamento
        // não vou referenciar a tabela 'final',
        // mas a tabela no 'meio' do relacionamento
        // vamos representar o relacionamento N-M à custa
        // de dois relacionamentos do tipo 1-N
        /// <summary>
        /// Lista das Reservas_Servicos associadas ao Servico
        /// </summary>
        public ICollection<Reservas_Servicos> ListaReservasServicos { get; set; }

    }
}
