using Microsoft.EntityFrameworkCore;

namespace projectTest.Models
{
    public partial class dbContext : DbContext
    {
        public dbContext(DbContextOptions<dbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PartTime_order> PartTime_order { get; set; }
        public virtual DbSet<FullTime_order> FullTime_order { get; set; }
        public virtual DbSet<FullTime_order_detail> FullTime_order_detail { get; set; }
        public virtual DbSet<ManagerAuthority> ManagerAuthority { get; set; }
        public virtual DbSet<ProjectBudget> ProjectBudget { get; set; }
        public virtual DbSet<DepBudget> DepBudget { get; set; }
        public virtual DbSet<Studentinfo> Studentinfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PartTime_order>(entity =>
            {
                entity.HasKey(e => e.PartTime_orderNo);

                entity.Property(e => e.PartTime_orderNo).HasMaxLength(10);
                entity.Property(e => e.ApplicantName).IsRequired().HasMaxLength(30);
                entity.Property(e => e.ApplicationOffice).IsRequired().HasMaxLength(60);
                entity.Property(e => e.ApplicationTime).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.Application).IsRequired().HasMaxLength(5);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(15);
                entity.Property(e => e.PlanName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.AccountNo).IsRequired().HasMaxLength(15);
                entity.Property(e => e.PlanNo).IsRequired(false).HasMaxLength(30);
                entity.Property(e => e.CommissionName).IsRequired(false).HasMaxLength(60);
                entity.Property(e => e.PlanStartDate).IsRequired(false).HasMaxLength(10);
                entity.Property(e => e.PlanEndDate).IsRequired(false).HasMaxLength(10);
                entity.Property(e => e.LeaderName).IsRequired(false).HasMaxLength(30);
                entity.Property(e => e.EmployedName).IsRequired().HasMaxLength(30);
                entity.Property(e => e.ID).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Birthday).IsRequired().HasMaxLength(10);
                entity.Property(e => e.EmployedStartDate).IsRequired().HasMaxLength(10);
                entity.Property(e => e.EmployedEndDate).IsRequired().HasMaxLength(10);
                entity.Property(e => e.phone).IsRequired().HasMaxLength(10);
                entity.Property(e => e.StudyStatus).IsRequired().HasMaxLength(10);
                entity.Property(e => e.StudyStatusMemo).IsRequired(false).HasMaxLength(60);
                entity.Property(e => e.EducationLevel).IsRequired().HasMaxLength(10);
                entity.Property(e => e.SalaryCategory).IsRequired().HasMaxLength(5);
                entity.Property(e => e.Salary).IsRequired().HasMaxLength(10);
                entity.Property(e => e.IsTechPerson).IsRequired().HasMaxLength(5);
                entity.Property(e => e.ResignDate).IsRequired(false).HasMaxLength(10);
                entity.Property(e => e.SalaryChangeDate).IsRequired(false).HasMaxLength(10);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(5);
                entity.Property(e => e.isApplyRenewal).IsRequired().HasMaxLength(5).HasDefaultValue(0);
                entity.Property(e => e.isApplySalaryChange).IsRequired().HasMaxLength(5).HasDefaultValue(0);
                entity.Property(e => e.isApplyResignation).IsRequired().HasMaxLength(5).HasDefaultValue(0);
                entity.Property(e => e.projtype).IsRequired(false).HasMaxLength(5);

            });

            modelBuilder.Entity<FullTime_order>(entity =>
            {
                entity.HasKey(e => e.FullTime_orderNo);

                entity.Property(e => e.FullTime_orderNo).HasMaxLength(10);
                entity.Property(e => e.ApplicantName).IsRequired().HasMaxLength(30);
                entity.Property(e => e.ApplicationOffice).IsRequired().HasMaxLength(60);
                entity.Property(e => e.ApplicationTime).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.Application).IsRequired().HasMaxLength(5);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(15);
                entity.Property(e => e.EmployedName).IsRequired().HasMaxLength(30);
                entity.Property(e => e.ID).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Birthday).IsRequired().HasMaxLength(10);
                entity.Property(e => e.EmployedStartDate).IsRequired().HasMaxLength(10);
                entity.Property(e => e.EmployedEndDate).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(10);
                entity.Property(e => e.EducationLevel).IsRequired().HasMaxLength(5);
                entity.Property(e => e.JobTitle).IsRequired().HasMaxLength(10);
                entity.Property(e => e.SalaryGrade).IsRequired(false).HasMaxLength(5);
                entity.Property(e => e.Salary).IsRequired().HasMaxLength(10);
                entity.Property(e => e.SalaryChangeReason).IsRequired(false).HasMaxLength(200);
                entity.Property(e => e.ResignDate).IsRequired(false).HasMaxLength(10);
                entity.Property(e => e.SalaryChangeDate).IsRequired(false).HasMaxLength(10);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(5);
                entity.Property(e => e.isApplyRenewal).IsRequired().HasMaxLength(5).HasDefaultValue(0);
                entity.Property(e => e.isApplySalaryChange).IsRequired().HasMaxLength(5).HasDefaultValue(0);
                entity.Property(e => e.isApplyResignation).IsRequired().HasMaxLength(5).HasDefaultValue(0);
                

                entity.HasMany(e => e.OrderDetails)
                      .WithOne(d => d.FullTimeOrder)
                      .HasForeignKey(d => d.FullTime_orderNo)
                      .HasConstraintName("FK_FullTime_Order_detail_FullTime_Order")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<FullTime_order_detail>(entity =>
            {
                entity.HasKey(e => new { e.FullTime_orderNo, e.AccountNo });
                entity.Property(e => e.FullTime_orderNo).IsRequired().HasMaxLength(10);
                entity.Property(e => e.AccountNo).IsRequired().HasMaxLength(15);
                entity.Property(e => e.PlanName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PlanNo).IsRequired(false).HasMaxLength(30);
                entity.Property(e => e.CommissionName).IsRequired(false).HasMaxLength(60);
                entity.Property(e => e.PlanStartDate).IsRequired(false).HasMaxLength(10);
                entity.Property(e => e.PlanEndDate).IsRequired(false).HasMaxLength(10);
                entity.Property(e => e.LeaderName).IsRequired(false).HasMaxLength(30);
                entity.Property(e => e.projtype).IsRequired(false).HasMaxLength(5);
            });

            modelBuilder.Entity<ManagerAuthority>(entity =>
            {
                entity.HasKey(e => new { e.Authority, e.EmployeeNo });
                entity.Property(e => e.EmployeeNo).IsRequired().HasMaxLength(4);
                entity.Property(e => e.Authority).IsRequired().HasMaxLength(5);
            });

            modelBuilder.Entity<ProjectBudget>().HasNoKey();
            modelBuilder.Entity<DepBudget>().HasNoKey();
            modelBuilder.Entity<Studentinfo>().HasNoKey();
        }
    }
}
