using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class CreateModel : ExpansionBase, ICodeStencilCodeNanite
    {
        #region Fields
        public const string StrTableLinker = "->";
        private List<string> _splitTableCode;
        private List<string> _listOfLinks = new List<string>();
        private List<string> _linesOfDataTypes = new List<string>();
        private string _tableCode;
        private string _columnCode;
        private string _dataTypes;
        private string _schemaCode;
        private readonly Dictionary<string, string> _dataTypeDictionary = new Dictionary<string, string>();
        #endregion

        public void MainFunction()
        {
            var frm = new frmModelCreator();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SchemaItem.Clear();
                _tableCode = frm.TableCode;
                _columnCode = frm.ColumnCode;
                _dataTypes = frm.DataTypes;
                _schemaCode = frm.SchemaText;

                _splitTableCode = SplitStringToList(_tableCode, "\r\n");
                _listOfLinks = GetListOfLinks(_tableCode);

                CreateTypesDictionary();
                var i = 0;
                foreach (var item in _listOfLinks)
                {
                    i++;
                    //ProcessTables(item, SchemaItem.Count+1);
                    ProcessTables(item);
                    //if (i == 4)
                    //    break;
                }
            }
        }



        //private void DeleteAndResetTable()
        //{
        //    const string sqlstring = "ALTER TABLE [SchemaItems] ALTER COLUMN [SchemaItemId] IDENTITY (1,1)";
        //    _context.SchemaItems.Clear();
        //    _context.SaveChanges();
        //    _context.Database.ExecuteSqlCommand(sqlstring);
        //}


        /// <summary>
        /// Create Dictionary of datatypes
        /// </summary>
        private void CreateTypesDictionary()
        {
            _linesOfDataTypes = SplitStringToList(_dataTypes, "\r\n");
            foreach (var item in _linesOfDataTypes)
            {
                //var leftString = item.Split('=').First().Replace("\n", string.Empty);
                //var rightString = item.Split('=').Last().Replace("\n", string.Empty);
                var leftString = item.Split('=').First();
                var rightString = item.Split('=').Last();
                if ( !_dataTypeDictionary.ContainsKey(leftString) && !string.IsNullOrEmpty(leftString) )
                    _dataTypeDictionary.Add(leftString, rightString);
            }
        }

        private static List<string> SplitStringToList(string text, string delimiter)
        {
            return text.Split(new[] { delimiter }, StringSplitOptions.None).ToList();
        }


        public static List<string> GetTablesInCreator(IEnumerable<string> tablesandlink)
        {
            var tables = new List<string>();
            foreach (var item in tablesandlink)
            {
                if (item.Contains("[") && item.Contains("]"))
                {
                    var link = item.Split('[', ']')[1];
                    var index = item.IndexOf(link, StringComparison.Ordinal);
                    var left = (index > 0 ? item.Substring(0, index - 1) : string.Empty);
                    var lefttable = string.IsNullOrEmpty(left) ? item : left;
                    var righttable = !string.IsNullOrEmpty(lefttable)
                        ? item.Replace(lefttable + "[" + link + "]", string.Empty)
                        : string.Empty;

                    if (!tables.Contains(lefttable))
                        tables.Add(lefttable);
                    if (!tables.Contains(righttable))
                        tables.Add(righttable);
                }
            }
            return tables;
        }

        /// <summary>
        /// Splits the table code into a list of all tables
        /// </summary>
        /// <param name="text">String from Text Control</param>
        /// <returns>List of Tables</returns>
        public static List<string> GetListOfLinks(string text)
        {
            var tables = new List<string>();
            //var tablecodes = new List<string>(text.Split('\r'));
            var tablecodes = SplitStringToList(text, "\r\n");
            foreach (var item in tablecodes)
                AddTableToList(item, tables);
            return tables;
        }

        /// <summary>
        /// Add a table to a collection
        /// </summary>
        /// <param name="text">Table Name</param>
        /// <param name="list">List to update</param>
        private static void AddTableToList(string text, ICollection<string> list)
        {
            if (text.Contains(StrTableLinker))
            {
                //var newList = new List<string>(text.Split(Convert.ToChar(StrTableLinker)));
                var newList = SplitStringToList(text, StrTableLinker);
                foreach (var item in newList)
                {
                    if (!list.Any(e => e.EndsWith(item)))
                        list.Add(item);
                }
            }
            else
                list.Add(text);
        }

        #region Schema Functions

        private string GetDataType(string text)
        {
            if (_dataTypeDictionary.ContainsKey(text))
            {
                var value = _dataTypeDictionary[text];
                return value;
            }
            return null;
        }

        #endregion Schema Functions
    }
}