using System.Collections.Generic;
using Libplanet.Action;
using Libplanet.Blocks;
using Libplanet.Blockchain.Renderers;
using Libplanet.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Serilog;
using Serilog.Sinks;
using Flawless.Actions;
using Flawless.States;
using Flawless.UI;

namespace Flawless
{
    public class Game : MonoSingleton<Game>
    {
        [SerializeField]
        private BattleUI battleUI = null;
        private IEnumerable<IRenderer<PolymorphicAction<ActionBase>>> _renderers;
        public Agent Agent { get; private set; }

        // Unity MonoBehaviour Awake().
        protected override void Awake()
        {
            base.Awake();
            // General application settings.
            Screen.SetResolution(800, 600, FullScreenMode.Windowed);
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            // Renderers are called when certain conditions are met.
            // There are different types of renderers called under different conditions.
            // Some are called when a new block is added, some are called when an action is executed.
            _renderers = new List<IRenderer<PolymorphicAction<ActionBase>>>()
            {
                new AnonymousActionRenderer<PolymorphicAction<ActionBase>>()
                {
                    ActionRenderer = (action, context, nextStates) =>
                    {
                        if (context.Signer != Agent.Address)
                        {
                            return;
                        }

                        switch (((PolymorphicAction<ActionBase>)action).InnerAction)
                        {
                            case CreateAccountAction ca:
                                UnityEngine.Debug.Log($"CA: {nextStates}");
                                break;
                            case BattleAction ba:
                                Agent.RunOnMainThread(
                                    () => RenderBattleAction(ba, context, nextStates)
                                );
                                break;
                        }
                    }
                }
            };

            // Initialize a Libplanet Unity Agent.
            Agent = Agent.AddComponentTo(gameObject, _renderers);
        }

        // Unity MonoBehaviour Start().
        public void Start()
        {
            if (Agent.GetState(EnvironmentState.EnvironmentAddress) is null)
            {
                Agent.MakeTransaction(
                    new PolymorphicAction<ActionBase>[]
                    {
                        new InitalizeStatesAction(
                            weaponSheetCsv: Resources.Load<TextAsset>("TableSheets/WeaponSheet").text,
                            skillPresetSheetCsv: Resources.Load<TextAsset>("TableSheets/SkillPresetSheet").text
                        ),
                    }
                );
            }

            if (Agent.GetState(Agent.Address) is null)
            {
                Agent.MakeTransaction(
                    new PolymorphicAction<ActionBase>[]
                    {
                        new CreateAccountAction(
                            account: Agent.Address,
                            name: Agent.Address.ToString()
                        ),
                    }
                );
            }
        }

        private void RenderBattleAction(
            BattleAction action,
            IActionContext context,
            IAccountStateDelta nextStates
        )
        {
            battleUI.WriteLogs(action.BattleLogs);
        }
    }
}