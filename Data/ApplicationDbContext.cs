namespace Frontfolio.API.Data;
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
        //So, there is a many-one relationship between User and UserOtp
        modelBuilder.Entity<User>()
            .HasMany(u => u.UserOtps)
            .WithOne(uo => uo.User)
            .HasForeignKey(uo => uo.UserId)
            .OnDelete(DeleteBehavior.Cascade); //Delete User -> Delete UserOtps for that user



    }


}

