using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace ControleDeCinema.Testes.Integracao.Compartilhado;

public static class TestDbContextFactory
{
    public static ControleDeCinemaDbContext CriarDbContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<ControleDeCinemaDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        var dbContext = new ControleDeCinemaDbContext(options);

        return dbContext;
    }
}

