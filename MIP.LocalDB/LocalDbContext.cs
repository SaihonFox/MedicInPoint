using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Principal;

using Microsoft.EntityFrameworkCore;

using MIP.Base.Models;
using MIP.LocalDB.Models;

namespace MIP.LocalDB;

public class LocalDbContext : DbContext
{
	private string dbPath = "";

	public IQueryable<T> SqlQuery<T>(string sql, params object[] parameters) => Database.SqlQueryRaw<T>(sql, parameters);

	public LocalDbContext(bool useCustomDirectory = true, DirectoryInfo? directory = null)
	{
		Database.EnsureCreated();

		if (!useCustomDirectory)
			directory ??= new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/MedicInPoint");
		else
			directory ??= new DirectoryInfo(Environment.CurrentDirectory + "/MedicInPoint");
		Directory.CreateDirectory(directory.FullName);
		dbPath = directory.FullName;
	}

	#region Admin
	public virtual DbSet<AnalysesAdminSearch> AnalysesAdminSearches { get; set; }

	public virtual DbSet<AnalysisCategoriesAdminSearch> AnalysisCategoriesAdminSearches { get; set; }

	public virtual DbSet<PatientsAdminSearch> PatientsAdminSearches { get; set; }

	public virtual DbSet<UsersAdminSearch> UsersAdminSearches { get; set; }
	#endregion Admin

	#region Doctor
	public virtual DbSet<PatientDoctorSearch> PatientDoctorSearches { get; set; }

	public virtual DbSet<RnRDoctorSearch> RnRDoctorSearches { get; set; }
	#endregion Doctor

	public virtual DbSet<AdBlock> AdBlocks { get; set; }

	public virtual DbSet<Analysis> Analyses { get; set; }

	public virtual DbSet<AnalysisCategoriesList> AnalysisCategoriesLists { get; set; }

	public virtual DbSet<AnalysisCategory> AnalysisCategories { get; set; }

	public virtual DbSet<AnalysisOrder> AnalysisOrders { get; set; }

	public virtual DbSet<Message> Messages { get; set; }

	public virtual DbSet<MessagesMessage> MessagesMessages { get; set; }

	public virtual DbSet<MmFile> MmFiles { get; set; }

	public virtual DbSet<MmFileType> MmFileTypes { get; set; }

	public virtual DbSet<Patient> Patients { get; set; }

	public virtual DbSet<PatientAnalysisAddress> PatientAnalysisAddresses { get; set; }

	public virtual DbSet<PatientAnalysisCart> PatientAnalysisCarts { get; set; }

	public virtual DbSet<PatientAnalysisCartItem> PatientAnalysisCartItems { get; set; }

	public virtual DbSet<PatientDatum> PatientData { get; set; }

	public virtual DbSet<PatientsDataList> PatientsDataLists { get; set; }

	public virtual DbSet<Request> Requests { get; set; }

	public virtual DbSet<RequestAnalysis> RequestAnalyses { get; set; }

	public virtual DbSet<RequestState> RequestStates { get; set; }

	public virtual DbSet<User> Users { get; set; }

	public virtual DbSet<UserLastEnter> UserLastEnters { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite($"Data Source={Path.Combine(dbPath, "local.db")}");
		optionsBuilder.UseLazyLoadingProxies();
	}

	[SupportedOSPlatform("windows")]
	void SetFolderPermissions(string folderPath)
	{
		// Получаем текущего пользователя
		var currentUser = WindowsIdentity.GetCurrent();
		var currentPrincipal = new WindowsPrincipal(currentUser);

		// Создаем объект DirectorySecurity для папки
		var directorySecurity = new DirectoryInfo(folderPath).GetAccessControl();

		// Удаляем все существующие правила доступа
		directorySecurity.SetAccessRuleProtection(true, false);

		// Создаем правило доступа для текущего пользователя
		var accessRule = new FileSystemAccessRule(
			currentUser.Name,
			FileSystemRights.FullControl,
			InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
			PropagationFlags.None,
			AccessControlType.Allow
		);

		// Добавляем правило доступа
		directorySecurity.AddAccessRule(accessRule);

		// Применяем новые права доступа к папке
		new DirectoryInfo(folderPath).SetAccessControl(directorySecurity);
	}

	[SupportedOSPlatform("windows")]
	public class CurrentUserSecurity
	{
		WindowsIdentity _currentUser;
		WindowsPrincipal _currentPrincipal;

		public CurrentUserSecurity()
		{
			_currentUser = WindowsIdentity.GetCurrent();
			_currentPrincipal = new WindowsPrincipal(_currentUser);
		}

		public bool HasAccess(DirectoryInfo directory, FileSystemRights right)
		{
			// Get the collection of authorization rules that apply to the directory.
			AuthorizationRuleCollection acl = directory.GetAccessControl()
				.GetAccessRules(true, true, typeof(SecurityIdentifier));
			return HasFileOrDirectoryAccess(right, acl);
		}

		public bool HasAccess(FileInfo file, FileSystemRights right)
		{
			// Get the collection of authorization rules that apply to the file.
			AuthorizationRuleCollection acl = file.GetAccessControl()
				.GetAccessRules(true, true, typeof(SecurityIdentifier));
			return HasFileOrDirectoryAccess(right, acl);
		}

		private bool HasFileOrDirectoryAccess(FileSystemRights right,
											  AuthorizationRuleCollection acl)
		{
			bool allow = false;
			bool inheritedAllow = false;
			bool inheritedDeny = false;

			for (int i = 0; i < acl.Count; i++)
			{
				var currentRule = (FileSystemAccessRule)acl[i];
				// If the current rule applies to the current user.
				if (_currentUser.User.Equals(currentRule.IdentityReference) ||
					_currentPrincipal.IsInRole(
									(SecurityIdentifier)currentRule.IdentityReference))
				{

					if (currentRule.AccessControlType.Equals(AccessControlType.Deny))
					{
						if ((currentRule.FileSystemRights & right) == right)
						{
							if (currentRule.IsInherited)
							{
								inheritedDeny = true;
							}
							else
							{ // Non inherited "deny" takes overall precedence.
								return false;
							}
						}
					}
					else if (currentRule.AccessControlType
													  .Equals(AccessControlType.Allow))
					{
						if ((currentRule.FileSystemRights & right) == right)
						{
							if (currentRule.IsInherited)
							{
								inheritedAllow = true;
							}
							else
							{
								allow = true;
							}
						}
					}
				}
			}

			if (allow)
			{ // Non inherited "allow" takes precedence over inherited rules.
				return true;
			}
			return inheritedAllow && !inheritedDeny;
		}
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<AnalysesAdminSearch>(entity => {
			entity.ToTable("analyses_admin_search");
			entity.HasKey(a => a.id).HasName("primary");
			entity.Property(a => a.name).HasColumnName("name").HasColumnType("text").IsRequired();
		});

		modelBuilder.Entity<AnalysisCategoriesAdminSearch>(entity => {
			entity.ToTable("analysis_categories_admin_search");
			entity.HasKey(a => a.id).HasName("primary");
			entity.Property(a => a.name).HasColumnName("name").HasColumnType("text").IsRequired();
		});

		modelBuilder.Entity<PatientsAdminSearch>(entity => {
			entity.ToTable("patients_admin_search");
			entity.HasKey(a => a.id).HasName("primary");
			entity.Property(a => a.name).HasColumnName("name").HasColumnType("text").IsRequired();
		});

		modelBuilder.Entity<UsersAdminSearch>(entity => {
			entity.ToTable("users_admin_search");
			entity.HasKey(a => a.id).HasName("primary");
			entity.Property(a => a.id).HasColumnName("id");
			entity.Property(a => a.name).HasColumnName("name").HasColumnType("text").IsRequired();
		});

		#region MySQL
		modelBuilder.Entity<AdBlock>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("ad_block");

			entity.HasIndex(e => e.AnalysisId, "analysis_id");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.AnalysisId).HasColumnName("analysis_id");
			entity.Property(e => e.DateEnd)
				.HasColumnType("datetime")
				.HasColumnName("date_end");
			entity.Property(e => e.DateStart)
				.HasColumnType("datetime")
				.HasColumnName("date_start");
			entity.Property(e => e.Description)
				.HasColumnType("text")
				.HasColumnName("description");
			entity.Property(e => e.Name)
				.HasColumnType("text")
				.HasColumnName("name");
			entity.Property(e => e.Price)
				.HasPrecision(10, 2)
				.HasColumnName("price");
			entity.Property(e => e.Sex)
				.HasColumnType("text")
				.HasColumnName("sex");

			entity.HasOne(d => d.Analysis).WithMany(p => p.AdBlocks)
				.HasForeignKey(d => d.AnalysisId)
				.HasConstraintName("ad_block_ibfk_1");
		});

		modelBuilder.Entity<Analysis>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("analysis");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Biomaterial)
				.HasColumnType("text")
				.HasColumnName("biomaterial");
			entity.Property(e => e.Description)
				.HasColumnType("text")
				.HasColumnName("description");
			entity.Property(e => e.Name)
				.HasColumnType("text")
				.HasColumnName("name");
			entity.Property(e => e.Preparation)
				.HasColumnType("text")
				.HasColumnName("preparation");
			entity.Property(e => e.Price)
				.HasPrecision(10, 2)
				.HasColumnName("price");
			entity.Property(e => e.ResultsAfter)
				.HasColumnType("text")
				.HasColumnName("results_after");
		});

		modelBuilder.Entity<AnalysisCategoriesList>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("analysis_categories_list");

			entity.HasIndex(e => e.AnalysisCategoryId, "analysis_category_id");

			entity.HasIndex(e => e.AnalysisId, "analysis_id");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.AnalysisCategoryId).HasColumnName("analysis_category_id");
			entity.Property(e => e.AnalysisId).HasColumnName("analysis_id");

			entity.HasOne(d => d.AnalysisCategory).WithMany(p => p.AnalysisCategoriesLists)
				.HasForeignKey(d => d.AnalysisCategoryId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("analysis_categories_list_ibfk_2");

			entity.HasOne(d => d.Analysis).WithMany(p => p.AnalysisCategoriesLists)
				.HasForeignKey(d => d.AnalysisId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("analysis_categories_list_ibfk_1");
		});

		modelBuilder.Entity<AnalysisCategory>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("analysis_category");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Name)
				.HasColumnType("text")
				.HasColumnName("name");
		});

		modelBuilder.Entity<AnalysisOrder>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("analysis_order");

			entity.HasIndex(e => e.PatientAnalysisAddressId, "patient_analysis_address_id");

			entity.HasIndex(e => e.PatientAnalysisCartId, "patient_analysis_cart_id");

			entity.HasIndex(e => e.PatientId, "patient_id");

			entity.HasIndex(e => e.UserId, "user_id");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.AnalysisDatetime)
				.HasColumnType("datetime")
				.HasColumnName("analysis_datetime");
			entity.Property(e => e.Comment)
				.HasColumnType("text")
				.HasColumnName("comment");
			entity.Property(e => e.PatientAnalysisAddressId).HasColumnName("patient_analysis_address_id");
			entity.Property(e => e.PatientAnalysisCartId).HasColumnName("patient_analysis_cart_id");
			entity.Property(e => e.PatientId).HasColumnName("patient_id");
			entity.Property(e => e.RegistrationDate)
				.HasColumnType("datetime")
				.HasColumnName("registration_date");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.PatientAnalysisAddress).WithMany(p => p.AnalysisOrders)
				.HasForeignKey(d => d.PatientAnalysisAddressId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("analysis_order_ibfk_4");

			entity.HasOne(d => d.PatientAnalysisCart).WithMany(p => p.AnalysisOrders)
				.HasForeignKey(d => d.PatientAnalysisCartId)
				.HasConstraintName("analysis_order_ibfk_3");

			entity.HasOne(d => d.Patient).WithMany(p => p.AnalysisOrders)
				.HasForeignKey(d => d.PatientId)
				.HasConstraintName("analysis_order_ibfk_2");

			entity.HasOne(d => d.User).WithMany(p => p.AnalysisOrders)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("analysis_order_ibfk_1");
		});

		modelBuilder.Entity<Message>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("messages");

			entity.HasIndex(e => e.PatientId, "patient_id");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.PatientId).HasColumnName("patient_id");

			entity.HasOne(d => d.Patient).WithMany(p => p.Messages)
				.HasForeignKey(d => d.PatientId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("messages_ibfk_1");
		});

		modelBuilder.Entity<MessagesMessage>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("messages_message");

			entity.HasIndex(e => e.MessagesId, "messages_id");

			entity.HasIndex(e => e.PatientId, "patient_id");

			entity.HasIndex(e => e.UserId, "user_id");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Message)
				.HasColumnType("text")
				.HasColumnName("message");
			entity.Property(e => e.MessagesId).HasColumnName("messages_id");
			entity.Property(e => e.PatientId).HasColumnName("patient_id");
			entity.Property(e => e.Time)
				.HasColumnType("datetime")
				.HasColumnName("time");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.Messages).WithMany(p => p.MessagesMessages)
				.HasForeignKey(d => d.MessagesId)
				.HasConstraintName("messages_message_ibfk_1");

			entity.HasOne(d => d.Patient).WithMany(p => p.MessagesMessages)
				.HasForeignKey(d => d.PatientId)
				.HasConstraintName("messages_message_ibfk_3");

			entity.HasOne(d => d.User).WithMany(p => p.MessagesMessages)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("messages_message_ibfk_2");
		});

		modelBuilder.Entity<MmFile>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("mm_files");

			entity.HasIndex(e => e.MessagesMessageId, "messages_message_id");

			entity.HasIndex(e => e.MmFileTypeId, "mm_file_type_id");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.File).HasColumnName("file");
			entity.Property(e => e.FileExtension)
				.HasColumnType("text")
				.HasColumnName("file_extension");
			entity.Property(e => e.FileName)
				.HasColumnType("text")
				.HasColumnName("file_name");
			entity.Property(e => e.MessagesMessageId).HasColumnName("messages_message_id");
			entity.Property(e => e.MmFileTypeId).HasColumnName("mm_file_type_id");

			entity.HasOne(d => d.MessagesMessage).WithMany(p => p.MmFiles)
				.HasForeignKey(d => d.MessagesMessageId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("mm_files_ibfk_1");

			entity.HasOne(d => d.MmFileType).WithMany(p => p.MmFiles)
				.HasForeignKey(d => d.MmFileTypeId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("mm_files_ibfk_2");
		});

		modelBuilder.Entity<MmFileType>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("mm_file_type");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Name)
				.HasColumnType("text")
				.HasColumnName("name");
		});

		modelBuilder.Entity<Patient>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("patient");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Birthday).HasColumnName("birthday");
			entity.Property(e => e.Email)
				.HasColumnType("text")
				.HasColumnName("email");
			entity.Property(e => e.Image).HasColumnName("image");
			entity.Property(e => e.Login)
				.HasColumnType("text")
				.HasColumnName("login");
			entity.Property(e => e.Name)
				.HasColumnType("text")
				.HasColumnName("name");
			entity.Property(e => e.Passport)
				.HasMaxLength(10)
				.HasColumnName("passport");
			entity.Property(e => e.Password)
				.HasColumnType("text")
				.HasColumnName("password");
			entity.Property(e => e.Patronym)
				.HasColumnType("text")
				.HasColumnName("patronym");
			entity.Property(e => e.Phone)
				.HasMaxLength(11)
				.HasColumnName("phone");
			entity.Property(e => e.Sex)
				.HasColumnType("text")
				.HasColumnName("sex");
			entity.Property(e => e.Surname)
				.HasColumnType("text")
				.HasColumnName("surname");
		});

		modelBuilder.Entity<PatientAnalysisAddress>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("patient_analysis_address");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Address)
				.HasColumnType("text")
				.HasColumnName("address");
			entity.Property(e => e.Entrance).HasColumnName("entrance");
			entity.Property(e => e.Floor).HasColumnName("floor");
			entity.Property(e => e.Intercome)
				.HasColumnType("text")
				.HasColumnName("intercome");
			entity.Property(e => e.Room).HasColumnName("room");
		});

		modelBuilder.Entity<PatientAnalysisCart>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("patient_analysis_cart");

			entity.HasIndex(e => e.PatientId, "patient_id");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.PatientId).HasColumnName("patient_id");

			entity.HasOne(d => d.Patient).WithMany(p => p.PatientAnalysisCarts)
				.HasForeignKey(d => d.PatientId)
				.HasConstraintName("patient_analysis_cart_ibfk_1");
		});

		modelBuilder.Entity<PatientAnalysisCartItem>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("patient_analysis_cart_item");

			entity.HasIndex(e => e.AnalysisId, "analysis_id");

			entity.HasIndex(e => e.PatientAnalysisCartId, "patient_analysis_cart_id");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.AnalysisId).HasColumnName("analysis_id");
			entity.Property(e => e.PatientAnalysisCartId).HasColumnName("patient_analysis_cart_id");

			entity.HasOne(d => d.Analysis).WithMany(p => p.PatientAnalysisCartItems)
				.HasForeignKey(d => d.AnalysisId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("patient_analysis_cart_item_ibfk_1");

			entity.HasOne(d => d.PatientAnalysisCart).WithMany(p => p.PatientAnalysisCartItems)
				.HasForeignKey(d => d.PatientAnalysisCartId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("patient_analysis_cart_item_ibfk_2");
		});

		modelBuilder.Entity<PatientDatum>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("patient_data");

			entity.HasIndex(e => e.PatientId, "patient_id");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Height).HasColumnName("height");
			entity.Property(e => e.PatientId).HasColumnName("patient_id");
			entity.Property(e => e.Weight).HasColumnName("weight");

			entity.HasOne(d => d.Patient).WithMany(p => p.PatientData)
				.HasForeignKey(d => d.PatientId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("patient_data_ibfk_1");
		});

		modelBuilder.Entity<PatientsDataList>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("patients_data_list");

			entity.HasIndex(e => e.PatientId, "patient_id");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.ChangeDt)
				.HasColumnType("datetime")
				.HasColumnName("change_dt");
			entity.Property(e => e.Email)
				.HasColumnType("text")
				.HasColumnName("email");
			entity.Property(e => e.Height).HasColumnName("height");
			entity.Property(e => e.Login)
				.HasMaxLength(20)
				.HasColumnName("login");
			entity.Property(e => e.Method)
				.HasComment("insert or update")
				.HasColumnType("text")
				.HasColumnName("method");
			entity.Property(e => e.Name)
				.HasColumnType("text")
				.HasColumnName("name");
			entity.Property(e => e.Passport)
				.HasMaxLength(10)
				.HasColumnName("passport");
			entity.Property(e => e.PatientId).HasColumnName("patient_id");
			entity.Property(e => e.Patronym)
				.HasColumnType("text")
				.HasColumnName("patronym");
			entity.Property(e => e.Phone)
				.HasMaxLength(11)
				.HasColumnName("phone");
			entity.Property(e => e.Surname)
				.HasColumnType("text")
				.HasColumnName("surname");
			entity.Property(e => e.Weight).HasColumnName("weight");

			entity.HasOne(d => d.Patient).WithMany(p => p.PatientsDataLists)
				.HasForeignKey(d => d.PatientId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("patients_data_list_ibfk_1");
		});

		modelBuilder.Entity<Request>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("request");

			entity.HasIndex(e => e.DoctorId, "doctor_id");

			entity.HasIndex(e => e.PatientAnalysisAddressId, "patient_analysis_address_id");

			entity.HasIndex(e => e.PatientId, "patient_id");

			entity.HasIndex(e => e.RequestStateId, "request_state_id");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.AnalysisDatetime)
				.HasColumnType("datetime")
				.HasColumnName("analysis_datetime");
			entity.Property(e => e.PatientComment)
				.HasColumnType("text")
				.HasColumnName("comment");
			entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
			entity.Property(e => e.PatientAnalysisAddressId).HasColumnName("patient_analysis_address_id");
			entity.Property(e => e.PatientId).HasColumnName("patient_id");
			entity.Property(e => e.RequestChanged)
				.HasComment("Время изменения запроса на запись")
				.HasColumnType("datetime")
				.HasColumnName("request_changed");
			entity.Property(e => e.RequestSended)
				.HasComment("Время создания запроса на запись")
				.HasColumnType("datetime")
				.HasColumnName("request_sended");
			entity.Property(e => e.RequestStateId)
				.HasComment("true - request accepted\r\nfalse - request declined")
				.HasColumnName("request_state_id");

			entity.HasOne(d => d.Doctor).WithMany(p => p.Requests)
				.HasForeignKey(d => d.DoctorId)
				.HasConstraintName("request_ibfk_1");

			entity.HasOne(d => d.PatientAnalysisAddress).WithMany(p => p.Requests)
				.HasForeignKey(d => d.PatientAnalysisAddressId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("request_ibfk_4");

			entity.HasOne(d => d.Patient).WithMany(p => p.Requests)
				.HasForeignKey(d => d.PatientId)
				.HasConstraintName("request_ibfk_2");

			entity.HasOne(d => d.RequestState).WithMany(p => p.Requests)
				.HasForeignKey(d => d.RequestStateId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("request_ibfk_5");
		});

		modelBuilder.Entity<RequestAnalysis>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("request_analyses");

			entity.HasIndex(e => e.AnalysisId, "analysis_id");

			entity.HasIndex(e => e.RequestId, "request_id");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.AnalysisId).HasColumnName("analysis_id");
			entity.Property(e => e.RequestId).HasColumnName("request_id");

			entity.HasOne(d => d.Analysis).WithMany(p => p.RequestAnalyses)
				.HasForeignKey(d => d.AnalysisId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("request_analyses_ibfk_2");

			entity.HasOne(d => d.Request).WithMany(p => p.RequestAnalyses)
				.HasForeignKey(d => d.RequestId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("request_analyses_ibfk_1");
		});

		modelBuilder.Entity<RequestState>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("request_state");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Name)
				.HasColumnType("text")
				.HasColumnName("name");
		});

		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("user");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Birthday).HasColumnName("birthday");
			entity.Property(e => e.Image).HasColumnName("image");
			entity.Property(e => e.IsBlocked)
				.HasDefaultValueSql("'0'")
				.HasColumnName("is_blocked");
			entity.Property(e => e.Login)
				.HasColumnType("text")
				.HasColumnName("login");
			entity.Property(e => e.Name)
				.HasColumnType("text")
				.HasColumnName("name");
			entity.Property(e => e.Passport)
				.HasMaxLength(10)
				.HasColumnName("passport");
			entity.Property(e => e.Password)
				.HasColumnType("text")
				.HasColumnName("password");
			entity.Property(e => e.Patronym)
				.HasColumnType("text")
				.HasColumnName("patronym");
			entity.Property(e => e.Phone)
				.HasMaxLength(11)
				.HasColumnName("phone");
			entity.Property(e => e.Post).HasColumnName("post");
			entity.Property(e => e.Surname)
				.HasColumnType("text")
				.HasColumnName("surname");
		});

		modelBuilder.Entity<UserLastEnter>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.ToTable("user_last_enter");

			entity.HasIndex(e => e.UserId, "user_id");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.LastEnterDate)
				.HasColumnType("datetime")
				.HasColumnName("last_enter_date");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.User).WithMany(p => p.UserLastEnters)
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("user_last_enter_ibfk_1");
		});
		#endregion MySQL
	}
}