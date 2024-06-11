using System.ComponentModel.DataAnnotations;

namespace HOTEL360___Trabalho_final.Models
{
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
