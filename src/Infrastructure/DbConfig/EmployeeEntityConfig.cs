using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DbConfig
{
    internal class EmployeeEntityConfig : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder
                .ToTable("Employees", SchemaNames.HR)
                .HasIndex(e => e.FirstName)
                .HasDatabaseName("IX_Employees_FirstName");

            builder
                .HasIndex(e => e.LastName)
                .HasDatabaseName("IX_Employees_LastName");
        }
    }
}
