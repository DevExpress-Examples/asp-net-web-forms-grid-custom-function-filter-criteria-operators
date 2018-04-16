Option Infer On

Imports DevExpress.Data.Filtering
Imports System

Public Class MyCustomFunctionOperator
    Implements ICustomFunctionOperator

    Public Shared ReadOnly Property Name() As String
        Get
            Return "MyFunctionOperator"
        End Get
    End Property

    Private Function ICustomFunctionOperator_Evaluate(ParamArray ByVal operands() As Object) As Object Implements ICustomFunctionOperator.Evaluate
        If operands.Length = 3 Then
            Dim state = TryCast(operands(0), String)
            If Not String.IsNullOrEmpty(state) Then
                Dim shippedDate = If(Not ReferenceEquals(operands(1), Nothing), Convert.ToDateTime(operands(1)), Date.MaxValue)
                Dim requiredDate = If(Not ReferenceEquals(operands(2), Nothing), Convert.ToDateTime(operands(2)), Date.MaxValue)
                Select Case state
                    Case "OK"
                        Return shippedDate <= requiredDate
                    Case "NOTOK"
                        Return shippedDate > requiredDate AndAlso shippedDate <> DateTime.MaxValue
                    Case "ISSUE"
                        Return shippedDate = Date.MaxValue
                End Select
            End If
        End If
        Return True
    End Function

    Private ReadOnly Property ICustomFunctionOperator_Name() As String Implements ICustomFunctionOperator.Name
        Get
            Return Name
        End Get
    End Property
    Private Function ICustomFunctionOperator_ResultType(ParamArray ByVal operands() As Type) As Type Implements ICustomFunctionOperator.ResultType
        Return GetType(Boolean)
    End Function
End Class