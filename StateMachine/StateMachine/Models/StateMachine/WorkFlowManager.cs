using Stateless.Graph;

using System.Diagnostics;

namespace StateMachine.Models.StateMachine
{
    public class WorkFlowManager
    {
        private StateMachineModel _stateMachine;
        public WorkFlowManager(RequestModel requestModel)
        {
            configureWorkflow(requestModel);
        }


        private void configureWorkflow(RequestModel requestModel)
        {
            var model = StatusGenerator.GetStatusModels();

            var BeginStatus = model.First(x => x.MethodModel.CurrentStatus == "شروع");
            BeginStatus.MethodModel.OnEntry = (model) =>
            {
                Debug.WriteLine($"شروع OnEntry {nameof(model)} {requestModel.RequestText}");
            };

            BeginStatus.MethodModel.OnExit = (model) =>
            {
                Debug.WriteLine($"شروع OnExit {nameof(model)} {requestModel.RequestText}");
            };

            BeginStatus.IfClause.First(x => x.Trigger == "ذخیره").GuardClauseDelegates = () =>
            {
                return UserCanPost;
            };

            var InProgressStatus = model.First(x => x.MethodModel.CurrentStatus == "درجریان");

            InProgressStatus.MethodModel.OnEntry = (model) =>
            {
                Debug.WriteLine($"InProgress OnEntry {nameof(model)}");
            };

            InProgressStatus.MethodModel.OnExit = (model) =>
            {
                Console.WriteLine($"درجریان OnExit {nameof(model)}");
            };

            InProgressStatus.IfClause.First(x => x.Trigger == "قبول").GuardClauseDelegates = () =>
            {
                return UserIsAdmin;
            };

            InProgressStatus.IfClause.First(x => x.Trigger == "رد").GuardClauseDelegates = () =>
            {
                return UserIsAdmin;
            };

            InProgressStatus.PermitReentryIfModels.First(x => x.Trigger == "ذخیره").GuardClauseDelegates = () =>
            {
                return UserHasEditRights;
            };

            var PublishedStatus = model.First(x => x.MethodModel.CurrentStatus == "انتشار یافته");

            PublishedStatus.MethodModel.OnEntry = (model) =>
            {
                Debug.WriteLine($"انتشار یافته OnEntry {nameof(model)}");
            };

            PublishedStatus.MethodModel.OnExit = (model) =>
            {
                Debug.WriteLine($"انتشار یافته OnExit {nameof(model)}");
            };

            PublishedStatus.IfClause.First(x => x.Trigger == "نیاز به ویرایش").GuardClauseDelegates = () =>
            {
                return UserHasEditRights;
            };


            var RejectedStatus = model.First(x => x.MethodModel.CurrentStatus == "رد شده");
            RejectedStatus.MethodModel.OnEntry = (model) =>
            {
                Debug.WriteLine($"رد شده OnEntry {nameof(model)}");
            };
            RejectedStatus.MethodModel.OnExit = (model) =>
            {
                Debug.WriteLine($"رد شده OnExit {nameof(model)}");
            };
            RejectedStatus.IfClause.First(x => x.Trigger == "نیاز به ویرایش").GuardClauseDelegates = () =>
            {
                return UserHasEditRights;
            };

            _stateMachine = new StateMachineModel(new StatusModelWrapper
            {
                Status = requestModel.CurrentStatus,
                StatusModels = model
            });

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


        public string Save(RequestModel model)
        {
            return _stateMachine.TryFireTrigger(StatusGenerator.GetTriggers().First(x => x == "ذخیره"))
                ? $"وضعیت تغییر کرد"
                : "امکان تغییر وضعیت وجود ندارد";
        }
        public string RequireEdit(RequestModel model)
        {
            return _stateMachine.TryFireTrigger(StatusGenerator.GetTriggers().First(x => x == "نیاز به ویرایش"))
                ? "نیاز به ویرایش دارد"
                : "امکان تغییر وضعیت وجود ندارد";
        }

        public string Accept(RequestModel model)
        {
            return _stateMachine.TryFireTrigger(StatusGenerator.GetTriggers().First(x => x == "قبول"))
                ? "پذیرفته شد"
                : "امکان تغییر وضعیت وجود ندارد";
        }
        public string Reject(RequestModel model)
        {
            return _stateMachine.TryFireTrigger(StatusGenerator.GetTriggers().First(x => x == "رد"))
                ? "رد شد"
                : "امکان تغییر وضعیت وجود ندارد";
        }

        public string GetGraphInfo()
        {
            return UmlDotGraph.Format(_stateMachine.GetInfo());
        }

    }

}
