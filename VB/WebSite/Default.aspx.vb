Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports DevExpress.Web
Imports System.Collections.Generic

Partial Public Class _Default
	Inherits System.Web.UI.Page

	Private isEditing As Boolean = False

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		'Create the grid's data source on the first load
		If Not IsPostBack AndAlso Not IsCallback Then
			Session("GridDataSource") = CreateDataSource()
		End If
		'Populate the grid with data on each page load
		If Session("GridDataSource") IsNot Nothing Then
			ASPxGridView1.DataSource = TryCast(Session("GridDataSource"), DataTable)
			ASPxGridView1.DataBind()
		End If
	End Sub

	Private Function CreateDataSource() As DataTable
		Dim dataTable As New DataTable("MyTable")
		dataTable.Columns.Add("ID", GetType(Integer))
		dataTable.Columns.Add("Song", GetType(String))
		dataTable.Columns.Add("Genre", GetType(String))
		dataTable.PrimaryKey = New DataColumn() { dataTable.Columns("ID") }
		For i As Integer = 0 To 14
			dataTable.Rows.Add(New Object() { i, "Song" & i.ToString(), "Rock" })
		Next i
		Return dataTable
	End Function

	Protected Sub ASPxGridView1_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs)
		Dim grid As ASPxGridView = TryCast(sender, ASPxGridView)
		'Switch the grid to edit mode for the last selected row
		If e.Parameters = "StartEditing" Then
			Dim lastSelectedRowKeyValue As Object = grid.GetSelectedFieldValues("ID")(grid.Selection.Count - 1)
			Dim lastSelectedRowVisibleIndex As Integer = grid.FindVisibleIndexByKeyValue(lastSelectedRowKeyValue)
			grid.StartEdit(lastSelectedRowVisibleIndex)
		End If
	End Sub

	Protected Sub ASPxGridView1_StartRowEditing(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxStartRowEditingEventArgs)
		isEditing = True
	End Sub

	Protected Sub ASPxGridView1_CellEditorInitialize(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewEditorEventArgs)
		Dim grid As ASPxGridView = TryCast(sender, ASPxGridView)
		'Make sure that the code initializing editor values in EditForm is executed only once - when switching the grid to edit mode
		If Not (grid.IsEditing AndAlso isEditing) Then
			Return
		End If
		If grid.Selection.Count = 1 OrElse e.Column.ReadOnly Then
			Return
		End If
		'Initialize an editor's value depending upon the value's uniqueness within a column
		e.Editor.Value = If(IsCommonValueForAllSelectedRows(e.Column, e.Value), e.Value, Nothing)

	End Sub

	Private Function IsCommonValueForAllSelectedRows(ByVal column As DevExpress.Web.GridViewDataColumn, ByVal value As Object) As Boolean
		'Determine whether the passed value is common for all rows within the specified column
		Dim res As Boolean = True
		Dim selectedRowValues As List(Of Object) = ASPxGridView1.GetSelectedFieldValues(column.FieldName)
		For i As Integer = 0 To selectedRowValues.Count - 1
			If selectedRowValues(i).ToString() <> value.ToString() Then
				res = False
				Exit For
			End If
		Next i
		Return res
	End Function

	Protected Sub ASPxGridView1_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs)
		Dim dt As DataTable = TryCast(Session("GridDataSource"), DataTable)

		'Modify row values after a single grid row has been edited
		If ASPxGridView1.Selection.Count = 1 Then
			Dim row As DataRow = TryCast(dt.Rows.Find(e.Keys("ID")), DataRow)
			row("Song") = e.NewValues("Song")
			row("Genre") = e.NewValues("Genre")
		End If
		'Modify row values after multiple grid rows have been edited
		If ASPxGridView1.Selection.Count > 1 Then
			'Obtain key field values of the selected rows
			Dim selectedRowKeys As List(Of Object) = ASPxGridView1.GetSelectedFieldValues("ID")
			For i As Integer = 0 To selectedRowKeys.Count - 1
				'Find a row in the data table by the row's key field value
				Dim row As DataRow = TryCast(dt.Rows.Find(selectedRowKeys(i)), DataRow)
				'Modify data rows by leaving old values (if the EditForm's editors are left blank) or saving new values entered into editors
				row("Song") = If(e.NewValues("Song") Is Nothing, row("Song"), e.NewValues("Song"))
				row("Genre") = If(e.NewValues("Genre") Is Nothing, row("Genre"), e.NewValues("Genre"))
				Continue For
			Next i
		End If

		dt.AcceptChanges()
		Session("GridDataSource") = dt

		'Close the grid's EditForm and avoid default update processing from being executed
		e.Cancel = True
		ASPxGridView1.CancelEdit()
	End Sub
End Class
