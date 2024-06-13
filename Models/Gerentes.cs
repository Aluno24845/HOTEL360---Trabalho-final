using System.ComponentModel.DataAnnotations;

namespace HOTEL360___Trabalho_final.Models
{
    /// <summary>
    /// Gerentes é uma extensão de Utilizadores
    /// </summary>
    public class Gerentes : Utilizadores{

        /// <summary>
        /// NIF do Gerente
        /// </summary>
        [Display(Name = "Número de contribuinte")]
        [StringLength(9)]
        [RegularExpression("[1235679][0-9]{8}", 
            ErrorMessage = "O {0}  deve começar com 1,2,3,5,6,7,9 e só aceita 9 digitos")]
        public string NIF { get; set; }

    }
}
