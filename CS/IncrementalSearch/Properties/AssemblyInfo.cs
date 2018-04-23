// Developer Express Code Central Example:
// How to add Incremental Search into GridControl
// 
// This example demonstrates how to add Incremental Search into GridControl.
// 
// Our
// GridControl doesn't have Incremental Search. To provide this functionality, we
// subscribe to the GridControl's PreviewTextInput event. When PreviewTextInput is
// raised, we add the entered value to a searching string. Then, we iterate through
// all cells in a selected column. If a cell value starts with the searching
// string, we remember a row handle of this cell. To move to the previous or next
// row that starts with the searching string, we iterate through all previous or
// next cells of the current column and change the FocusedRowHandle value to a row
// handle of a found cell. To highlight the matching string of a cell, we cast
// InplaceBaseEdit to the InplaceBaseEdit interface and set the HighlightedText
// property to the searching string value.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=T163003

using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("IncrementalSearch")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("IncrementalSearch")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]


// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
