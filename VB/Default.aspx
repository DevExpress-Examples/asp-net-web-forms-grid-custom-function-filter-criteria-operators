<%@ Page Language="vb" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.v16.2, Version=16.2.17.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ASPxGridView - How to apply Custom Function Filter Criteria Operator</title>
</head>
<body>
    <h2>ASPxGridView - How to apply Custom Function Filter Criteria Operator</h2>

    <form id="form1" runat="server">
        <div>
            <dx:ASPxGridView ID="Grid" runat="server" DataSourceID="SqlDataSource1"
                OnAutoFilterCellEditorCreate="Grid_AutoFilterCellEditorCreate"
                OnAutoFilterCellEditorInitialize="Grid_AutoFilterCellEditorInitialize"
                OnProcessColumnAutoFilter="Grid_ProcessColumnAutoFilter">
                <Columns>
                    <dx:GridViewDataTextColumn FieldName="ShipName" />
                    <dx:GridViewDataTextColumn FieldName="ProductName" />
                    <dx:GridViewDataDateColumn FieldName="ShippedDate" Caption="Date of delivery" HeaderStyle-BackColor="Wheat" />
                    <dx:GridViewDataDateColumn FieldName="RequiredDate" Caption="Expiration date of delivery"
                        HeaderStyle-BackColor="Wheat" Settings-AllowAutoFilter="False" />
                </Columns>
                <Settings ShowFilterRow="true" />
            </dx:ASPxGridView>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" 
                SelectCommand="SELECT [ShipName], [ProductName], [ShippedDate], [RequiredDate] FROM [Invoices]" />
        </div>
    </form>
</body>
</html>