using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Projeto.Models;

namespace Projeto.Data
{

    /// <summary>
    /// class that represents new User data
    /// </summary>
    public class ApplicationUser : IdentityUser
    {

        /// <summary>
        /// personal name of user to be used at interface
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// registration date
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; }


    }


    /// <summary>
    /// this class connects our project with database
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        /// <summary>
        /// it executes code before the creation of model
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // imports the previous execution of this method
            base.OnModelCreating(modelBuilder);

            //*****************************************
            // add, at this point, your new code

            // seed the Roles data
            modelBuilder.Entity<IdentityRole>().HasData(
              new IdentityRole { Id = "u", Name = "User", NormalizedName = "User" },
              new IdentityRole { Id = "a", Name = "Admin", NormalizedName = "Admin" }
              );


        }

        // define table on the database
        public DbSet<Newsletter> Newsletters { get; set; }
        public DbSet<Universidade> Universidades { get; set; }
        public DbSet<Estudante> Estudantes { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

    }
}