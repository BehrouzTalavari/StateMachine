using Stateless;

namespace StateMachine.StateMachineSamples
{
    public class LightSatetMachine
    {
        private readonly string on = "On";
        private string off = "Off";
        readonly char triggerKey = 'd';
        StateMachine<string, char> onOffSwitch;

        public LightSatetMachine()
        {
            onOffSwitch = new StateMachine<string, char>(initialState: off);
            onOffSwitch.Configure(state: off).Permit(trigger: triggerKey, destinationState: on);
            onOffSwitch.Configure(state: on).Permit(trigger: triggerKey, destinationState: off);
        }
        public string Run(char pressed)
        {    
            onOffSwitch.Fire(trigger: pressed);
            return $"Switch is in state: {onOffSwitch.State}";             
            
        }
    }

}
