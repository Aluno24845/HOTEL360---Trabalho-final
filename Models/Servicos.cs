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

        [Key] //PK
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao {  get; set; }
        public decimal Preco { get; set; }

        /* ************************************************
       * Vamos criar as Relações (FKs) com outras tabelas
       * *********************************************** */

        // relacionamento do tipo N-M, COM atributos do relacionamento
        // não vou referenciar a tabela 'final',
        // mas a tabela no 'meio' do relacionamento
        // vamos representar o relacionamento N-M à custa
        // de dois relacionamentos do tipo 1-N
        public ICollection<Reservas_Servicos> ListaReservasServicos { get; set; }

    }
}
