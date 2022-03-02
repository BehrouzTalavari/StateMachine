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
        public virtual string ChangeStatus(string trigger, T workFlowModel)
        {
            if (TryFireTrigger(trigger))
            {
                workFlowModel.CurrentStatus = CurrentStatus;
                return CurrentStatus;

            }
            return string.Empty;
        }

        public void AddOrganizationStructure()
        {
            var getOrg = OrganizattionRepository.GetOrganizationStracture().OrderByDescending(x => x.level);

            var list = new List<StatusModel>();

            int id = 1;
            list.Add(new StatusModel()
            {
                Id = id++,
                MethodModel = new MethodModel { CurrentStatus = "شروع" },
                IfClause = new List<TriggerModel>() { new TriggerModel { Trigger = $"ذخیره-{getOrg.First().Role}", Status = $"در انتظار تائید-{getOrg.First().Role}" } },
            });

            foreach (var item in getOrg)
            {
                list.Add(new StatusModel
                {
                    Id = id++,
                    MethodModel = new MethodModel { CurrentStatus = $"در انتظار تائید-{item.Role}" },
                    IfClause = new List<TriggerModel>() { new TriggerModel { Trigger = $"{item.Role}-تائید توسط", Status = $"در انتظار تائید-{item.ParrentRole}" } },
                });
            }

            list.Add(new StatusModel()
            {
                Id = id,
                MethodModel = new MethodModel { CurrentStatus = list.Last().MethodModel.CurrentStatus },
                IfClause = new List<TriggerModel>() { new TriggerModel { Trigger = "خاتمه", Status = "خاتمه یافته" } },
            });

            ConfigureStateMachine(new StatusModelWrapper
            {
                Status = "شروع",
                StatusModels = list
            });

        }

    }
    public static class OrganizattionRepository
    {
        public static List<OrganizationStracture> GetOrganizationStracture()
        {
            List<OrganizationStracture> organizationStractures = new List<OrganizationStracture>();
            organizationStractures.Add(new OrganizationStracture
            {
                level = 1,
                ParrentRole = String.Empty,
                Role = "مدیر عامل"
            });
            organizationStractures.Add(new OrganizationStracture
            {
                level = 2,
                ParrentRole = "مدیر عامل",
                Role = "معاون فناوری"
            });

            organizationStractures.Add(new OrganizationStracture
            {
                level = 3,
                ParrentRole = "معاون فناوری",
                Role = "مدیر فناوری اطلاعات"
            });

            organizationStractures.Add(new OrganizationStracture
            {
                level = 4,
                ParrentRole = "مدیر فناوری اطلاعات",
                Role = "رئیس سخت افزار"
            });

            organizationStractures.Add(new OrganizationStracture
            {
                level = 5,
                ParrentRole = "رئیس سخت افزار",
                Role = "کارشناس سخت افزار"
            });

            //organizationStractures.Add(new OrganizationStracture
            //{
            //    level = 4,
            //    ParrentRole = "مدیر فناوری اطلاعات",
            //    Role = "رئیس سیستم های وب"
            //});

            //organizationStractures.Add(new OrganizationStracture
            //{
            //    level = 5,
            //    ParrentRole = "رئیس سیستم های وب",
            //    Role = "کارشناس نرم افزار"
            //});

            //organizationStractures.Add(new OrganizationStracture
            //{
            //    level = 4,
            //    ParrentRole = "مدیر فناوری اطلاعات",
            //    Role = "رئیس سیستم های MIS"
            //});

            //organizationStractures.Add(new OrganizationStracture
            //{
            //    level = 5,
            //    ParrentRole = "رئیس سیستم های MIS",
            //    Role = "کارشناس نرم افزار MIS"
            //});
            return organizationStractures;
        }
    }
    public class OrganizationStracture
    {
        public int level { get; set; }
        public string Role { get; set; }
        public string ParrentRole { get; set; }
    }
}
