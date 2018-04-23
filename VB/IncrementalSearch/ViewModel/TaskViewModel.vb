Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace IncrementalSearch
	Public Class TaskViewModel

		Private _TaskData As ObservableCollection(Of Task)

		Public ReadOnly Property TaskData() As ObservableCollection(Of Task)
			Get
				If _TaskData Is Nothing Then
					_TaskData = New ObservableCollection(Of Task)()

					For i As Integer = 0 To 29
						_TaskData.Add(New Task() With {.Name = "Task " & i, .Number = i, .Date = New DateTime(2014, 10, New Random().Next(1, 31)), .IsCompleted = i Mod 2 <> 0})
					Next i
				End If
				Return _TaskData
			End Get
		End Property
	End Class

	Public Class Task
		Implements INotifyPropertyChanged
		Private _Name As String
		Private _Number As Integer
		Private _Date As DateTime
		Private _IsCompleted As Boolean

		Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

		Private Sub OnPropertyChanged(ByVal propertyName As String)
			RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
		End Sub

		Public Property Name() As String
			Get
				Return _Name
			End Get

			Set(ByVal value As String)
				_Name = value
				OnPropertyChanged("Name")
			End Set
		End Property

		Public Property Number() As Integer
			Get
				Return _Number
			End Get
			Set(ByVal value As Integer)
				_Number = value
				OnPropertyChanged("Number")
			End Set
		End Property

		Public Property [Date]() As DateTime
			Get
				Return _Date
			End Get
			Set(ByVal value As DateTime)
				_Date = value
				OnPropertyChanged("Date")
			End Set
		End Property

		Public Property IsCompleted() As Boolean
			Get
				Return _IsCompleted
			End Get
			Set(ByVal value As Boolean)
				_IsCompleted = value
				OnPropertyChanged("IsCompleted")
			End Set
		End Property
	End Class
End Namespace
