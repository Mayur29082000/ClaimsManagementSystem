
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Linq;

namespace ClaimsSystems_DAL.Models;

public partial class ClaimsSystemsDbContext : DbContext
{
    public ClaimsSystemsDbContext()
    {
    }

    public ClaimsSystemsDbContext(DbContextOptions<ClaimsSystemsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Claim> Claims { get; set; }

    public virtual DbSet<ClaimHistory> ClaimHistories { get; set; }

    public virtual DbSet<ClaimScoreAlcon> ClaimScoreAlcons { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Policy> Policies { get; set; }

    public virtual DbSet<Receipt> Receipts { get; set; }

    public virtual DbSet<ScoreTeam> ScoreTeams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
=> optionsBuilder.UseSqlServer(
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build()
            .GetConnectionString("QuickKartDBConnectionString"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasKey(e => e.ClaimId).HasName("PK__claims__F9CC08967D52DE60");

            entity.ToTable("claims", tb => tb.HasTrigger("trg_handle_collection_claim"));

            entity.Property(e => e.ClaimId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("claim_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.ClaimType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("claim_type");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.PolicyId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("policy_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("In Processing")
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("submitted_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Policy).WithMany(p => p.Claims)
                .HasForeignKey(d => d.PolicyId)
                .HasConstraintName("FK__claims__policy_i__2F10007B");
        });

        modelBuilder.Entity<ClaimHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__claim_hi__096AA2E9661C25AE");

            entity.ToTable("claim_history");

            entity.Property(e => e.HistoryId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("history_id");
            entity.Property(e => e.ClaimId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("claim_id");
            entity.Property(e => e.Comment)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("Payment Done")
                .HasColumnName("comment");
            entity.Property(e => e.DateOfClosed)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("date_of_closed");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Completed")
                .HasColumnName("status");

            entity.HasOne(d => d.Claim).WithMany(p => p.ClaimHistories)
                .HasForeignKey(d => d.ClaimId)
                .HasConstraintName("FK__claim_his__claim__49C3F6B7");
        });

        modelBuilder.Entity<ClaimScoreAlcon>(entity =>
        {
            entity.HasKey(e => e.AlconId).HasName("PK__claim_sc__1BF4FD46D67266E4");

            entity.ToTable("claim_score_alcon");

            entity.Property(e => e.AlconId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("alcon_id");
            entity.Property(e => e.ClaimId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("claim_id");
            entity.Property(e => e.Comments)
                .HasColumnType("text")
                .HasColumnName("comments");
            entity.Property(e => e.Decision)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("decision");
            entity.Property(e => e.ReviewedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("reviewed_at");
            entity.Property(e => e.ReviewerId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("reviewer_id");
            entity.Property(e => e.RiskScore).HasColumnName("risk_score");

            entity.HasOne(d => d.Claim).WithMany(p => p.ClaimScoreAlcons)
                .HasForeignKey(d => d.ClaimId)
                .HasConstraintName("FK__claim_sco__claim__36B12243");

            entity.HasOne(d => d.Reviewer).WithMany(p => p.ClaimScoreAlcons)
                .HasForeignKey(d => d.ReviewerId)
                .HasConstraintName("FK__claim_sco__revie__37A5467C");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__customer__CD65CB85649BF8FE");

            entity.ToTable("customers");

            entity.Property(e => e.CustomerId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("customer_id");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__payments__ED1FC9EA0C9A626A");

            entity.ToTable("payments", tb => tb.HasTrigger("trg_create_receipt"));

            entity.Property(e => e.PaymentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("payment_id");
            entity.Property(e => e.AmountPaid)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("amount_paid");
            entity.Property(e => e.ClaimId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("claim_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("notes");
            entity.Property(e => e.PayerType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("payer_type");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("payment_date");

            entity.HasOne(d => d.Claim).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ClaimId)
                .HasConstraintName("FK__payments__claim___3D5E1FD2");
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.PolicyId).HasName("PK__policies__47DA3F03CDE355AE");

            entity.ToTable("policies", tb => tb.HasTrigger("trg_set_is_active"));

            entity.Property(e => e.PolicyId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("policy_id");
            entity.Property(e => e.CoverageAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("coverage_amount");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("customer_id");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IssueDate).HasColumnName("issue_date");
            entity.Property(e => e.PolicyNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("policy_number");
            entity.Property(e => e.PolicyType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("policy_type");

            entity.HasOne(d => d.Customer).WithMany(p => p.Policies)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__policies__custom__276EDEB3");
        });

        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.HasKey(e => e.ReceiptId).HasName("PK__receipts__91F52C1FD5D3125C");

            entity.ToTable("receipts");

            entity.Property(e => e.ReceiptId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("receipt_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BuyerFees)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("buyer_fees");
            entity.Property(e => e.ClaimId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("claim_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PaymentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("payment_id");
            entity.Property(e => e.RemainingAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("remaining_amount");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Claim).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.ClaimId)
                .HasConstraintName("FK__receipts__claim___440B1D61");

            entity.HasOne(d => d.Payment).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK__receipts__paymen__4316F928");
        });

        modelBuilder.Entity<ScoreTeam>(entity =>
        {
            entity.HasKey(e => e.ReviewerId).HasName("PK__score_te__443D5A07767AA9D3");

            entity.ToTable("score_team");

            entity.Property(e => e.ReviewerId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("reviewer_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Role)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
