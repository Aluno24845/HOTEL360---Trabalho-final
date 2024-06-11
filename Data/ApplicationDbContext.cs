using HOTEL360___Trabalho_final.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HOTEL360___Trabalho_final.Data{

    /// <summary>
    /// classe responsável pela criação e gestão da Base de Dados
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options){
        }

        // definição das 'tabelas'
        public DbSet<Utilizadores> Utilizadores { get; set; }
        public DbSet<Hospedes> Hospedes { get; set; }
        public DbSet<Gerentes> Gerentes { get; set; }
        public DbSet<Reccecionistas> Reccecionistas { get; set; }
        public DbSet<Quartos> Quartos { get; set; }
        public DbSet<Reservas_Servicos> ReservasServicos { get; set; }
        public DbSet<Reservas> Reservas { get; set; }
        public DbSet<Servicos> Servicos { get; set; }

    }
}
