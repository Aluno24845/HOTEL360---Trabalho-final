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
        /// <summary>
        /// Tabela Utilizadores
        /// </summary>
        public DbSet<Utilizadores> Utilizadores { get; set; }
        /// <summary>
        /// Tabela Hospedes
        /// </summary>
        public DbSet<Hospedes> Hospedes { get; set; }
        /// <summary>
        /// Tabela Gerentes
        /// </summary>
        public DbSet<Gerentes> Gerentes { get; set; }
        /// <summary>
        /// Tabela Reccecionistas
        /// </summary>
        public DbSet<Reccecionistas> Reccecionistas { get; set; }
        /// <summary>
        /// Tabela Quartos
        /// </summary>
        public DbSet<Quartos> Quartos { get; set; }        
        /// <summary>
        /// Tabela Reservas
        /// </summary>
        public DbSet<Reservas> Reservas { get; set; }
        /// <summary>
        /// Tabela Servicos
        /// </summary>
        public DbSet<Servicos> Servicos { get; set; }

    }
}
