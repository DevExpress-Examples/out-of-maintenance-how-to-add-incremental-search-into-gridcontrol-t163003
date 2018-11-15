<!-- default file list -->
*Files to look at*:

* [IncrementalSearchBehavior.cs](./CS/IncrementalSearch/Behavior/IncrementalSearchBehavior.cs) (VB: [IncrementalSearchBehavior.vb](./VB/IncrementalSearch/Behavior/IncrementalSearchBehavior.vb))
* [MainWindow.xaml](./CS/IncrementalSearch/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/IncrementalSearch/MainWindow.xaml))
* [MainWindow.xaml.cs](./CS/IncrementalSearch/MainWindow.xaml.cs) (VB: [MainWindow.xaml](./VB/IncrementalSearch/MainWindow.xaml))
* [TaskViewModel.cs](./CS/IncrementalSearch/ViewModel/TaskViewModel.cs) (VB: [TaskViewModel.vb](./VB/IncrementalSearch/ViewModel/TaskViewModel.vb))
<!-- default file list end -->
# How to add Incremental Search into GridControl


<p><br>Starting with <strong>v16.1</strong> our GridControl supports <strong>Incremental Search</strong> out of the box. To enable this functionality, it's sufficient to set the <a href="https://documentation.devexpress.com/WPF/DevExpress.Xpf.Grid.DataViewBase.IncrementalSearchMode.property">IncrementalSearchMode</a> property to <strong>Enabled</strong> in your GridControl's view. To learn more, please refer to the following documentation article: <a href="https://documentation.devexpress.com/WPF/118017/Controls-and-Libraries/Data-Grid/Filtering-and-Searching/Incremental-Search">Incremental Search</a>.<br><br>In previous versions, you can use the approach illustrated in this example.</p>
<p>To provide this functionality in previous versions, we subscribe to the GridControl's PreviewTextInput event. When PreviewTextInput is raised, we add the entered value to a searching string. Then, we iterate through all cells in a selected column. If a cell value starts with the searching string, we remember a row handle of this cell. To move to the previous or next row that starts with the searching string, we iterate through all previous or next cells of the current column and change the FocusedRowHandle value to a row handle of a found cell. To highlight the matching string of a cell, we cast InplaceBaseEdit to the InplaceBaseEdit interface and set the HighlightedText property to the searching string value.</p>

<br/>


