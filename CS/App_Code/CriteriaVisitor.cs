using DevExpress.Data.Filtering;
using System;
using System.Linq;

public class CriteriaVisitor : CriteriaPatcherBase {
    enum CustomVisitMode { LookingOnlyForCustomOperator, RemoveCustomOperator }
    public static CriteriaOperator RemoveCustomFunction(CriteriaOperator theOperator, string fieldName) {
        return new CriteriaVisitor(fieldName, CustomVisitMode.RemoveCustomOperator).AcceptOperator(theOperator);
    }
    public static string GetCustomFunctionOperatorValue(CriteriaOperator theOperator, string fieldName) {
        var myFunctionOperator = new CriteriaVisitor(fieldName, CustomVisitMode.LookingOnlyForCustomOperator).AcceptOperator(theOperator) as FunctionOperator;
        if(!IsNull(myFunctionOperator))
            return ((ConstantValue)myFunctionOperator.Operands[1]).Value.ToString();
        return null;
    }

    CriteriaVisitor(String fieldName, CustomVisitMode visitMode) {
        FieldName = fieldName;
        VisitMode = visitMode;
    }

    protected string FieldName { get; private set; }
    CustomVisitMode VisitMode { get; set; }

    protected override CriteriaOperator VisitFunction(FunctionOperator theOperator) {
        if(IsMyCustomFunctionOperator(theOperator))
            return VisitMode == CustomVisitMode.LookingOnlyForCustomOperator ? theOperator : null;
        if(VisitMode == CustomVisitMode.RemoveCustomOperator)
            return base.VisitFunction(theOperator);
        return null;
    }
    protected override CriteriaOperator VisitGroup(GroupOperator theOperator) {
        var operands = VisitOperands(theOperator.Operands).Where(op => !IsNull(op));
        if(VisitMode == CustomVisitMode.LookingOnlyForCustomOperator)
            return operands.FirstOrDefault(op => IsMyCustomFunctionOperator(op as FunctionOperator));
        return new GroupOperator(theOperator.OperatorType, operands);
    }
    protected bool IsMyCustomFunctionOperator(FunctionOperator criteria) {
        if(IsNull(criteria) || criteria.OperatorType != FunctionOperatorType.Custom)
            return false;
        var operands = criteria.Operands;
        if(operands.Count != 4)
            return false;
        var customFunctionName = operands[0] as ConstantValue;
        var stateOperand = operands[1] as OperandValue;
        var columnOperand = operands[2] as OperandProperty;
        if(IsNull(customFunctionName) || IsNull(stateOperand) || IsNull(columnOperand))
            return false;
        return customFunctionName.Value.ToString() == MyCustomFunctionOperator.Name && columnOperand.PropertyName == FieldName;
    }
}