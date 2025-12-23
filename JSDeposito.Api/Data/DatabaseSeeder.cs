using JSDeposito.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace JSDeposito.Api.Data;

public static class DatabaseSeeder
{
    public static void Seed(AppDbContext context)
    {
        context.Database.Migrate();

        if (!context.Usuarios.Any(u => u.Email == "admin@jsdeposito.com"))
        {
            var admin = new Usuario(
                nome: "Administrador",
                email: "admin@jsdeposito.com",
                telefone: "79988012359",
                senhaHash: BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                role: "Admin"
            );

            context.Usuarios.Add(admin);
        }

        if (!context.Usuarios.Any(u => u.Email == "cliente@jsdeposito.com"))
        {
            var cliente = new Usuario(
                nome: "Cliente Teste",
                email: "cliente@jsdeposito.com",
                telefone: "79988012359",
                senhaHash: BCrypt.Net.BCrypt.HashPassword("Cliente@123"),
                role: "Cliente"
            );

            context.Usuarios.Add(cliente);
        }

        context.SaveChanges();
    }
}
