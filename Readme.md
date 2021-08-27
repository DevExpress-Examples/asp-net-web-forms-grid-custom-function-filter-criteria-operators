<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128533360/16.2.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T546944)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [CriteriaPatcherBase.cs](./CS/App_Code/CriteriaPatcherBase.cs) (VB: [CriteriaPatcherBase.vb](./VB/App_Code/CriteriaPatcherBase.vb))
* [CriteriaVisitor.cs](./CS/App_Code/CriteriaVisitor.cs) (VB: [CriteriaVisitor.vb](./VB/App_Code/CriteriaVisitor.vb))
* [MyCustomFunctionOperator.cs](./CS/App_Code/MyCustomFunctionOperator.cs) (VB: [MyCustomFunctionOperator.vb](./VB/App_Code/MyCustomFunctionOperator.vb))
* [Default.aspx](./CS/Default.aspx) (VB: [Default.aspx](./VB/Default.aspx))
* [Default.aspx.cs](./CS/Default.aspx.cs) (VB: [Default.aspx.vb](./VB/Default.aspx.vb))
<!-- default file list end -->
# ASPxGridView - How to apply Custom Function Filter Criteria Operator
<!-- run online -->
**[[Run Online]](https://codecentral.devexpress.com/t546944/)**
<!-- run online end -->


<p>Some complicated filter cases require visiting every leaf of the filter criteria tree when a <a href="https://documentation.devexpress.com/eXpressAppFramework/113480/Concepts/Filtering/Custom-Function-Criteria-Operators">custom function criteria operator</a> is used. <br>This example shows how it can be implemented via <a href="https://www.devexpress.com/Support/Center/Question/Details/T320172/the-base-implementation-of-the-iclientcriteriavisitor-interface-for-the-criteriaoperator">CriteriaPatchBase's</a> class descendant.<br>The main idea is to operate with ASPxGridView's <a href="https://documentation.devexpress.com/AspNet/DevExpressWebASPxGridBase_FilterExpressiontopic.aspx">FilterExpression</a>:<br>1. If FilterExpression has a custom function operator, it's necessary to find exactly this custom operator and replace its value with an actual one. <br>In the case of Auto Filter Row, this can be done in the <a href="https://documentation.devexpress.com/#AspNet/DevExpressWebASPxGridView_ProcessColumnAutoFiltertopic">ASPxGridView.ProcessColumnAutoFilter</a> event handler as shown below:</p>


```cs
protected void Grid_ProcessColumnAutoFilter(object sender, DevExpress.Web.ASPxGridViewAutoFilterEventArgs e) {
    var grid = (ASPxGridView)sender;
    if(e.Column.FieldName == SpecialFilterColumnFieldName && e.Kind == GridViewAutoFilterEventKind.CreateCriteria) {
        grid.FilterExpression = UpdateGridFilterExpression(grid, e);
        e.Criteria = null;
    }
}
```


<p>Note that <strong>e.Criteria</strong> in this case must be set to null.</p>
<p>2. Then, assign the custom criteria operator value to the filter editor.<br>The right place of the code to do this is the <a href="https://documentation.devexpress.com/#AspNet/DevExpressWebASPxGridView_AutoFilterCellEditorInitializetopic">ASPxGridView.AutoFilterCellEditorInitialize</a> event handler. For example, it can be implemented in the following manner:</p>


```cs
protected void Grid_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e) {
        var grid = (ASPxGridView)sender;
        if(e.Column.FieldName == SpecialFilterColumnFieldName) {
            var gridCriteria = CriteriaOperator.Parse(grid.FilterExpression);
            e.Editor.Value = CriteriaVisitor.GetCustomFunctionOperatorValue(gridCriteria, e.Column.FieldName);
        }
    }
```


<p>where <strong>CriteriaVisitor </strong>is <a href="https://www.devexpress.com/Support/Center/Question/Details/T320172/the-base-implementation-of-the-iclientcriteriavisitor-interface-for-the-criteriaoperator">CriteriaPatchBase's</a> class descendant (see implementation details).  <strong> </strong><br><br><strong>See also: </strong><br><a href="https://www.devexpress.com/Support/Center/Question/Details/T320172/the-base-implementation-of-the-iclientcriteriavisitor-interface-for-the-criteriaoperator">The base implementation of the IClientCriteriaVisitor interface for the CriteriaOperator expression patcher</a></p>


<h3>Description</h3>

<p>The <strong>CriteriaVisitor&nbsp;</strong>class provides&nbsp;<em>only</em>&nbsp;two public static methods:&nbsp;<strong><em>RemoveCustomFunction</em></strong><strong>&nbsp;</strong>and&nbsp;<strong><em>GetCustomFunctionOperatorValue</em></strong>.&nbsp;<br>The first one returns CriteriaOperator without a custom operator; the second one returns a string value of a custom function criteria operator.<br>There are also two overridden methods:&nbsp;<strong><em>VisitFunction</em></strong>&nbsp;and&nbsp;<strong><em>VisitGroup</em></strong>. They return specific values depending on the&nbsp;<strong><em>VisitorMode</em></strong><strong>&nbsp;</strong>(<em>LookingOnlyForCustomOperator</em>&nbsp;or&nbsp;<em>RemoveCustomOperator</em>).<br>The IsMyCustomFunctionOperator method&nbsp;allows determining whether FunctionOperator is exactly that custom one (<strong>MyCustomFunctionOperator</strong>). <br><br>The <strong>MyCustomFunctionOperator&nbsp;</strong>class contains some custom filtering logic, which is implemented in the&nbsp;<strong><em>Evaluate</em></strong><strong>&nbsp;</strong>method.</p>

<br/>


