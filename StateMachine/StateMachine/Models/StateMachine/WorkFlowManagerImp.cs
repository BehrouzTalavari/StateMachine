
using System.Diagnostics;

namespace StateMachine.Models.StateMachine
{

    public class WorkFlowManagerImp : WorkFlowManager<RequestModel>
    {
        public WorkFlowManagerImp(RequestModel requestModel) : base(requestModel)
        {
        }


        public override void ConfigureWorkFlow(RequestModel workFlowModel)
        {
            RequestViewModel vm1 = (RequestViewModel)workFlowModel.Params[0];
            RequestViewModelXXXXX vm2 = (RequestViewModelXXXXX)workFlowModel.Params[1];

            AddOnEntryStatus("شروع", (state) =>
            {
                Debug.WriteLine($"شروع OnEntry {nameof(state)} {vm1.RequestText}");
            })
            .AddOnExitStatus("شروع", (state, requestModel) =>
            {
                Debug.WriteLine($"شروع OnExit {nameof(state)} {vm1.RequestText}");
            })
            .AddIfClause("شروع", "ذخیره", () =>
            {
                return UserCanPost || vm1.Id == 1;
            });

            base.AddOnEntryStatus("درجریان", (state) =>
            {
                Debug.WriteLine($"شروع OnEntry {nameof(state)} {vm1.RequestText}");
            })
                .AddOnExitStatus("درجریان", (state, requestModel) => { Debug.WriteLine($"شروع OnExit {nameof(state)} {vm1.RequestText}"); })
                .AddIfClause("درجریان", "قبول", () => { return UserHasEditRights || UserHasEditRights; })
                .AddIfClause("درجریان", "رد", () => { return UserIsAdmin || vm1.Id == 1; })
                .AddReentryIf("درجریان", "ذخیره", () => { return UserCanPost || vm1.Id == 1; });


            base.AddOnEntryStatus("انتشار یافته", (state) => { Debug.WriteLine($"شروع OnEntry {nameof(state)} {vm2.FirstName}"); })
                .AddOnExitStatus("انتشار یافته", (state, requestModel) => { Debug.WriteLine($"شروع OnExit {nameof(state)} {vm2.LastName}"); })
                .AddIfClause("انتشار یافته", "نیاز به ویرایش", () => { return UserHasEditRights || UserHasEditRights; });


            base.AddOnEntryStatus("رد شده", (state) => { Debug.WriteLine($"شروع OnEntry {nameof(state)} {vm1.RequestText}"); })
                .AddOnExitStatus("رد شده", (state, requestModel) => { Debug.WriteLine($"شروع OnExit {nameof(state)} {vm1.RequestText}"); })
                .AddIfClause("رد شده", "نیاز به ویرایش", () => { return UserHasEditRights || UserHasEditRights; });

            base.AddTrasitionMethod((state) =>
            {
                Debug.WriteLine($"شروع OnExit {nameof(state)} {((RequestModel)workFlowModel).CurrentStatus} {DateTime.Now}");
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

        public override string ChangeStatus(string trigger, RequestModel workFlowModel)
        {

            if (base.ChangeStatus(trigger, workFlowModel) == string.Empty)
            {
                return "عملیات ناموفق";
            }
            else
            {
                return $"وضعیت توسط عملیات {trigger}  به وضعیت {workFlowModel.CurrentStatus} تغییر پیدا کرد";
            }
        }
    }


    public class WorkFlowManagerImpForOrganization : WorkFlowManager<RequestModel>
    {
        public WorkFlowManagerImpForOrganization(RequestModel requestModel) : base(requestModel)
        {
            AddOrganizationStructure();
        }

        public override void ConfigureWorkFlow(RequestModel workFlowModel)
        { 
        }
    }

}
