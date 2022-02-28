using Stateless.Graph;

namespace StateMachine.Models.StateMachine
{
    public abstract class WorkFlowManager<T> : StateMachineModel where T : class, IRequestModel, new()
    {
        private readonly T _requestModel;
        public WorkFlowManager(StatusModelWrapper statusModelWrapper) : base(statusModelWrapper)
        {
        }
        protected WorkFlowManager(T requestModel)
            : this(new StatusModelWrapper()
            {
                Status = requestModel.CurrentStatus,
                StatusModels = StatusGenerator.GetStatusModels(requestModel)
            })
        {
            _requestModel = requestModel;
            ConfigureWorkFlow(requestModel);
        }

        public WorkFlowManager<T> AddOnEntryStatus(string status, Action<object> method)
        {
            var findedstatus = _model.First(x => x.MethodModel.CurrentStatus == status);
            findedstatus.MethodModel.OnEntry = (model) =>
            {
                method(model);
            };

            return this;
        }

        public WorkFlowManager<T> AddOnExitStatus(string status, Action<object, T> method)
        {
            var findedstatus = _model.First(x => x.MethodModel.CurrentStatus == status);
            findedstatus.MethodModel.OnExit = (model) =>
            {
                method(model, _requestModel);
            };

            return this;
        }
        public WorkFlowManager<T> AddIfClause(string status, string trigger, Func<bool> method)
        {
            var findedstatus = _model.First(x => x.MethodModel.CurrentStatus == status);
            findedstatus.IfClause.First(x => x.Trigger == trigger).GuardClauseDelegates = () => { return method(); };

            return this;
        }

        public WorkFlowManager<T> AddReentryIf(string status, string trigger, Func<bool> method)
        {
            var findedstatus = _model.First(x => x.MethodModel.CurrentStatus == status);
            findedstatus.PermitReentryIfModels.First(x => x.Trigger == trigger).GuardClauseDelegates = () => { return method(); };

            return this;
        }

        public string GetGraphJsonModel()
        {
            return UmlDotGraph.Format(GetInfo());
        }

        public void AddTrasitionMethod(Action<object> action)
        {
            SetTransitionedMethod(action);
        }

        public abstract void ConfigureWorkFlow(T workFlowModel);
        public virtual string Invoke(string trigger, T workFlowModel)
        {
            if (TryFireTrigger(trigger))
            {
                workFlowModel.CurrentStatus = CurrentStatus;
                return CurrentStatus;

            }
            return string.Empty;
        }


    }

}
