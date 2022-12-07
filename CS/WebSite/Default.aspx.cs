using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DevExpress.Web;
using System.Collections.Generic;

public partial class _Default : System.Web.UI.Page 
{
    bool isEditing = false;

    protected void Page_Load(object sender, EventArgs e){
        //Create the grid's data source on the first load
        if (!IsPostBack && !IsCallback) Session["GridDataSource"] = CreateDataSource();
        //Populate the grid with data on each page load
        if (Session["GridDataSource"] != null){
            ASPxGridView1.DataSource = Session["GridDataSource"] as DataTable;
            ASPxGridView1.DataBind();
        }
    }

    private DataTable CreateDataSource(){
        DataTable dataTable = new DataTable("MyTable");
        dataTable.Columns.Add("ID", typeof(int));
        dataTable.Columns.Add("Song", typeof(string));
        dataTable.Columns.Add("Genre", typeof(string));
        dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["ID"] };
        for (int i = 0; i < 15; i++)
        {
            dataTable.Rows.Add(new object[] { i, "Song" + i.ToString(), "Rock" });
        }
        return dataTable;
    }

    protected void ASPxGridView1_CustomCallback(object sender, DevExpress.Web.ASPxGridViewCustomCallbackEventArgs e){
        ASPxGridView grid = sender as ASPxGridView;
        //Switch the grid to edit mode for the last selected row
        if (e.Parameters == "StartEditing"){
            object lastSelectedRowKeyValue = grid.GetSelectedFieldValues("ID")[grid.Selection.Count - 1];
            int lastSelectedRowVisibleIndex = grid.FindVisibleIndexByKeyValue(lastSelectedRowKeyValue);
            grid.StartEdit(lastSelectedRowVisibleIndex);
        }
    }

    protected void ASPxGridView1_StartRowEditing(object sender, DevExpress.Web.Data.ASPxStartRowEditingEventArgs e){
        isEditing = true;
    }

    protected void ASPxGridView1_CellEditorInitialize(object sender, DevExpress.Web.ASPxGridViewEditorEventArgs e){
        ASPxGridView grid = sender as ASPxGridView;
        //Make sure that the code initializing editor values in EditForm is executed only once - when switching the grid to edit mode
        if (!(grid.IsEditing && isEditing)) return;
        if (grid.Selection.Count == 1 || e.Column.ReadOnly) return;
        //Initialize an editor's value depending upon the value's uniqueness within a column
        e.Editor.Value = IsCommonValueForAllSelectedRows(e.Column, e.Value) ? e.Value : null;

    }

    private bool IsCommonValueForAllSelectedRows(DevExpress.Web.GridViewDataColumn column, object value){
        //Determine whether the passed value is common for all rows within the specified column
        bool res = true;
        List<object> selectedRowValues = ASPxGridView1.GetSelectedFieldValues(column.FieldName);
        for (int i = 0; i < selectedRowValues.Count; i++){
            if (selectedRowValues[i].ToString() != value.ToString()){
                res = false;
                break;
            }
        }
        return res;
    }

    protected void ASPxGridView1_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e){
        DataTable dt = Session["GridDataSource"] as DataTable;

        //Modify row values after a single grid row has been edited
        if (ASPxGridView1.Selection.Count == 1){
            DataRow row = dt.Rows.Find(e.Keys["ID"]) as DataRow;
            row["Song"] = e.NewValues["Song"];
            row["Genre"] = e.NewValues["Genre"];
        }
        //Modify row values after multiple grid rows have been edited
        if (ASPxGridView1.Selection.Count > 1){
            //Obtain key field values of the selected rows
            List<object> selectedRowKeys = ASPxGridView1.GetSelectedFieldValues("ID");
            for (int i = 0; i < selectedRowKeys.Count; i++){
                //Find a row in the data table by the row's key field value
                DataRow row = dt.Rows.Find(selectedRowKeys[i]) as DataRow;
                //Modify data rows by leaving old values (if the EditForm's editors are left blank) or saving new values entered into editors
                row["Song"] = (e.NewValues["Song"] == null) ? row["Song"] : e.NewValues["Song"];
                row["Genre"] = (e.NewValues["Genre"] == null) ? row["Genre"] : e.NewValues["Genre"];
                continue;
            }
        }

        dt.AcceptChanges();
        Session["GridDataSource"] = dt;

        //Close the grid's EditForm and avoid default update processing from being executed
        e.Cancel = true;
        ASPxGridView1.CancelEdit();
    }
}
