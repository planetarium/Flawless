
using Flawless.Data;

namespace Flawless.Battle.Skill
{
    public class SkillFactory
    {
        public static SkillFactory Instance
        {
            get => _instance ??= new SkillFactory();
        }

        private static SkillFactory _instance = null;

        private SkillFactory()
        {
        }

        public SkillBase CreateSkill(SkillSheet.Row row)
        {
            switch (row.Id)
            {
                case "DownwardSlash":
                    return new DownwardSlash(
                        row.Speed,
                        row.Cooldown,
                        row.ATKCoefficient,
                        row.DEXCoefficient,
                        row.INTCoefficient,
                        row.FinishPose,
                        row.AvailablePoses);
                case "UpwardSlash":
                    return new UpwardSlash(
                        row.Speed,
                        row.Cooldown,
                        row.ATKCoefficient,
                        row.DEXCoefficient,
                        row.INTCoefficient,
                        row.FinishPose,
                        row.AvailablePoses);
                case "DownwardThrust":
                    return new DownwardThrust(
                        row.Speed,
                        row.Cooldown,
                        row.ATKCoefficient,
                        row.DEXCoefficient,
                        row.INTCoefficient,
                        row.FinishPose,
                        row.AvailablePoses);
                case "UpwardThrust":
                    return new UpwardThrust(
                        row.Speed,
                        row.Cooldown,
                        row.ATKCoefficient,
                        row.DEXCoefficient,
                        row.INTCoefficient,
                        row.FinishPose,
                        row.AvailablePoses);
                case "Heal":
                    return new Heal(
                        row.Speed,
                        row.Cooldown,
                        row.ATKCoefficient,
                        row.DEXCoefficient,
                        row.INTCoefficient,
                        row.FinishPose,
                        row.AvailablePoses);
                case "SideStep":
                    return new SideStep(
                        row.Speed,
                        row.Cooldown,
                        row.ATKCoefficient,
                        row.DEXCoefficient,
                        row.INTCoefficient,
                        row.FinishPose,
                        row.AvailablePoses);
                default:
                    return null;
            }
        }
    }
}
