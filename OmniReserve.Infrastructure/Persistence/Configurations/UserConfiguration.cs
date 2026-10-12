using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniReserve.Domain.Entities;
using OmniReserve.Domain.ValueObjects;

namespace OmniReserve.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        // Value Conversion: El puente entre el Dominio complejo y la BD simple
        builder.Property(user => user.Email)
               .HasConversion(
                   emailObj => emailObj.Value, // Cómo grabar (C# a SQL)
                   emailStr => new EmailAddress(emailStr) // Cómo leer (SQL a C#)
               )
               .HasColumnName("Email") // Fuerza el nombre de la columna en la BD
                .HasMaxLength(255) // Limita la longitud en la BD
                .IsRequired(); // No permite nulos en la BD

        // Opcional recomendado: El email debe ser único en la BD
        builder.HasIndex(user => user.Email)
               .IsUnique();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(150);
    }
}