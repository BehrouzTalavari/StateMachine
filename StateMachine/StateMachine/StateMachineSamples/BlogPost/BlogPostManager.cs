namespace StateMachine.StateMachineSamples
{
    public class BlogPostManager
    {
        private BlogPostStateMachine _stateMachine;
        public BlogPostManager()
        {
            configureWorkflow();
        }

        private void configureWorkflow()
        {
            _stateMachine = new BlogPostStateMachine();

            _stateMachine.GuardClauseFromBeginToInProgressUsingTriggerSave = () => { return UserCanPost; };
            _stateMachine.OnBeginExit = () => { /* save data + save state + send an email to admin */ };

            _stateMachine.GuardClauseFromInProgressToPublishedUsingTriggerAccept = () => { return UserIsAdmin; };
            _stateMachine.GuardClauseFromInProgressToRejectedUsingTriggerReject = () => { return UserIsAdmin; };
            _stateMachine.GuardClauseFromInProgressToInProgressUsingTriggerSave = () => { return UserHasEditRights; };
            _stateMachine.OnInProgressExit = () => { /* save data + save state + send an email to user */ };

            _stateMachine.OnPublishedExit = () => { /* save data + save state + send an email to admin */ };
            _stateMachine.GuardClauseFromPublishedToInProgressUsingTriggerRequireEdit = () => { return UserHasEditRights; };

            _stateMachine.OnRejectedExit = () => { /* save data + save state + send an email to admin */ };
            _stateMachine.GuardClauseFromRejectedToInProgressUsingTriggerRequireEdit = () => { return UserHasEditRights; };
        }

        public bool UserIsAdmin
        {
            get
            {
                return true; // TODO: Evaluate if user is an admin.
            }
        }

        public bool UserCanPost
        {
            get
            {
                return true; // TODO: Evaluate if user is authenticated
            }
        }

        public bool UserHasEditRights
        {
            get
            {
                return true; // TODO: Evaluate if user is owner or admin
            }
        }

        // User actions
        public string Save(string Req) 
        {
            if (_stateMachine.TryFireTrigger(BlogPostStateMachine.Trigger.Save)) return $"Post Saved {Req}";
            return "State Not Changed";
        }
        public string RequireEdit() {
            if (_stateMachine.TryFireTrigger(BlogPostStateMachine.Trigger.RequireEdit)) return "Post Require Edit";
            return "State Not Changed";
        }

        // Admin actions
        public string Accept() {
            if (_stateMachine.TryFireTrigger(BlogPostStateMachine.Trigger.Accept)) return "Post Accepted";
            return "State Not Changed";  
        }
        public string Reject()  
        {
            if (_stateMachine.TryFireTrigger(BlogPostStateMachine.Trigger.Reject)) return "Post Rejected";
            return "State Not Changed";  
        }
}

}
