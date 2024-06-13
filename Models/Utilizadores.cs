using System.ComponentModel.DataAnnotations;

namespace HOTEL360___Trabalho_final.Models{

    /// <summary>
    /// classe genérica dos Utilizadores da aplicação
    /// </summary>
    public class Utilizadores {

        /// <summary>
        /// Chave Primária
        /// </summary>
        [Key] 
        public int Id { get; set; }

        /// <summary>
        /// Nome do Utilizador
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Telemóvel do Utilizador
        /// </summary>
        [Display(Name = "Telemóvel")]
        [StringLength(9)]
        [RegularExpression("9[1236][0-9]{7}",
             ErrorMessage = "O {0} só aceita 9 digitos")]
        public string Telemovel { get; set; }

        /// <summary>
        /// Nome do ficheiro que contém o avatar do Utilizador
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Data de nascimento do Utilizador
        /// </summary>
        public DateOnly DataNascimento { get; set; }
    }
}
