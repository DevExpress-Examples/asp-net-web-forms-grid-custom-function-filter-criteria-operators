using DevExpress.Data.Filtering;
using System;

public class MyCustomFunctionOperator : ICustomFunctionOperator {
    public static string Name { get { return "MyFunctionOperator"; } }

    object ICustomFunctionOperator.Evaluate(params object[] operands) {
        if(operands.Length == 3) {
            var state = operands[0] as string;
            if(!string.IsNullOrEmpty(state)) {
                var shippedDate = !ReferenceEquals(operands[1], null) ? Convert.ToDateTime(operands[1]) : DateTime.MaxValue;
                var requiredDate = !ReferenceEquals(operands[2], null) ? Convert.ToDateTime(operands[2]) : DateTime.MaxValue;
                switch(state) {
                    case "OK":
                        return shippedDate <= requiredDate;
                    case "NOTOK":
                        return shippedDate > requiredDate && shippedDate != DateTime.MaxValue;
                    case "ISSUE":
                        return shippedDate == DateTime.MaxValue;
                }
            }
        }
        return true;
    }

    string ICustomFunctionOperator.Name { get { return Name; } }
    Type ICustomFunctionOperator.ResultType(params Type[] operands) { return typeof(bool); }
}