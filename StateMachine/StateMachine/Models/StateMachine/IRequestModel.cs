namespace StateMachine.Models.StateMachine
{
    public interface IRequestModel
    { 
        public string SystemName { get; set; }
        public string WorkflowName { get; set; }
        public string CurrentStatus { get; set; }
        public List<object> Params{ get; set; }

    }

}
