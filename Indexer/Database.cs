namespace Indexer
{
    using POCOs;
    using System.Data.Common;
    using System.Data.Entity;      
    using System.Data.Entity.ModelConfiguration.Conventions;

    public partial class Database : DbContext
    {
        public DbSet<Team> Teams { get; set; }

        public Database()
        {            
        }
        public Database(DbConnection connection)
            : base(connection, true)
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
