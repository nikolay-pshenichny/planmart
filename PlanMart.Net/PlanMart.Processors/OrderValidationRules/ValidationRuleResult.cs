namespace PlanMart.Processors.OrderValidationRules
{
    public class ValidationRuleResult
    {
        public ValidationRuleResult()
        {
            //
        }

        public ValidationRuleResult(bool valid, string message = null)
        {
            this.Valid = valid;
            this.Message = message;
        }

        public bool Valid { get; set; }

        public string Message { get; set; }

    }
}
