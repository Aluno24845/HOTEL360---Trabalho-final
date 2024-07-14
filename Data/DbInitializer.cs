using Microsoft.AspNetCore.Identity;
using HOTEL360___Trabalho_final.Data;
using HOTEL360___Trabalho_final.Models;

namespace HOTEL360___Trabalho_final.Data
{
    internal class DbInitializer
    {

        internal static async void Initialize(ApplicationDbContext dbContext)
        {

            /*
             * https://stackoverflow.com/questions/70581816/how-to-seed-data-in-net-core-6-with-entity-framework
             * 
             * https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-6.0#initialize-db-with-test-data
             * https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/data/ef-mvc/intro/samples/5cu/Program.cs
             * https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0300
             */


            ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
            dbContext.Database.EnsureCreated();

            // var auxiliar
            bool haAdicao = false;



            // Se não houver Quartos, cria-os
            var quartos = Array.Empty<Quartos>();
            if (!dbContext.Quartos.Any())
            {
                quartos = [
                    new Quartos{ Nome="Titanic Dreams", Capacidade=2, Preco=100, Descricao="Quarto com casa de banho privada", Imagem="28805e2b-b8a5-47af-8edc-93177671872f.png", Localizacao="Rua Amorim Pereira nº92, 2400-023, Portugal",   },
                    new Quartos{ Nome="Happy room", Capacidade=4, Preco=250, Descricao="Possui televisão", Imagem="91469ea7-1d63-464d-a978-938a164b68e1.jpg", Localizacao="Rua da Fábrica nº46, 2100-020 Évora, Portugal",   },
                //adicionar outros quartos
                ];
                await dbContext.Quartos.AddRangeAsync(quartos);
                haAdicao = true;
            }

            // Se não houver Servicos, cria-os
            var servicos = Array.Empty<Servicos>();
            if (!dbContext.Servicos.Any())
            {
                servicos = [
                    new Servicos{ Nome="Jantar", Preco=25,Descricao="Inclui sopa, prato principal, sobremesa e bebida"},
                    new Servicos{ Nome="Almoço", Preco=4,Descricao="Sopa + Sobremesa"},
                    new Servicos{ Nome="Pequeno-Almoço", Preco=17,Descricao="Buffet"},
                    new Servicos{ Nome="SPA", Preco=40,Descricao="Duração de 1 hora"},
                //adicionar outros Servicos
                ];
                await dbContext.Servicos.AddRangeAsync(servicos);
                haAdicao = true;
            }


            // Se não houver Utilizadores Identity, cria-os
            var users = Array.Empty<IdentityUser>();
            //a hasher to hash the password before seeding the user to the db
            var hasher = new PasswordHasher<IdentityUser>();

            if (!dbContext.Users.Any())
            {
                users = [
                    new IdentityUser{UserName="gerente1@mail.pt", NormalizedUserName="GERENTE1@MAIL.PT", Email="gerente1@mail.pt",NormalizedEmail="GERENTE1@MAIL.PT", EmailConfirmed=true, SecurityStamp="5ZPZEF6SBW7IU4M344XNLT4NN5RO4GRU", ConcurrencyStamp="c86d8254-dd50-44be-8561-d2d44d4bbb2f", PasswordHash=hasher.HashPassword(null,"Aa0_aa") },
                    new IdentityUser{UserName="gerente2@mail.pt", NormalizedUserName="GERENTE2@MAIL.PT", Email="gerente2@mail.pt",NormalizedEmail="GERENTE2@MAIL.PT", EmailConfirmed=true, SecurityStamp="TW49PF6SBW7IU4M344XNLT4NN5RO4GRU", ConcurrencyStamp="d8254c86-dd50-44be-8561-d2d44d4bbb2f", PasswordHash=hasher.HashPassword(null,"Aa0_aa") },
                    
                    new IdentityUser{UserName="hospede1@mail.pt", NormalizedUserName="HOSPEDE1@MAIL.PT", Email="hospede1@mail.pt",NormalizedEmail="HOSPEDE1@MAIL.PT", EmailConfirmed=true, SecurityStamp="TW49PF6SBW7IU4M344XNLT4NN5RO4GRU", ConcurrencyStamp="d8254c86-dd50-44be-8561-d2d44d4bbb2f", PasswordHash=hasher.HashPassword(null,"Aa0_aa") },
                    new IdentityUser{UserName="hospede2@mail.pt", NormalizedUserName="HOSPEDE2@MAIL.PT", Email="hospede2@mail.pt",NormalizedEmail="HOSPEDE2@MAIL.PT", EmailConfirmed=true, SecurityStamp="TW49PF6SBW7IU4M344XNLT4NN5RO4GRU", ConcurrencyStamp="d8254c86-dd50-44be-8561-d2d44d4bbb2f", PasswordHash=hasher.HashPassword(null,"Aa0_aa") },
                         
                    new IdentityUser{UserName="rececionista1@mail.pt", NormalizedUserName="RECECIONISTA1@MAIL.PT", Email="rececionista1@mail.pt",NormalizedEmail="RECECIONISTA1@MAIL.PT", EmailConfirmed=true, SecurityStamp="TW49PF6SBW7IU4M344XNLT4NN5RO4GRU", ConcurrencyStamp="d8254c86-dd50-44be-8561-d2d44d4bbb2f", PasswordHash=hasher.HashPassword(null,"Aa0_aa") },
                    new IdentityUser{UserName="rececionista2@mail.pt", NormalizedUserName="RECECIONISTA2@MAIL.PT", Email="rececionista2@mail.pt",NormalizedEmail="RECECIONISTA2@MAIL.PT", EmailConfirmed=true, SecurityStamp="TW49PF6SBW7IU4M344XNLT4NN5RO4GRU", ConcurrencyStamp="d8254c86-dd50-44be-8561-d2d44d4bbb2f", PasswordHash=hasher.HashPassword(null,"Aa0_aa") }
                   ];
                await dbContext.Users.AddRangeAsync(users);
                haAdicao = true;
            }


            // Se não houver Hospedes, cria-os
            var hospedes = Array.Empty<Hospedes>();
            if (!dbContext.Hospedes.Any())
            {
                hospedes = [
                    new Hospedes{ Nome="Hospede 4", Telemovel="962938439", Avatar="40f50b9d-936b-4397-aa27-ae6003730f94.png", DataNascimento=DateOnly.Parse("1991-01-01"), NIF="716272993", UserId=users[2].Id },
                    new Hospedes{ Nome="Hospede 0", Telemovel="912123123", Avatar="c3269903-f5ac-4b20-b6ea-2a0eb5fd4ae7.png", DataNascimento=DateOnly.Parse("1994-06-04"), NIF="123321123", UserId=users[3].Id  }
                //adicionar outros hospedes
                ];
                await dbContext.Hospedes.AddRangeAsync(hospedes);
                haAdicao = true;
            }
            // Se não houver Gerentes, cria-os
            var gerentes = Array.Empty<Gerentes>();
            if (!dbContext.Servicos.Any())
            {
                gerentes = [
                   new Gerentes{ Nome="Joana Alves", Telemovel="967365380", Avatar="1fd1283e-0f9c-444f-bad6-bc71c41bec82.png", DataNascimento=DateOnly.Parse("1994-06-18"), NIF="213892766", UserId=users[0].Id },
                   new Gerentes{ Nome="Albertina", Telemovel="916781234", Avatar="26163c37-15e7-40fe-a208-fb4225373d9c.png", DataNascimento=DateOnly.Parse("1956-06-04"), NIF="123321123", UserId=users[1].Id },
                //adicionar outros Gerentes
                ];
                await dbContext.Gerentes.AddRangeAsync(gerentes);
                haAdicao = true;
            }
            // Se não houver Reccecionistas, cria-os
            var reccecionistas = Array.Empty<Reccecionistas>();
            if (!dbContext.Servicos.Any())
            {
                reccecionistas = [
                     new Reccecionistas{ Nome="Manuel António", Telemovel="917637655", Avatar="b56a727c-b366-4642-b62a-9514d40016dd.png", DataNascimento=DateOnly.Parse("1990-02-22"),  UserId=users[4].Id },
                     new Reccecionistas{ Nome="Mario", Telemovel="916782234", Avatar="084f5f1a-2531-4884-be01-6901ae28d6dd.png", DataNascimento=DateOnly.Parse("1953-06-04"), UserId=users[5].Id },
                //adicionar outros Reccecionistas
                ];
                await dbContext.Reccecionistas.AddRangeAsync(reccecionistas);
                haAdicao = true;
            }
            // Se não houver Reservas, cria-as
            var reservas = Array.Empty<Reservas>();
            if (!dbContext.Reservas.Any())
            {
                reservas = [
                    new Reservas{ValorPago=100, DataReserva=DateTime.Parse("2024-03-29T16:00:00.0000000"), DataCheckIN=DateTime.Parse("2024-06-28T11:56:00.0000000"), DataCheckOUT=DateTime.Parse("2024-07-12T11:56:00.0000000"), QuartoFK=quartos[0].Id, HospedeId= hospedes[0].Id, ValorTotal= 9870, ValorAPagar=8331, ListaServicos=[servicos[0],servicos[1]]},
                    new Reservas{ValorPago=500, DataReserva=DateTime.Parse("2024-06-24T19:00:00.0000000"), DataCheckIN=DateTime.Parse("2024-09-30T16:00:00.0000000"), DataCheckOUT=DateTime.Parse("2024-10-02T12:15:00.0000000"), QuartoFK=quartos[1].Id, HospedeId= hospedes[1].Id, ValorTotal= 721, ValorAPagar=21,ListaServicos=[servicos[0]] },
                ];
                await dbContext.Reservas.AddRangeAsync(reservas);
                haAdicao = true;
            }


            try
            {
                if (haAdicao)
                {
                    // tornar persistentes os dados
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw;
            }



        }
    }
}
