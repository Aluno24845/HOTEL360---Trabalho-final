using System.Globalization;

namespace HOTEL360___Trabalho_final.Models
{
    public class Utilizadores {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Telemovel { get; set; }

        public string Avatar { get; set; }

        public DateOnly DataNascimento { get; set; }
    }
}
