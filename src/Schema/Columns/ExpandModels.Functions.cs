using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Pluralize.NET;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class ExpandModels
    {
        private string _public = "public ";
        private string _getSet = " { get; set; }";
        private string _virtual = "virtual ";
        private void MainFunction()
        {
            AppendText();
            BuildSnippet(null);
            BuildSnippet("");
            BuildSnippet(_public+"class " + Singularize(Input),4) ;
            BuildSnippet("{",4);
            AppendText(GetColumns());
            BuildSnippet("}", 4);
            AppendText(BuildSnippet());
            //AppendText(Indent(0) + "}");
        }

        readonly StringBuilder _iCollectionList = new StringBuilder();

        private string GetColumns(bool ignoreCalculated = true)
        {
            _iCollectionList.Clear();
            var columns = GetColumnsAndNavigation(Input);

            foreach (var item in columns)
            {
                if (ignoreCalculated && item.IsCalculatedColumn) continue;

                if (item.IsForeignKey && item.TableName != Input)
                    GetCollectionNavigation(item);
                else
                    BuildSnippet(GetColumnProperty(item));

                if (item.IsForeignKey && item.TableName == Input)
                {
                    if (item.RelatedTable == Input) //A table related to itself
                    {
                        BuildSnippet(_public + _virtual + item.RelatedTable + " " + item.ColumnName + NavigationLabel() + _getSet);
                        GetCollectionNavigation(item.TableName, item.ColumnName + NavigationLabel());
                    }
                    else
                        BuildSnippet(_public + _virtual + item.RelatedTable + " " + item.RelatedTable + _getSet);
                }
            }
            BuildSnippet(_iCollectionList.ToString());
            return BuildSnippet();
        }


        private string GetColumnProperty(ISchemaItem item )
        {
            BuildSnippet(_public + item.ColumnType + GetNullSign(item) + " " + item.ColumnName + _getSet,8,true);
            return BuildSnippet();
        }

        public new static string GetNullSign(ISchemaItem item) => item.AllowDbNull ? "?" : string.Empty;

        private void GetCollectionNavigation(ISchemaItem item)
        {
            var column = Singularize(item.TableName);
            var columns = Pluralize(item.TableName);
            BuildSnippet(_iCollectionList, _public +"virtual ICollection<" + column + "> " + columns + _getSet+" = new HashSet<" + column + ">();",8);
        }
        private void GetCollectionNavigation(string item, string inverse)
        {
            BuildSnippet(_iCollectionList, _public + "virtual ICollection<" + item + "> " + inverse + _getSet + " = new HashSet<" + item + ">();");
        }

        //private static string Singularize(string text) => new Pluralizer().Singularize(text);
        //private static string Pluralize(string text) => new Pluralizer().Pluralize(text);
        private void GetHashSet(List<ISchemaItem> columns, string theIndent)
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

        private void AddNamespaces()
        {
            AppendText(Indent(0) + "using System.ComponentModel.DataAnnotations;");
            AppendText(Indent(0) + "using System.ComponentModel.DataAnnotations.Schema;");
            AppendText(string.Empty);
        }

    }
}