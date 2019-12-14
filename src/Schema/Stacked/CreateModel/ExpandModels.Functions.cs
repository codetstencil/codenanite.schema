using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Pluralize.NET;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class ExpandModels
    {
        private void MainFunction()
        {
            AppendText();

            //AddNamespaces();
            //AppendText(Indent(0) + "namespace " + GetDefaultNameSpace());
            //AppendText(Indent(0) + "{");
            //AppendText(Indent(4) + "public class " + GetTable(Input));
            AppendText(Indent(4) + "public class " + Singularize(Input) );
            AppendText(Indent(4) + "{");
            AddColumns();
            AppendText(Indent(4) + "}");
            //AppendText(Indent(0) + "}");
        }

        private void AddNamespaces()
        {
            AppendText(Indent(0) + "using System.ComponentModel.DataAnnotations;");
            AppendText(Indent(0) + "using System.ComponentModel.DataAnnotations.Schema;");
            AppendText(string.Empty);
        }

        private void AddColumns()
        {
            var lines = new List<string>();
            var columns = GetColumnsAndNavigation(Input);
            var theIndent = Indent(8);

            GetHashSet(columns, theIndent);

            foreach (var item in columns)
            {
                string line;

                if (item.IsForeignKey && item.TableName != Input)
                    line = GetCollectionNavigation(theIndent, item);
                else
                    line = GetColumnProperty(theIndent, item);
                AppendText(line);

                if (item.IsForeignKey && item.TableName == Input)
                    AppendText(theIndent + "public virtual "+item.RelatedTable+" "+item.RelatedTable+" { get; set; }");

            }
        }

        private  void GetHashSet(List<ISchemaItem> columns, string theIndent)
        {
            AppendText(theIndent + "public " + Singularize(Input) + "()");
            AppendText(theIndent + "{");
            foreach (var item in columns)
            {
                if (item.IsForeignKey && item.TableName!= Input)
                    AppendText(theIndent + Indent(4) + "this." + Pluralize(item.TableName) + " = new HashSet<" + Singularize(item.TableName) + ">();");
            }
            AppendText(theIndent + "}");
        }

        private static string GetColumnProperty(string theIndent, ISchemaItem item)
        {
            return theIndent + "public " + item.ColumnType + " " + item.ColumnName + " { get; set; }";
        }

        private  string GetCollectionNavigation(string theIndent, ISchemaItem item)
        {
            return theIndent + "public virtual ICollection<" + Singularize(item.TableName) + "> " + Pluralize(item.TableName) + " { get; set; }";
        }

        //private static string Singularize(string text) => new Pluralizer().Singularize(text);
        //private static string Pluralize(string text) => new Pluralizer().Pluralize(text);
    }
}