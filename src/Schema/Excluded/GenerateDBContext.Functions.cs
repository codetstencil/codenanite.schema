using System.Collections.Generic;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class GenerateDbContext
    {
        #region Fields

        private string _namespace;
        private string _table;
        private List<ISchemaItem> _columns;
        private string _dbcontext;

        #endregion Fields

        private void MainFunction()
        {
            _dbcontext = GetExpansionString("DB_CONTEXT");
            _namespace = GetExpansionString("NAMESPACE");
            _table = GetTable(Input);
            //_columns = GetColumns(_table,false);
            _columns = GetColumnsAndNavigation(Input, false);
            if (_columns == null)
                return;

            Generate();
        }

        private void Generate()
        {
            AppendText();
            var thistable = GetTables(false);
            foreach (var item in thistable)
            {
                AppendText(Indent(8) + "public DbSet<" + Singularize(item.TableName) + "> " + Pluralize(item.TableName) + " { get; set; }");
            }
            AppendText("");

            AppendText(Indent(8) + "protected override void OnModelCreating(ModelBuilder modelBuilder)");
            AppendText(Indent(8) + "{");
            var noLineFeed = string.Empty;
            foreach (var item in thistable)
            {
                var tableEntityConfig = Singularize(item.TableName) + "EntityConfiguration";
                AppendText(Indent(12) + "modelBuilder.ApplyConfiguration(new " + tableEntityConfig + "());", noLineFeed) ;
            }
            AppendText(Indent(8) + "}");
        }

        //private void Generate()
        //{
        //    AppendText();
        //    var thistable = GetTables();
        //    foreach (var item in thistable)
        //    {
        //        AppendText(Indent(8) + "public DbSet<" + Singularize(item.TableName) + "> " + Pluralize(item.TableName) + " { get; set; }");
        //    }
        //    AppendText("");

        //    AppendText(Indent(8) + "protected override void OnModelCreating(DbModelBuilder modelBuilder)");
        //    AppendText(Indent(8) + "{");
        //    var noLineFeed = string.Empty;
        //    foreach (var item in thistable)
        //    {
        //        var tableEntityconfig = Singularize(item.TableName) + "EntityConfiguration";
        //        AppendText(Indent(12) + "modelBuilder.Configurations.Add(new " + tableEntityconfig + "());", noLineFeed);
        //        //AppendText("");
        //    }
        //    AppendText(Indent(8) + "}");
        //}

    }
}