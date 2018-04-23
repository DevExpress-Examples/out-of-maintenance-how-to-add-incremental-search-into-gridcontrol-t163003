Imports Microsoft.VisualBasic
Imports DevExpress.Mvvm.UI.Interactivity
Imports DevExpress.Xpf.Editors
Imports DevExpress.Xpf.Grid
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Threading

Namespace IncrementalSearch
	Public Class IncrementalSearchBehavior
		Inherits Behavior(Of UIElement)
		Private Grid As GridControl
		Private TableView As TableView
		Private SearchingString As String
		Private PreviousColumn As GridColumn
		Private PreviousRowHandle As Integer
		Private Cell As FrameworkElement
		Private IsMatchRow As Boolean
		Private IsCustomFocus As Boolean

		Public Enum SearchParameter
			[Next]
			Pevious
		End Enum

		Protected Overrides Sub OnAttached()
			MyBase.OnAttached()
			Grid = TryCast(Me.AssociatedObject, GridControl)
			AddHandler Grid.Loaded, AddressOf Grid_Loaded
		End Sub

		Private Sub Grid_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			TableView = TryCast(Grid.View, TableView)

			AddHandler Grid.PreviewTextInput, AddressOf Grid_PreviewTextInput
			AddHandler Grid.PreviewKeyDown, AddressOf Grid_PreviewKeyDown
			AddHandler Grid.CurrentColumnChanged, AddressOf Grid_CurrentColumnChanged
			AddHandler TableView.LayoutUpdated, AddressOf TableView_LayoutUpdated
			AddHandler TableView.FocusedRowHandleChanged, AddressOf TableView_FocusedRowHandleChanged
		End Sub

		Private Sub TableView_FocusedRowHandleChanged(ByVal sender As Object, ByVal e As FocusedRowHandleChangedEventArgs)
			If IsCustomFocus = False Then
				SearchingString = Nothing
				SetTextHighlight(Nothing, PreviousRowHandle, TryCast(Grid.CurrentColumn, GridColumn))

			Else
				IsCustomFocus = False
			End If
		End Sub

		Private Sub TableView_LayoutUpdated(ByVal sender As Object, ByVal e As EventArgs)
			TextHighlightUpdate()
		End Sub

		Private Sub Grid_CurrentColumnChanged(ByVal sender As Object, ByVal e As CurrentColumnChangedEventArgs)
			If PreviousColumn IsNot Nothing Then
				SetTextHighlight(Nothing, PreviousRowHandle, PreviousColumn)
				SearchingString = Nothing
				IsMatchRow = False
			End If
		End Sub

		Private Sub Grid_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs)
			If Keyboard.Modifiers = ModifierKeys.Control AndAlso e.Key = Key.Down Then
				If IsMatchRow <> False AndAlso TableView.FocusedRowHandle + 1 < Grid.VisibleRowCount Then
					IsCustomFocus = True

					SetTextHighlight(Nothing, TableView.FocusedRowHandle, PreviousColumn)

					Dim handle = FindNextMatch(TableView.FocusedRowHandle, TryCast(Grid.CurrentColumn, GridColumn), SearchingString, SearchParameter.Next)
					TableView.FocusedRowHandle = handle

					PreviousRowHandle = TableView.FocusedRowHandle

					Cell = TableView.GetCellElementByRowHandleAndColumn(TableView.FocusedRowHandle, Grid.CurrentColumn)
					e.Handled = True
				End If
			ElseIf Keyboard.Modifiers = ModifierKeys.Control AndAlso e.Key = Key.Up Then
				If IsMatchRow <> False AndAlso TableView.FocusedRowHandle - 1 >= 0 Then
					IsCustomFocus = True

					SetTextHighlight(Nothing, TableView.FocusedRowHandle, PreviousColumn)

					Dim handle = FindNextMatch(TableView.FocusedRowHandle, TryCast(Grid.CurrentColumn, GridColumn), SearchingString, SearchParameter.Pevious)
					TableView.FocusedRowHandle = handle
					PreviousRowHandle = TableView.FocusedRowHandle

					Cell = TableView.GetCellElementByRowHandleAndColumn(TableView.FocusedRowHandle, Grid.CurrentColumn)
					e.Handled = True
				End If

			ElseIf e.Key = Key.Up OrElse e.Key = Key.Down Then
				SearchingString = Nothing
				SetTextHighlight(Nothing, TableView.FocusedRowHandle, PreviousColumn)
			End If
		End Sub

		Private Sub Grid_PreviewTextInput(ByVal sender As Object, ByVal e As System.Windows.Input.TextCompositionEventArgs)
			If e.OriginalSource.GetType() Is GetType(RowIndicator) OrElse e.OriginalSource.GetType() Is GetType(InplaceBaseEdit) Then
				Dim currentColumn = TryCast(Grid.CurrentColumn, GridColumn)

				If PreviousColumn IsNot currentColumn AndAlso PreviousColumn IsNot Nothing Then
					SearchingString = Nothing
				End If

				If e.Text Is Constants.vbBack AndAlso SearchingString IsNot Nothing AndAlso SearchingString.Length > 0 Then
					IsCustomFocus = True
					SearchingString = SearchingString.Remove(SearchingString.Length - 1)

				ElseIf e.Text IsNot Constants.vbBack AndAlso e.Text IsNot Constants.vbCr Then
					IsCustomFocus = True
					SearchingString &= e.Text
				End If

				TextSearch(currentColumn, SearchingString)

				e.Handled = True
			End If
		End Sub

		Public Sub TextSearch(ByVal selectedColumn As GridColumn, ByVal searchingString As String)
			IsMatchRow = False

			Dim matchRowHandle As Integer = 0

			If (Not String.IsNullOrEmpty(searchingString)) Then
				For i As Integer = 0 To Grid.VisibleRowCount - 1
					Dim handle = Grid.GetRowHandleByVisibleIndex(i)

					Dim cellValue = Grid.GetCellValue(handle, selectedColumn).ToString()

					If cellValue.StartsWith(searchingString, True, System.Globalization.CultureInfo.CurrentCulture) Then
						matchRowHandle = handle
						IsMatchRow = True
						Exit For
					End If
				Next i

				If IsMatchRow = True Then
					SetTextHighlight(Nothing, PreviousRowHandle, selectedColumn)

					TableView.FocusedRowHandle = matchRowHandle
					PreviousRowHandle = matchRowHandle

                    Dispatcher.BeginInvoke(New Action(Function() AnonymousMethod1(selectedColumn, searchingString, matchRowHandle)), DispatcherPriority.Render)

				Else
					Me.SearchingString = Me.SearchingString.Remove(Me.SearchingString.Length - 1)
				End If
			Else
				SetTextHighlight(Nothing, PreviousRowHandle, selectedColumn)
			End If

			PreviousColumn = selectedColumn
		End Sub
		
        Private Function AnonymousMethod1(ByVal selectedColumn As GridColumn, ByVal searchingString As String, ByVal matchRowHandle As Integer) As Boolean
            SetTextHighlight(searchingString, matchRowHandle, selectedColumn)
            Cell = TableView.GetCellElementByRowHandleAndColumn(matchRowHandle, selectedColumn)
            Return True
        End Function

		Public Function FindNextMatch(ByVal rowHandle As Integer, ByVal selectedColumn As GridColumn, ByVal searchingString As String, ByVal parameter As SearchParameter) As Integer
			Dim foundHandle As Integer = 0

			If (Not String.IsNullOrEmpty(searchingString)) Then
				Dim match As Boolean = False

				Dim currentHandle = Grid.GetRowVisibleIndexByHandle(rowHandle)

				If parameter = SearchParameter.Next Then
					For i As Integer = currentHandle + 1 To Grid.VisibleRowCount - 1
						Dim handle = Grid.GetRowHandleByVisibleIndex(i)

						Dim cellValue = Grid.GetCellValue(handle, selectedColumn).ToString()

						If cellValue.StartsWith(searchingString, True, System.Globalization.CultureInfo.CurrentCulture) Then
							foundHandle = handle
							match = True
							Exit For
						End If
					Next i

					If match = False Then
						foundHandle = currentHandle
					End If
				ElseIf parameter = SearchParameter.Pevious Then
					For i As Integer = currentHandle - 1 To 0 Step -1
						Dim handle = Grid.GetRowHandleByVisibleIndex(i)

						Dim cellValue = Grid.GetCellValue(handle, selectedColumn).ToString()

						If cellValue.StartsWith(searchingString, True, System.Globalization.CultureInfo.CurrentCulture) Then
							foundHandle = handle
							match = True
							Exit For
						End If
					Next i

					If match = False Then
						foundHandle = currentHandle
					End If
				End If
			End If

			Return foundHandle
		End Function

		Public Sub SetTextHighlight(ByVal valueString As Object, ByVal rowHandle As Integer, ByVal column As GridColumn)
			Dim cell = TryCast(TableView.GetCellElementByRowHandleAndColumn(rowHandle, column), CellEditor)

			If cell IsNot Nothing Then
				Dim editor = TryCast(VisualTreeHelper.GetChild(cell, 0), InplaceBaseEdit)
				Dim inplaceEditor = TryCast(editor, IInplaceBaseEdit)

				inplaceEditor.HighlightedText = TryCast(valueString, String)
				inplaceEditor.HighlightedTextCriteria = HighlightedTextCriteria.StartsWith
			End If
		End Sub

		Public Sub TextHighlightUpdate()
			Dim currentCell = TableView.GetCellElementByRowHandleAndColumn(TableView.FocusedRowHandle, Grid.CurrentColumn)

			If currentCell IsNot Nothing Then
				Cell = currentCell

				SetTextHighlight(SearchingString, TableView.FocusedRowHandle, TryCast(Grid.CurrentColumn, GridColumn))
			ElseIf Cell IsNot Nothing Then
				Dim editor = TryCast(VisualTreeHelper.GetChild(Cell, 0), InplaceBaseEdit)
				Dim inplaceEditor = TryCast(editor, IInplaceBaseEdit)

				inplaceEditor.HighlightedText = Nothing
			End If
		End Sub

		Protected Overrides Sub OnDetaching()
			MyBase.OnDetaching()
		End Sub
	End Class
End Namespace
