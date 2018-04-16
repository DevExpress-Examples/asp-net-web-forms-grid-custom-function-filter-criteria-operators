using DevExpress.Data.Filtering;
using DevExpress.Web;
using System;

public partial class _Default : System.Web.UI.Page {
    const string
        SpecialFilterColumnFieldName = "ShippedDate",
        OtherFilterColumnFieldName = "RequiredDate";

    protected void Page_Init(object sender, EventArgs e) {
        CriteriaOperator.RegisterCustomFunction(new MyCustomFunctionOperator());
    }

    protected void Grid_ProcessColumnAutoFilter(object sender, DevExpress.Web.ASPxGridViewAutoFilterEventArgs e) {
        var grid = (ASPxGridView)sender;
        if(e.Column.FieldName == SpecialFilterColumnFieldName && e.Kind == GridViewAutoFilterEventKind.CreateCriteria) {
            grid.FilterExpression = UpdateGridFilterExpression(grid, e);
            e.Criteria = null;
        }
    }

    protected string UpdateGridFilterExpression(ASPxGridView grid, ASPxGridViewAutoFilterEventArgs e) {
        var gridCriteria = CriteriaOperator.Parse(grid.FilterExpression);
        gridCriteria = CriteriaVisitor.RemoveCustomFunction(gridCriteria, e.Column.FieldName);

        var customCriteria = new FunctionOperator(FunctionOperatorType.Custom, MyCustomFunctionOperator.Name, e.Value, new OperandProperty(e.Column.FieldName), new OperandProperty(OtherFilterColumnFieldName));
        if(ReferenceEquals(gridCriteria, null) && ReferenceEquals(customCriteria, null))
            return string.Empty;
        return GroupOperator.And(gridCriteria, customCriteria).ToString();
    }

    protected void Grid_AutoFilterCellEditorCreate(object sender, ASPxGridViewEditorCreateEventArgs e) {
        if(e.Column.FieldName == SpecialFilterColumnFieldName)
            e.EditorProperties = new ComboBoxProperties();
    }

    protected void Grid_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e) {
        var grid = (ASPxGridView)sender;
        if(e.Column.FieldName == SpecialFilterColumnFieldName) {
            InitializeFilterCombobox((ASPxComboBox)e.Editor);

            var gridCriteria = CriteriaOperator.Parse(grid.FilterExpression);
            e.Editor.Value = CriteriaVisitor.GetCustomFunctionOperatorValue(gridCriteria, e.Column.FieldName);
        }
    }

    protected void InitializeFilterCombobox(ASPxComboBox editor) {
        editor.ValueType = typeof(string);
        editor.Items.Add("Delivered in time", "OK");
        editor.Items.Add("Delivered with delay", "NOTOK");
        editor.Items.Add("Delivery lost", "ISSUE");
    }
}