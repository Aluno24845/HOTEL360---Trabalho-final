using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOTEL360___Trabalho_final.Models
{
    [PrimaryKey(nameof(ReservaFK), nameof(ServicoFK))] // PK composta, na EF > 6.0
    public class Reservas_Servicos {
                
        public int Quantidade { get; set; }


        /* ************************************************
         * Vamos criar as Relações (FKs) com as
         *    tabelas Reservas e Servicos
         * *********************************************** */

        // FK para a tabela das Reservas
        [ForeignKey(nameof(Reserva))]
        //   [Key] // PK composta, na EF <= 6.0
        public int ReservaFK { get; set; }
        public Reservas Reserva { get; set; }

        // FK para a tabela dos Servicos
        [ForeignKey(nameof(Servico))]
        //   [Key] // PK composta, na EF <= 6.0
        public int ServicoFK { get; set; }
        public Servicos Servico { get; set; }
    }
}
