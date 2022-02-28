using Stateless;

namespace StateMachine.StateMachineSamples
{
    public class BlogPostStateMachine
    {
        public delegate void UnhandledTriggerDelegate(State state, Trigger trigger);
        public delegate void EntryExitDelegate();
        public delegate bool GuardClauseDelegate();

        public enum Trigger
        {
            Save,
            RequireEdit,
            Accept,
            Reject,
        }

        public enum State
        {
            Begin,
            InProgress,
            Published,
            Rejected,
        }

        private readonly StateMachine<State, Trigger> stateMachine = null;

        public EntryExitDelegate OnBeginEntry = null;
        public EntryExitDelegate OnBeginExit = null;
        public EntryExitDelegate OnInProgressEntry = null;
        public EntryExitDelegate OnInProgressExit = null;
        public EntryExitDelegate OnPublishedEntry = null;
        public EntryExitDelegate OnPublishedExit = null;
        public EntryExitDelegate OnRejectedEntry = null;
        public EntryExitDelegate OnRejectedExit = null;
        public GuardClauseDelegate GuardClauseFromBeginToInProgressUsingTriggerSave = null;
        public GuardClauseDelegate GuardClauseFromInProgressToPublishedUsingTriggerAccept = null;
        public GuardClauseDelegate GuardClauseFromInProgressToRejectedUsingTriggerReject = null;
        public GuardClauseDelegate GuardClauseFromInProgressToInProgressUsingTriggerSave = null;
        public GuardClauseDelegate GuardClauseFromPublishedToInProgressUsingTriggerRequireEdit = null;
        public GuardClauseDelegate GuardClauseFromRejectedToInProgressUsingTriggerRequireEdit = null;
        public UnhandledTriggerDelegate OnUnhandledTrigger = null;

        public BlogPostStateMachine()
        {
            stateMachine = new StateMachine<State, Trigger>(State.Begin);

            stateMachine.Configure(State.Begin)
              .OnEntry(() => { if (OnBeginEntry != null) OnBeginEntry(); })
              .OnExit(() => { if (OnBeginExit != null) OnBeginExit(); })
              .PermitIf(Trigger.Save, State.InProgress, () =>
               {
                   if (GuardClauseFromBeginToInProgressUsingTriggerSave != null)
                       return GuardClauseFromBeginToInProgressUsingTriggerSave();

                   return true;
               })
            ;

            stateMachine.Configure(State.InProgress)
              .OnEntry(() => { if (OnInProgressEntry != null) OnInProgressEntry(); })
              .OnExit(() => { if (OnInProgressExit != null) OnInProgressExit(); })
              .PermitIf(Trigger.Accept, State.Published, () => { if (GuardClauseFromInProgressToPublishedUsingTriggerAccept != null) return GuardClauseFromInProgressToPublishedUsingTriggerAccept(); return true; })
              .PermitIf(Trigger.Reject, State.Rejected, () => { if (GuardClauseFromInProgressToRejectedUsingTriggerReject != null) return GuardClauseFromInProgressToRejectedUsingTriggerReject(); return true; })
              .PermitReentryIf(Trigger.Save, () => 
              {
                  if (GuardClauseFromInProgressToInProgressUsingTriggerSave != null) 
                      return GuardClauseFromInProgressToInProgressUsingTriggerSave(); 
                  return true;
              })
            ;

            stateMachine.Configure(State.Published)
              .OnEntry(() => { if (OnPublishedEntry != null) OnPublishedEntry(); })
              .OnExit(() => { if (OnPublishedExit != null) OnPublishedExit(); })
              .PermitIf(Trigger.RequireEdit, State.InProgress, () => { if (GuardClauseFromPublishedToInProgressUsingTriggerRequireEdit != null) return GuardClauseFromPublishedToInProgressUsingTriggerRequireEdit(); return true; })
            ;
            stateMachine.Configure(State.Rejected)
              .OnEntry(() => { if (OnRejectedEntry != null) OnRejectedEntry(); })
              .OnExit(() => { if (OnRejectedExit != null) OnRejectedExit(); })
              .PermitIf(Trigger.RequireEdit, State.InProgress, () => { if (GuardClauseFromRejectedToInProgressUsingTriggerRequireEdit != null) return GuardClauseFromRejectedToInProgressUsingTriggerRequireEdit(); return true; })
            ;

            stateMachine.OnUnhandledTrigger((state, trigger) => { if (OnUnhandledTrigger != null) OnUnhandledTrigger(state, trigger); });
        }

        public bool TryFireTrigger(Trigger trigger)
        {
            if (!stateMachine.CanFire(trigger))
            {
                return false;
            }
            stateMachine.Fire(trigger);
            return true;
        }

        public State GetState
        {
            get
            {
                return stateMachine.State;
            }
        }
    }

}
