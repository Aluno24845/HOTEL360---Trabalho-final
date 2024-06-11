using System.ComponentModel.DataAnnotations.Schema;

namespace HOTEL360___Trabalho_final.Models
{
    public class Reservas_Servicos {

        public int Quantidade { get; set; }


        /* ************************************************
         * Vamos criar as Relações (FKs) com as
         *    tabelas Reservas e Servicos
         * *********************************************** */

        // FK para a tabela das Reservas
        [ForeignKey(nameof(Reserva))]
        public int ReservaFK { get; set; }
        public Reservas Reserva { get; set; }

        // FK para a tabela dos Servicos
        [ForeignKey(nameof(Servico))]
        public int ServicoFK { get; set; }
        public Servicos Servico { get; set; }
    }
}
