namespace StateMachine.Models.StateMachine
{
    public class RequestModel : IRequestModel
    {

        public string CurrentStatus { get; set; }
        public string SystemName { get; set; }
        public string WorkflowName { get; set; }
        public List<object> Params { get; set; } = new List<object>();
    }
    public class RequestViewModel
    {
        public int Id { get; set; }
        public string RequestText { get; set; }
    }
    public class RequestViewModelXXXXX
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
