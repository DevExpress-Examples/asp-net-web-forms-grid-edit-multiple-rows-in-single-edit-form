<%@ Page Language="VB" AutoEventWireup="true"  CodeFile="Default.aspx.vb" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.14.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>

    
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        Select one or several grid rows and click the Edit button to modify row values.<br /><br />

        <dx:aspxgridview id="ASPxGridView1" runat="server" AutoGenerateColumns="False" ClientInstanceName="grid" KeyFieldName="ID" OnCellEditorInitialize="ASPxGridView1_CellEditorInitialize" OnCustomCallback="ASPxGridView1_CustomCallback" OnRowUpdating="ASPxGridView1_RowUpdating" OnStartRowEditing="ASPxGridView1_StartRowEditing">
            <Columns>
                <dx:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0">
                </dx:GridViewCommandColumn>

                <dx:GridViewDataTextColumn FieldName="ID" VisibleIndex="1" ReadOnly="True">
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>

                <dx:GridViewDataTextColumn FieldName="Song" VisibleIndex="2">
                    <PropertiesTextEdit NullText="Type new value here">
                    </PropertiesTextEdit>
                </dx:GridViewDataTextColumn>

                <dx:GridViewDataTextColumn FieldName="Genre" VisibleIndex="3">
                    <PropertiesTextEdit NullText="Type new value here">
                    </PropertiesTextEdit>
                </dx:GridViewDataTextColumn>
            </Columns>
            <SettingsEditing Mode="PopupEditForm" EditFormColumnCount="1" PopupEditFormModal="True" PopupEditFormHorizontalAlign="WindowCenter" PopupEditFormVerticalAlign="WindowCenter" />
        </dx:aspxgridview>
        
        <br />
        
        <dx:ASPxButton ID="ASPxButton1" runat="server" AutoPostBack="False" Text="Edit">
            <ClientSideEvents Click="function(s, e) {
                if(grid.GetSelectedRowCount() == 0) alert('No grid row is selected.');
	            else grid.PerformCallback('StartEditing');
            }" />
        </dx:ASPxButton>
    </form>
</body>
</html>
