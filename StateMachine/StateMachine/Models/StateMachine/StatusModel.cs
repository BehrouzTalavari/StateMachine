using Stateless;

namespace StateMachine.Models.StateMachine
{
    public class StatusModelWrapper
    {
        public string Status { get; set; }
        public List<StatusModel> StatusModels { get; set; } = new List<StatusModel>();
    }
    public class StatusModel
    {
        public int Id { get; set; }
        public MethodModel MethodModel { get; set; }
        public List<TriggerModel> IfClause { get; set; } = new List<TriggerModel>();
        public List<PermitReentryIfModel> PermitReentryIfModels { get; set; } = new List<PermitReentryIfModel>();
        public delegate void UnhandledTriggerDelegate(string state, string trigger);
        public UnhandledTriggerDelegate OnUnhandledTrigger = null;
    }
    public class MethodModel
    {
        public string CurrentStatus { get; set; }
        public delegate void UnhandledTriggerDelegate(string state, string trigger);
        public delegate void EntryExitDelegate(object stateMachineModel);

        public EntryExitDelegate OnEntry = null;
        public EntryExitDelegate OnExit = null;
    }

    public class TriggerModel
    {
        public string Trigger { get; set; }
        public string Status { get; set; }
        public delegate bool GuardClauseDelegate();
        public GuardClauseDelegate GuardClauseDelegates = null;
    }

    public class PermitReentryIfModel
    {
        public string Trigger { get; set; }
        public delegate bool GuardClauseDelegate();
        public GuardClauseDelegate GuardClauseDelegates = null;
    }
    public static class StatusGenerator
    {
        public static List<StatusModel> GetStatusModels(IRequestModel requestModel)
        {
            //todo  get from Db

            var list = new List<StatusModel>();

            list.Add(new StatusModel()
            {
                Id = 1,
                MethodModel = new MethodModel { CurrentStatus = "شروع" },
                IfClause = new List<TriggerModel>() { new TriggerModel { Trigger = "ذخیره", Status = "درجریان" } },
            });

            list.Add(new StatusModel()
            {
                Id = 2,
                MethodModel = new MethodModel { CurrentStatus = "درجریان", },
                IfClause = new List<TriggerModel>()
                {
                    new TriggerModel { Trigger = "قبول", Status = "انتشار یافته"},
                    new TriggerModel { Trigger = "رد", Status = "رد شده",GuardClauseDelegates=()=>{return true; } },
                },
                PermitReentryIfModels = new List<PermitReentryIfModel> { new PermitReentryIfModel { Trigger = "ذخیره", } }
            });

            list.Add(new StatusModel()
            {
                Id = 3,
                MethodModel = new MethodModel
                {
                    CurrentStatus = "انتشار یافته",
                },
                IfClause = new List<TriggerModel>() { new TriggerModel { Trigger = "نیاز به ویرایش", Status = "درجریان", } },

            });

            list.Add(new StatusModel()
            {
                Id = 4,
                MethodModel = new MethodModel
                {
                    CurrentStatus = "رد شده",
                },
                IfClause = new List<TriggerModel>() { new TriggerModel { Trigger = "نیاز به ویرایش", Status = "درجریان" } },

            });

            return list;
        }
        public static List<string> GetTriggers()
        {
            return new List<string>(){
                "ذخیره",
                "نیاز به ویرایش",
                "قبول",
                "رد",
            };
        }
    }
}
