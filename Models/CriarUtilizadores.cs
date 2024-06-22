using System.ComponentModel.DataAnnotations;

namespace HOTEL360___Trabalho_final.Models{

    /// <summary>
    /// classe genérica dos Utilizadores da aplicação
    /// </summary>
    public class CriarUtilizadores {

        /// <summary>
        /// Chave Primária
        /// </summary>
        [Key] 
        public int Id { get; set; }

        /// <summary>
        /// Email do utilizador
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [EmailAddress(ErrorMessage = "Escreva um {0} válido, por favor.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Password de acesso ao sistema, pelo utilizador
        /// </summary>
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório.")]
        [StringLength(20, ErrorMessage = "A {0} tem de ter, pelo menos {2}, e um máximo de {1} caracteres.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Confirmação da password
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar password")]
        [Compare("Password", ErrorMessage = "A password e a sua confirmação não coincidem.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Nome do Utilizador
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
        [StringLength(100)]
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
        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)] // informa a View de como deve tratar este atributo
        [DisplayFormat(ApplyFormatInEditMode = true,
                     DataFormatString = "{0:dd-MM-yyyy}")]
        [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
        public DateOnly DataNascimento { get; set; }

        /// <summary>
        /// atributo para funcionar como FK
        /// no relacionamento entre a 
        /// base de dados do 'negócio' 
        /// e a base de dados da 'autenticação'
        /// </summary>
        [StringLength(40)]
        public string UserId { get; set; }
    }
}
