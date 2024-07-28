using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityManagerServerApi.Data
{
    public class AppDbContext(DbContextOptions options) : IdentityDbContext(options)        //meka extend kale ms.aspnetcore.identity.entityframeworkcore package eka harahai
    {
        //public AppDbContext(DbContextOptions options): base(options) 
        //{ 
        //} mekedi api hdla thiynne constructor ekk , me constructor eka eka paarama class definition ekema hdnna puluwan dan me class eka hdla thiynne e widiyat 
        // e jathiye constructor ekkt kiynne primary constructor kiyla
    }
}
