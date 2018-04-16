Option Infer On

Imports DevExpress.Data.Filtering
Imports DevExpress.Web
Imports System

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Private Const SpecialFilterColumnFieldName As String = "ShippedDate", OtherFilterColumnFieldName As String = "RequiredDate"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        CriteriaOperator.RegisterCustomFunction(New MyCustomFunctionOperator())
    End Sub

    Protected Sub Grid_ProcessColumnAutoFilter(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewAutoFilterEventArgs)
        Dim grid = DirectCast(sender, ASPxGridView)
        If e.Column.FieldName = SpecialFilterColumnFieldName AndAlso e.Kind = GridViewAutoFilterEventKind.CreateCriteria Then
            grid.FilterExpression = UpdateGridFilterExpression(grid, e)
            e.Criteria = Nothing
        End If
    End Sub

    Protected Function UpdateGridFilterExpression(ByVal grid As ASPxGridView, ByVal e As ASPxGridViewAutoFilterEventArgs) As String
        Dim gridCriteria = CriteriaOperator.Parse(grid.FilterExpression)
        gridCriteria = CriteriaVisitor.RemoveCustomFunction(gridCriteria, e.Column.FieldName)

        Dim customCriteria = New FunctionOperator(FunctionOperatorType.Custom, MyCustomFunctionOperator.Name, e.Value, New OperandProperty(e.Column.FieldName), New OperandProperty(OtherFilterColumnFieldName))
        If ReferenceEquals(gridCriteria, Nothing) AndAlso ReferenceEquals(customCriteria, Nothing) Then
            Return String.Empty
        End If
        Return GroupOperator.And(gridCriteria, customCriteria).ToString()
    End Function

    Protected Sub Grid_AutoFilterCellEditorCreate(ByVal sender As Object, ByVal e As ASPxGridViewEditorCreateEventArgs)
        If e.Column.FieldName = SpecialFilterColumnFieldName Then
            e.EditorProperties = New ComboBoxProperties()
        End If
    End Sub

    Protected Sub Grid_AutoFilterCellEditorInitialize(ByVal sender As Object, ByVal e As ASPxGridViewEditorEventArgs)
        Dim grid = DirectCast(sender, ASPxGridView)
        If e.Column.FieldName = SpecialFilterColumnFieldName Then
            InitializeFilterCombobox(CType(e.Editor, ASPxComboBox))

            Dim gridCriteria = CriteriaOperator.Parse(grid.FilterExpression)
            e.Editor.Value = CriteriaVisitor.GetCustomFunctionOperatorValue(gridCriteria, e.Column.FieldName)
        End If
    End Sub

    Protected Sub InitializeFilterCombobox(ByVal editor As ASPxComboBox)
        editor.ValueType = GetType(String)
        editor.Items.Add("Delivered in time", "OK")
        editor.Items.Add("Delivered with delay", "NOTOK")
        editor.Items.Add("Delivery lost", "ISSUE")
    End Sub
End Class