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

        //Save User Role enum value as a string instead of an int 

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
            .HasMany(proj => proj.Background)
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

        //A Project can only have many Challenges and a Challenge can only be for one Project
        //So, there is a one-many relationship between Project and Challenge
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Challenges)
            .WithOne(c => c.Project)
            .HasForeignKey(c => c.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  //delete Project ->  delete Challenges for that Project

        //A Project can only have many Achievements and an Achievement can only be for one Project
        //So, there is a one-many relationship between Project and Achievement
        modelBuilder.Entity<Achievement>()
            .HasOne(a => a.Project)
            .WithMany(p => p.Achievements)
            .HasForeignKey(a => a.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); //delete Project ->  delete Achievements for that Project

        //A Project can only have many Feedback and Feedback can only be for one Project
        //So, there is a one-many relationship between Project and Feedback
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Feedback)
            .WithOne(f => f.Project)
            .HasForeignKey(f => f.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); //delete Project ->  delete Feedback for that Project



    }


}

