using System.Collections.Generic;  
using System.ComponentModel.Composition;
using ZeraSystems.CodeNanite.Schema;
using ZeraSystems.CodeStencil.Contracts;  
  
namespace ZeraSystems.CodeNanite.Schema  
{
    /// <summary>  
    /// There are 11 elements in the String Array used by the   
    /// 0 - This is the name of the publisher  
    /// 1 - This is the title of the Code Nanite  
    /// 2 - This is the description  
    /// 3 - Version Number  
    /// 4 - Label of the Code Nanite  
    /// 5 - Namespace  
    /// 6 - Release Date  
    /// 7 - Name to use for Expander Label  
    /// 8 - RESERVED  
    /// 9 - RESERVED  
    ///10 - RESERVED  
    /// </summary>  
    //[Export(typeof(ICodeStencilCodeNanite))]  
    [CodeStencilCodeNanite(new[]  
    {
        "Zera Systems Inc.",                                        // 0
        "Global Schema Generator",                                  // 1
        "Generates Schema to be used to update the Global Schema",  // 2                                     
        "1.0",                                                      // 3
        "CreateModel",                                              // 4
        "ZeraSystems.CodeNanite.Schema",                                   // 5
        "01/01/2018",                                               // 6
        "CS_UPDATE_GLOBAL_SCHEMA",                                     // 7
        "0",                                                        // 8
        "1",                                                        // 9
        ""                                                          //10
    })]  
    public partial class CreateModel: ExpansionBase, ICodeStencilCodeNanite  
    {  
        public string Input { get; set; }  
        public string Output { get; set; }  
        public int Counter { get; set; }  
        public List<string> OutputList { get; set; }  
        public List<ISchemaItem> SchemaItem { get; set; }  
        public List<IExpander> Expander { get; set; }
        public List<string> InputList { get; set; }

        public void ExecutePlugin()  
        {
            Initializer(SchemaItem, Expander);  
            MainFunction();  
            Output = ExpandedText.ToString();  
        }  
    }  
}
