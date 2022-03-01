using System;
using System.Collections.Generic;
using System.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AccountingSystem.Models
{
    public partial class DmdCoursDBContext : DbContext
    {
        public DmdCoursDBContext()
        {
        }

        public DmdCoursDBContext(DbContextOptions<DmdCoursDBContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public virtual DbSet<Position> Positions { get; set; } = null!;
        public virtual DbSet<Request> Requests { get; set; } = null!;
        public virtual DbSet<RequestStatus> RequestStatuses { get; set; } = null!;
        public virtual DbSet<RequestType> RequestTypes { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Unit> Units { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestStatus>().HasData(
                new RequestStatus { Id = 1, Name = "Новая" },
                new RequestStatus { Id = 2, Name = "Отправлена на согласование" },
                new RequestStatus { Id = 3, Name = "Согласована" },
                new RequestStatus { Id = 4, Name = "Не согласована" },
                new RequestStatus { Id = 5, Name = "Утверждена" },
                new RequestStatus { Id = 6, Name = "Не утверждена" },
                new RequestStatus { Id = 7, Name = "Отозвана" },
                new RequestStatus { Id = 8, Name = "Отменена" }
            );
            modelBuilder.Entity<RequestType>().HasData(
                new RequestType { Id = 1, Name = "Выходной в счет отпуска", Paid = true },
                new RequestType { Id = 2, Name = "Выходной в счет отработки", Paid = false },
                new RequestType { Id = 3, Name = "Отгул", Paid = false },
                new RequestType { Id = 4, Name = "Основной оплачиваемый отпуск", Paid = true },
                new RequestType { Id = 5, Name = "Удаленная работа", Paid = true },
                new RequestType { Id = 6, Name = "Отпуск без сохранения заработной платы", Paid = false },
                new RequestType { Id = 7, Name = "Декретный отпуск", Paid = true },
                new RequestType { Id = 8, Name = "Отпуск по уходу за ребенком", Paid = true }
            );
            modelBuilder.Entity<Position>().HasData(new Position
            {
                Id = 1,
                Name = "Младший программист"
            });
            modelBuilder.Entity<Unit>().HasData(new Unit
            {
                Id = 1,
                Name = "Отдел разработки",
            }, new
            {
                Id = 2,
                Name = "Отдел тестирования"
            });
            modelBuilder.Entity<Role>().HasData(new
            {
                Id = 1,
                Name = "Сотрудник",
            }, new
            {
                Id = 2,
                Name = "Руководитель"
            }, new
            {
                Id = 3,
                Name = "Директор"
            }, new
            {
                Id = 4,
                Name = "Администратор",
            });

            modelBuilder.Entity<User>().HasData(new
            {
                Id = 1,
                Name = "Admin",
                Surname = "Admin",
                Password = Crypto.HashPassword("123"),
                Position = 1,
                Unit = 1,
                PhoneNumber = "8-999-999-9999",
                Email = "Admin",
                UnusedVacationDays = 20,
            });
            modelBuilder
    .Entity<User>()
    .HasMany(p => p.Roles)
    .WithMany(p => p.Users)
    .UsingEntity(j => j.HasData(new { RoleId = 4, UserId = 1 }));

            modelBuilder.Entity<Position>(entity =>
            {
                entity.ToTable("position");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(350)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.ToTable("request");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment)
                    .HasMaxLength(500)
                    .HasColumnName("comment");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("date")
                    .HasColumnName("creation_date");

                entity.Property(e => e.PortabilityDate).HasColumnName("portability_date");

                entity.Property(e => e.RequestType).HasColumnName("request_type");

                entity.Property(e => e.Sender).HasColumnName("sender");

                entity.Property(e => e.SendingDate)
                    .HasColumnType("date")
                    .HasColumnName("sending_date");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.SubstituteEmployee).HasColumnName("substitute_employee");

                entity.HasOne(d => d.RequestTypeNavigation)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.RequestType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_request_request_type");

                entity.HasOne(d => d.SenderNavigation)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.Sender)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_request_user");

                entity.HasOne(d => d.StatusNavigation)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.Status)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_request_request_status");
            });

            modelBuilder.Entity<RequestStatus>(entity =>
            {
                entity.ToTable("request_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<RequestType>(entity =>
            {
                entity.ToTable("request_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .HasColumnName("name");

                entity.Property(e => e.Paid).HasColumnName("paid");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("unit");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(350)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .HasMaxLength(500)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .HasColumnName("password");

                entity.Property(e => e.Patronymic)
                    .HasMaxLength(500)
                    .HasColumnName("patronymic");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(15)
                    .HasColumnName("phone_number");

                entity.Property(e => e.UnusedVacationDays).HasColumnName("unused_vacation_days");

                entity.Property(e => e.Position).HasColumnName("position");

                entity.Property(e => e.Surname)
                    .HasMaxLength(500)
                    .HasColumnName("surname");

                entity.Property(e => e.Unit).HasColumnName("unit");

                entity.HasOne(d => d.PositionNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Position)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user_position");

                entity.HasOne(d => d.UnitNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Unit)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user_unit");

                entity.HasMany(d => d.RequestsNavigation)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "RequestApprover",
                        l => l.HasOne<Request>().WithMany().HasForeignKey("RequestId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_request_approver_request"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_request_approver_user"),
                        j =>
                        {
                            j.HasKey("UserId", "RequestId");

                            j.ToTable("request_approver");

                            j.IndexerProperty<int>("UserId").HasColumnName("user_id");

                            j.IndexerProperty<int>("RequestId").HasColumnName("request_id");
                        });

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserRole",
                        l => l.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_user_role_role"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_user_role_user"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");

                            j.ToTable("user_role");

                            j.IndexerProperty<int>("UserId").HasColumnName("user_id");

                            j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
