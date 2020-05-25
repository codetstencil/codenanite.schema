using ZeraSystems.CodeNanite.Expansion;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class DatabaseContext
    {
        private void MainFunction()
        {
            AppendText();
            var table = GetTables(false);
            foreach (var item in table)
            {
                AppendText(Indent(8) + "public DbSet<" + Singularize(item.TableName) + "> " + Pluralize(item.TableName) + " { get; set; }");
            }

            AppendText(Indent(8) + "protected override void OnModelCreating(ModelBuilder modelBuilder)");
            AppendText(Indent(8) + "{");
            foreach (var item in table)
            {
                AppendText(Indent(12) + "modelBuilder.Entity<" + Singularize(item.TableName) + ">().ToTable(" + Singularize(item.TableName).AddQuotes() + ");");
            }
            AppendText(Indent(8) + "}");
            
        }
    }
}

/*
 This is what your output should look like:
 
    public DbSet<Enrollment> Enrollments { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
    }

*/
