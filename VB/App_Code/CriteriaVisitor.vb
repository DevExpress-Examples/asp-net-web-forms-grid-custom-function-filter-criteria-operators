Option Infer On

Imports DevExpress.Data.Filtering
Imports System
Imports System.Linq

Public Class CriteriaVisitor
    Inherits CriteriaPatcherBase

    Private Enum CustomVisitMode
        LookingOnlyForCustomOperator
        RemoveCustomOperator
    End Enum
    Public Shared Function RemoveCustomFunction(ByVal theOperator As CriteriaOperator, ByVal fieldName As String) As CriteriaOperator
        Return (New CriteriaVisitor(fieldName, CustomVisitMode.RemoveCustomOperator)).AcceptOperator(theOperator)
    End Function
    Public Shared Function GetCustomFunctionOperatorValue(ByVal theOperator As CriteriaOperator, ByVal fieldName As String) As String
        Dim myFunctionOperator = TryCast((New CriteriaVisitor(fieldName, CustomVisitMode.LookingOnlyForCustomOperator)).AcceptOperator(theOperator), FunctionOperator)
        If Not IsNull(myFunctionOperator) Then
            Return CType(myFunctionOperator.Operands(1), ConstantValue).Value.ToString()
        End If
        Return Nothing
    End Function

    Private Sub New(ByVal fieldName As String, ByVal visitMode As CustomVisitMode)
        Me.FieldName = fieldName
        Me.VisitMode = visitMode
    End Sub

    Private privateFieldName As String
    Protected Property FieldName() As String
        Get
            Return privateFieldName
        End Get
        Private Set(ByVal value As String)
            privateFieldName = value
        End Set
    End Property
    Private Property VisitMode() As CustomVisitMode

    Protected Overrides Function VisitFunction(ByVal theOperator As FunctionOperator) As CriteriaOperator
        If IsMyCustomFunctionOperator(theOperator) Then
            Return If(VisitMode = CustomVisitMode.LookingOnlyForCustomOperator, theOperator, Nothing)
        End If
        If VisitMode = CustomVisitMode.RemoveCustomOperator Then
            Return MyBase.VisitFunction(theOperator)
        End If
        Return Nothing
    End Function
    Protected Overrides Function VisitGroup(ByVal theOperator As GroupOperator) As CriteriaOperator
        Dim operands = VisitOperands(theOperator.Operands).Where(Function(op) Not IsNull(op))
        If VisitMode = CustomVisitMode.LookingOnlyForCustomOperator Then
            Return operands.FirstOrDefault(Function(op) IsMyCustomFunctionOperator(TryCast(op, FunctionOperator)))
        End If
        Return New GroupOperator(theOperator.OperatorType, operands)
    End Function
    Protected Function IsMyCustomFunctionOperator(ByVal criteria As FunctionOperator) As Boolean
        If IsNull(criteria) OrElse criteria.OperatorType <> FunctionOperatorType.Custom Then
            Return False
        End If
        Dim operands = criteria.Operands
        If operands.Count <> 4 Then
            Return False
        End If
        Dim customFunctionName = TryCast(operands(0), ConstantValue)
        Dim stateOperand = TryCast(operands(1), OperandValue)
        Dim columnOperand = TryCast(operands(2), OperandProperty)
        If IsNull(customFunctionName) OrElse IsNull(stateOperand) OrElse IsNull(columnOperand) Then
            Return False
        End If
        Return customFunctionName.Value.ToString() = MyCustomFunctionOperator.Name AndAlso columnOperand.PropertyName = FieldName
    End Function
End Class