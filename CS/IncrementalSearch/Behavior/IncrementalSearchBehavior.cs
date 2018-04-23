using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace IncrementalSearch
{
    public class IncrementalSearchBehavior : Behavior<UIElement>
    {
        private GridControl Grid;
        private TableView TableView;
        private string SearchingString;
        private GridColumn PreviousColumn;
        private int PreviousRowHandle;
        private FrameworkElement Cell;
        private bool IsMatchRow;
        private bool IsCustomFocus;

        public enum SearchParameter
        {
            Next,
            Pevious
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            Grid = this.AssociatedObject as GridControl;
            Grid.Loaded += Grid_Loaded;
        }

        void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            TableView = Grid.View as TableView;

            Grid.PreviewTextInput += Grid_PreviewTextInput;
            Grid.PreviewKeyDown += Grid_PreviewKeyDown;
            Grid.CurrentColumnChanged += Grid_CurrentColumnChanged;
            TableView.LayoutUpdated += TableView_LayoutUpdated;
            TableView.FocusedRowHandleChanged += TableView_FocusedRowHandleChanged;
        }

        void TableView_FocusedRowHandleChanged(object sender, FocusedRowHandleChangedEventArgs e)
        {
            if (IsCustomFocus == false)
            {
                SearchingString = null;
                SetTextHighlight(null, PreviousRowHandle, Grid.CurrentColumn as GridColumn);
            }

            else
            {
                IsCustomFocus = false;
            }
        }

        void TableView_LayoutUpdated(object sender, EventArgs e)
        {
            TextHighlightUpdate();
        }

        void Grid_CurrentColumnChanged(object sender, CurrentColumnChangedEventArgs e)
        {
            if (PreviousColumn != null)
            {
                SetTextHighlight(null, PreviousRowHandle, PreviousColumn);
                SearchingString = null;
                IsMatchRow = false;
            }
        }

        void Grid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Down)
            {
                if (IsMatchRow != false && TableView.FocusedRowHandle + 1 < Grid.VisibleRowCount)
                {
                    IsCustomFocus = true;

                    SetTextHighlight(null, TableView.FocusedRowHandle, PreviousColumn);

                    var handle = FindNextMatch(TableView.FocusedRowHandle, Grid.CurrentColumn as GridColumn, SearchingString, SearchParameter.Next);
                    TableView.FocusedRowHandle = handle;

                    PreviousRowHandle = TableView.FocusedRowHandle;

                    Cell = TableView.GetCellElementByRowHandleAndColumn(TableView.FocusedRowHandle, Grid.CurrentColumn);
                    e.Handled = true;
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Up)
            {
                if (IsMatchRow != false && TableView.FocusedRowHandle - 1 >= 0)
                {
                    IsCustomFocus = true;

                    SetTextHighlight(null, TableView.FocusedRowHandle, PreviousColumn);

                    var handle = FindNextMatch(TableView.FocusedRowHandle, Grid.CurrentColumn as GridColumn, SearchingString, SearchParameter.Pevious);
                    TableView.FocusedRowHandle = handle;
                    PreviousRowHandle = TableView.FocusedRowHandle;

                    Cell = TableView.GetCellElementByRowHandleAndColumn(TableView.FocusedRowHandle, Grid.CurrentColumn);
                    e.Handled = true;
                }
            }

            else if (e.Key == Key.Up || e.Key == Key.Down)
            {
                SearchingString = null;
                SetTextHighlight(null, TableView.FocusedRowHandle, PreviousColumn);
            }
        }

        void Grid_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(RowIndicator) || e.OriginalSource.GetType() == typeof(InplaceBaseEdit))
            {
                var currentColumn = Grid.CurrentColumn as GridColumn;

                if (PreviousColumn != currentColumn && PreviousColumn != null)
                {
                    SearchingString = null;
                }

                if (e.Text == "\b" && SearchingString != null && SearchingString.Length > 0)
                {
                    IsCustomFocus = true;
                    SearchingString = SearchingString.Remove(SearchingString.Length - 1);
                }

                else if (e.Text != "\b" && e.Text != "\r")
                {
                    IsCustomFocus = true;
                    SearchingString += e.Text;
                }

                TextSearch(currentColumn, SearchingString);

                e.Handled = true;
            }
        }

        public void TextSearch(GridColumn selectedColumn, string searchingString)
        {
            IsMatchRow = false;

            int matchRowHandle = 0;

            if (!String.IsNullOrEmpty(searchingString))
            {
                for (int i = 0; i < Grid.VisibleRowCount; i++)
                {
                    var handle = Grid.GetRowHandleByVisibleIndex(i);

                    var cellValue = Grid.GetCellValue(handle, selectedColumn).ToString();

                    if (cellValue.StartsWith(searchingString, true, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        matchRowHandle = handle;
                        IsMatchRow = true;
                        break;
                    }
                }

                if (IsMatchRow == true)
                {
                    SetTextHighlight(null, PreviousRowHandle, selectedColumn);

                    TableView.FocusedRowHandle = matchRowHandle;
                    PreviousRowHandle = matchRowHandle;

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        SetTextHighlight(searchingString, matchRowHandle, selectedColumn);
                        Cell = TableView.GetCellElementByRowHandleAndColumn(matchRowHandle, selectedColumn);
                    }), DispatcherPriority.Render);
                }

                else
                {
                    SearchingString = SearchingString.Remove(SearchingString.Length - 1);
                }
            }
            else
            {
                SetTextHighlight(null, PreviousRowHandle, selectedColumn);
            }

            PreviousColumn = selectedColumn;
        }

        public int FindNextMatch(int rowHandle, GridColumn selectedColumn, string searchingString, SearchParameter parameter)
        {
            int foundHandle = 0;

            if (!String.IsNullOrEmpty(searchingString))
            {
                bool match = false;

                var currentHandle = Grid.GetRowVisibleIndexByHandle(rowHandle);

                if (parameter == SearchParameter.Next)
                {
                    for (int i = currentHandle + 1; i < Grid.VisibleRowCount; i++)
                    {
                        var handle = Grid.GetRowHandleByVisibleIndex(i);

                        var cellValue = Grid.GetCellValue(handle, selectedColumn).ToString();

                        if (cellValue.StartsWith(searchingString, true, System.Globalization.CultureInfo.CurrentCulture))
                        {
                            foundHandle = handle;
                            match = true;
                            break;
                        }
                    }

                    if (match == false)
                    {
                        foundHandle = currentHandle;
                    }
                }
                else if (parameter == SearchParameter.Pevious)
                {
                    for (int i = currentHandle - 1; i >= 0; i--)
                    {
                        var handle = Grid.GetRowHandleByVisibleIndex(i);

                        var cellValue = Grid.GetCellValue(handle, selectedColumn).ToString();

                        if (cellValue.StartsWith(searchingString, true, System.Globalization.CultureInfo.CurrentCulture))
                        {
                            foundHandle = handle;
                            match = true;
                            break;
                        }
                    }

                    if (match == false)
                    {
                        foundHandle = currentHandle;
                    }
                }
            }

            return foundHandle;
        }

        public void SetTextHighlight(object valueString, int rowHandle, GridColumn column)
        {
            var cell = TableView.GetCellElementByRowHandleAndColumn(rowHandle, column) as CellEditor;

            if (cell != null)
            {
                var editor = VisualTreeHelper.GetChild(cell, 0) as InplaceBaseEdit;
                var inplaceEditor = editor as IInplaceBaseEdit;

                inplaceEditor.HighlightedText = valueString as string;
                inplaceEditor.HighlightedTextCriteria = HighlightedTextCriteria.StartsWith;
            }
        }

        public void TextHighlightUpdate()
        {
            var currentCell = TableView.GetCellElementByRowHandleAndColumn(TableView.FocusedRowHandle, Grid.CurrentColumn);

            if (currentCell != null)
            {
                Cell = currentCell;

                SetTextHighlight(SearchingString, TableView.FocusedRowHandle, Grid.CurrentColumn as GridColumn);
            }
            else if (Cell != null)
            {
                var editor = VisualTreeHelper.GetChild(Cell, 0) as InplaceBaseEdit;
                var inplaceEditor = editor as IInplaceBaseEdit;

                inplaceEditor.HighlightedText = null;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}
