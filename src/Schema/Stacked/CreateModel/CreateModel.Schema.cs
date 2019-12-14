using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class CreateModel
    {


        void ProcessTables(string table)
        {
            string primaryTable;
            var secondaryTable = string.Empty;
            var link = string.Empty;

            if (table.Contains("[") && table.Contains("]"))
            {
                link = table.Split('[', ']')[1];

                if (link == Constants.ManyToOne)
                    SplitTables(table, link, out secondaryTable, out primaryTable);
                else
                    SplitTables(table, link, out primaryTable, out secondaryTable);
            }
            else
            {
                primaryTable = table;
                ProcessPrimaryTable(primaryTable);
                return;
            }

            switch (link)
            {
                case Constants.OneToMany:
                    ProcessPrimaryTable(primaryTable);
                    ProcessSecondaryTable(secondaryTable, primaryTable);
                    break;
                case Constants.ManyToMany:
                    ProcessPrimaryTable(primaryTable);
                    ProcessPrimaryTable(secondaryTable);
                    break;
                case Constants.OneToOne:
                    ProcessPrimaryTable(primaryTable);
                    ProcessPrimaryTable(secondaryTable, primaryTable);
                    break;
            }

        }

        private static void SplitTables(string table, string link, out string lefttable, out string righttable)
        {
            //var index = table.IndexOf(Constants.OneToMany, StringComparison.Ordinal);
            var index = table.IndexOf(link, StringComparison.Ordinal);
            var left = (index > 0 ? table.Substring(0, index - 1) : string.Empty);
            lefttable = string.IsNullOrEmpty(left) ? table : left;
            righttable = !string.IsNullOrEmpty(lefttable) ? table.Replace(lefttable + "[" + link + "]", string.Empty) : string.Empty;
        }

        private void ProcessPrimaryTable(string table, string columnname = null)
        {
            if (GetParentId(table) < 0)
                //Add Table
                SchemaItemUpdater(CreateTableEntry(table));

            if (GetPrimaryKey(table) < 0)
                // Primary Key
               SchemaItemUpdater(CreatePrimaryKeyEntry(table, columnname));

            //Add Columns
            ProcessColumns(table);
        }

        void ProcessSecondaryTable(string table, string primarytable)
        {
            //var s = SchemaItem.ToArray();
            //Find if this table already has a primary key
            if (GetParentId(table) < 0)      //Meaning table not yet created
                SchemaItemUpdater(CreateTableEntry(table));
            if (GetPrimaryKey(table) < 0)    //Meaning not PK in the primary table
                SchemaItemUpdater(CreatePrimaryKeyEntry(table));

            //var s2 = SchemaItem.ToArray();


            ////Add Table
            //SchemaItemUpdater(CreateTableEntry(table));

            //// Primary Key
            //SchemaItemUpdater(CreatePrimaryKeyEntry(table));

            //Add Columns
            ProcessColumns(table);

            //Add Foreign Key
            SchemaItemUpdater(CreateForeignKeyEntry(table, primarytable));

            //var searchString = StrTableLinker + table;
            //var list = _splitTableCode.Where(e => e.Contains(searchString));
            //foreach (var item in list)
            //{
            //    var foreignKey = item.Replace(searchString, string.Empty) + "ID";
            //    SchemaItemUpdater(CreateForeignKeyEntry(i, table, foreignKey));
            //}


        }

        private void HasPrimaryKey(string table, bool createkey=false)
        {
            var primarykey = table + Constants.KeySuffix;
            var result = SchemaItem.First(t => (t.TableName == table && t.ColumnName == primarykey && t.IsPrimaryKey == true));
            if (result == null && createkey)
                SchemaItemUpdater(CreatePrimaryKeyEntry(table));

        }

        private int GetParentId(string table)
        {
            //var row = SchemaItem.First(t => (t.TableName == table && t.ColumnName == table && t.ParentId == 0));
            var row = SchemaItem.FirstOrDefault(t => (t.TableName == table && t.ColumnName == table && t.ParentId == 0));
            if (row != null)
                return row.SchemaItemId;
            else
                return -1;
        }

        private int GetPrimaryKey(string table)
        {
            var row = SchemaItem.FirstOrDefault(t => (t.TableName == table && t.ColumnName == table+Constants.KeySuffix && t.IsPrimaryKey));
            if (row != null)
                return row.SchemaItemId;
            else
                return -1;
        }

        /// <summary>
        /// Create a Table Name entry
        /// </summary>
        /// <param name="tablename">Table Name</param>
        /// <returns>Table Entry</returns>
        private SchemaItemObject CreateTableEntry(string tablename)
        {
            var row = new SchemaItemObject
            {
                //SchemaItemId = id,
                SchemaItemId = SchemaItem.Count+1,
                ParentId = 0,
                TableName = tablename,
                ColumnName = tablename
            };
            return row;
        }

        /// <summary>
        /// Create a Column Name Entry
        /// </summary>
        /// <param name="parentid">Parent ID column belongs to</param>
        /// <param name="tablename"></param>
        /// <param name="columnname"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="attrib"></param>
        /// <param name="allownull"></param>
        /// <returns></returns>
        private SchemaItemObject CreateColumnEntry(int parentid, string tablename, string columnname, string type, int size, bool allownull = true, string attrib = null)
        {
            var row = new SchemaItemObject
            {
                ParentId = parentid,
                TableName = tablename,
                ColumnName = columnname,
                ColumnType = type,
                ColumnSize = size,
                AllowDbNull = allownull,
                ColumnAttribute = attrib,
                IsChecked = true,
                IsUpdatedByNanite = true
            };
            return row;
        }



        /// <summary>
        /// Create a primary key entry
        /// </summary>
        /// <param name="table">Name of Table key belongs to</param>
        /// <param name="column">Column name to add ID to</param>
        /// <returns>Column Entry</returns>
        private SchemaItemObject CreatePrimaryKeyEntry(string table, string column = null)
        {
            var columname = column;
            if (columname != null)
            {
                if (columname.Substring(columname.Length - 3) == "ies")
                    columname = columname.Replace("ies", "y");
            }
            else
                columname = table;
            columname += Constants.KeySuffix;

            var row = new SchemaItemObject
            {
                ParentId = GetParentId(table),
                TableName = table,
                ColumnName = columname.Replace("\n", string.Empty),
                ColumnType = "int",
                ColumnSize = 0,
                ColumnAttribute = "[Key]",
                IsPrimaryKey = true,
                IsAutoIncrement = true,
                AllowDbNull = false,
                IsUpdatedByNanite = true
            };
            return row;
        }

        /// <summary>
        /// Create a Foreign Key entry
        /// </summary>
        /// <param name="table">Name of Table key belongs to</param>
        /// <param name="primarytable">Primary Table </param>
        /// <param name="allownull"></param>
        /// <param name="attrib"></param>
        /// <returns></returns>
        private SchemaItemObject CreateForeignKeyEntry(string table, string primarytable, bool allownull = true, string attrib = null)
        {
            var foreignKey = primarytable + Constants.KeySuffix;
            var row = new SchemaItemObject
            {
                ParentId = GetParentId(table),
                TableName = table,
                ColumnName = foreignKey.Replace("\n", string.Empty),
                ColumnType = "int",
                ColumnSize = 0,
                ColumnAttribute = attrib,
                AllowDbNull = allownull,
                IsChecked = true, 
                IsForeignKey = true,
                IsUpdatedByNanite = true
            };
            return row;
        }


        /// <summary>
        /// Process all the basic columns, no foreign keys, no primary keys
        /// </summary>
        /// <param name="table">The table to add columns for</param>
        /// <param name="i">ID of table to process</param>
        private void ProcessColumns(string table)
        {
            var columnTable = "[" + table + "]";
            var blocks = new List<string>(_columnCode.Replace("[","*[").Split('*'));
            var list = blocks.Where(e => e.Contains(columnTable));

            foreach (var item in list)
            {
                var columns = item.Replace(columnTable, string.Empty).Trim();
                var columnList = new List<string>(columns.Split(','));
                foreach (var col in columnList)
                {
                    if ( string.IsNullOrEmpty(col))
                        continue;
                    
                    var columnName = col.Trim();
                    if (col.Contains("(") && col.Contains(")"))
                    {
                        columnName = col.Split('(').First().Replace("\n", string.Empty);
                        var str = col.Split('(').Last().Replace(")", string.Empty);
                        var colProperty = GetColumnProperties(str);
                        //var colProperty = GetColumnProperties(col.Replace("(", string.Empty).Replace(")", string.Empty));

                        //var bracket = col.Split(new char[] { '(', ')' })[1];
                        //columnName = columnName.Replace("(" + bracket + ")", string.Empty);
                        //stringLetter = Regex.Replace(bracket, @"[^A-Z]+", string.Empty).ToLower();
                        //stringSize = Regex.Replace(bracket, @"[^0-9]+", string.Empty);

                        SchemaItemUpdater(CreateColumnEntry(GetParentId(table), table, columnName, GetDataType(colProperty.Datatype),
                            colProperty.Size, colProperty.Nullability));
                    }
                    else
                    {
                        SchemaItemUpdater(CreateColumnEntry(GetParentId(table), table, columnName, GetDataType("s"), 100, true));

                        //SchemaItemUpdater(CreateColumnEntry(i, table, columnName, GetDataType(colProperty.Datatype),
                        //    colProperty.Size, colProperty.Nullability));

                        //--need to recode this part
                        //var size = 0;
                        //if (!string.IsNullOrEmpty(stringSize))
                        //    size = Convert.ToInt32(stringSize);
                    }
                }
            }
        }

        private static ColumnProperty GetColumnProperties(string colstring)
        {
            var size = 0;
            var nullable = true;

            var arr = colstring.Split(',');
            var type = arr[0].ToLower();
            if (arr.Length > 1)
                size = Convert.ToInt32(arr[1]);
            if (arr.Length == 2)
                nullable = arr[2].ToLower() == "null";

            var prop = new ColumnProperty
            {
                Datatype = type,
                Size = size,
                Nullability = nullable
            };
            return prop;
        }

        private struct ColumnProperty
        {
            public string Datatype;
            public int Size;
            public bool Nullability;
        };

        /// <summary>
        /// Process the foreign key columns
        /// </summary>
        /// <param name="table">Table whose foreign key to insert</param>
        /// <param name="i">ID of the table</param>
        private void ProcessForeignKeys(string table, int i)
        {
            var searchString = StrTableLinker + table;
            var list = _splitTableCode.Where(e => e.Contains(searchString));
            foreach (var item in list)
            {
                var foreignKey = item.Replace(searchString, string.Empty) + Constants.KeySuffix;
                SchemaItemUpdater(CreateForeignKeyEntry(table, foreignKey));
            }
        }
    }
}