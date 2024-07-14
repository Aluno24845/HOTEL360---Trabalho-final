using HOTEL360___Trabalho_final.Models;
using Microsoft.AspNetCore.Identity;
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
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    /* Esta instrução importa tudo o que está pre-definido
        //     * na super classe
        //     */
        //    base.OnModelCreating(builder);

        //    /* Adição de dados à Base de Dados
        //     * Esta forma é PERSISTENTE, pelo que apenas deve ser utilizada em 
        //     * dados que perduram da fase de 'desenvolvimento' para a fase de 'produção'.
        //     * Implica efetuar um 'Add-Migration'
        //     * 
        //     * Atribuir valores às ROLES
        //     */
        //    builder.Entity<IdentityRole>().HasData(
        //        new IdentityRole { Id = "h", Name = "Hospedes", NormalizedName = "HOSPEDES" },
        //        new IdentityRole { Id = "r", Name = "Reccecionistas", NormalizedName = "RECCECIONISTAS" },
        //        new IdentityRole { Id = "g", Name = "Gerentes", NormalizedName = "GERENTES" }
        //        );

        //}

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
