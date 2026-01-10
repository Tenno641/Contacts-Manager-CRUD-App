using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ContactsManager.Infrastructure;
public class PersonsDbContext : IdentityDbContext<User, Role, Guid>
{
    public PersonsDbContext(DbContextOptions<PersonsDbContext> dbContextOptions) : base(dbContextOptions) { }
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<Person> Persons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>().ToTable("Countries");

        modelBuilder.Entity<Person>().ToTable("Persons");

        //foreach (Country country in GetCountriesListFromJson())
        //{
        //    modelBuilder.Entity<Country>().HasData(country);
        //}
        //foreach (Person person in GetPersonsFromJson())
        //{
        //    modelBuilder.Entity<Person>().HasData(person);
        //}

        modelBuilder.Entity<Person>()
            .HasOne(person => person.Country)
            .WithMany(country => country.Persons)
            .HasForeignKey(person => person.CountryId);

        modelBuilder.Entity<Country>()
            .HasIndex(country => country.Name)
            .IsUnique();

        modelBuilder.Entity<Country>()
            .Property(country => country.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Person>()
            .Property(person => person.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<User>()
            .Property(user => user.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Role>()
            .Property(role => role.Id)
            .ValueGeneratedNever();
    }
    public IEnumerable<Person> GetAllPersonsStoredProcedure()
    {
        FormattableString query = FormattableStringFactory.Create("EXECUTE [dbo].[PersonsGet]");
        return Persons.FromSql(query);
    }

    public void InsertPersonStoredProcedure(Person person)
    {
        SqlParameter[] sqlParameters = new SqlParameter[]
        {
            new("@Id", person.Id),
            new("@Name", person.Name),
            new("@Email", person.Email),
            new("@DateOfBirth", person.DateOfBirth),
            new("@Gender", person.Gender),
            new("@CountryId", person.CountryId),
            new("@Address", person.Address),
            new("@ReceiveNewsLetter", person.ReceiveNewsLetter)
        };
        Database.ExecuteSqlRaw("EXECUTE [dbo].[PersonInsert] @Id, @Name, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsLetter", sqlParameters);
    }

    List<Person> GetPersonsFromJson()
    {
        var fileContent = File.ReadAllText("persons.json");
        List<Person>? persons = JsonSerializer.Deserialize<List<Person>>(fileContent);
        return persons ?? [];
    }

    List<Country> GetCountriesListFromJson()
    {
        string fileContent = File.ReadAllText("countries.json");
        List<Country>? countries = JsonSerializer.Deserialize<List<Country>>(fileContent);
        return countries ?? [];
    }

}
