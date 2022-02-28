using Stateless;
using Stateless.Reflection;
 

namespace StateMachine.Models.StateMachine
{

    public class StateMachineModel 
    {
        private readonly StateMachine<string, string> stateMachine = null; 

        public StateMachineModel(StatusModelWrapper statusModel)
        { 
            stateMachine = new StateMachine<string, string>(statusModel.Status);

            foreach (var status in statusModel.StatusModels)
            {
                stateMachine.Configure(status.MethodModel.CurrentStatus) 
                    .OnEntry((model) => { if (status.MethodModel.OnEntry != null) status.MethodModel.OnEntry(model); })
                    .OnExit((model) => { if (status.MethodModel.OnExit != null) status.MethodModel.OnExit(model); });
  
                foreach (var permitIf in status.IfClause)
                {
                    stateMachine.Configure(status.MethodModel.CurrentStatus)
                        .PermitIf(permitIf.Trigger, permitIf.Status, () => 
                        { 
                            if (permitIf.GuardClauseDelegates != null) 
                                return permitIf.GuardClauseDelegates(); 
                            return true; 
                        });
                }

                foreach (var ifReEntry in status.PermitReentryIfModels)
                {
                    stateMachine.Configure(status.MethodModel.CurrentStatus)
                        .PermitReentryIf(ifReEntry.Trigger, () =>
                        {
                            if (ifReEntry.GuardClauseDelegates != null)
                                return ifReEntry.GuardClauseDelegates();
                            return true;
                        });
                }
            }

            stateMachine.OnUnhandledTrigger((state, trigger) => { if (statusModel.StatusModels[0].OnUnhandledTrigger != null) 
                    statusModel.StatusModels[0].OnUnhandledTrigger(state, trigger); });
        }

        public bool TryFireTrigger(string trigger)
        {
            if (!stateMachine.CanFire(trigger))
            {
                return false;
            }
             
            stateMachine.Fire(trigger);
            return true;
        }

        public string GetState
        {
            get
            {
                return stateMachine.State;
            }
        }

        public StateMachineInfo GetInfo()
        {
            StateMachineInfo info = stateMachine.GetInfo();
            return info;
        }
    }

}
