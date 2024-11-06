using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketService;

namespace Tests;

public static class DbHelper
{
    public static T CreateContext<T>() where T : DbContext
    {
        var builder = new DbContextOptionsBuilder<T>();
        builder.UseInMemoryDatabase(Guid.NewGuid().ToString());

        return (T)Activator.CreateInstance(typeof(T), builder.Options)!;
    }
}
