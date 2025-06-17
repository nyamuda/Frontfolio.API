namespace Frontfolio.API.Data;

using Frontfolio.API.Models;
using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext:DbContext
    {

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


    public DbSet<User> Users { get; set; } = default!;

    public DbSet<UserOtp> UserOtps { get; set; } = default!;



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //A User can have multiple OTPs and an OTP can only have one User
        //So, there is a one-many relationship between User and UserOtp
        modelBuilder.Entity<User>()
            .HasMany(u => u.UserOtps)
            .WithOne(uo => uo.User)
            .HasForeignKey(uo => uo.UserId)
            .OnDelete(DeleteBehavior.Cascade); //Delete User -> delete UserOtps for that user


        //A Project can have many Paragraphs and a paragraph can only belong to one Project
        //So, there is a one-many relationship between Project and Paragraph
        modelBuilder.Entity<Project>()
            .HasMany(proj => proj.FullDescription)
            .WithOne(par => par.Project)
            .HasForeignKey(par => par.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); //delete Project ->  delete Paragraphs for that project

        //A Project can only have one User and a User can have multiple projects
        //So, there is a many-one relationship between Project and User
        modelBuilder.Entity<Project>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade); //delete User ->  delete Projects for that user

    }


}

