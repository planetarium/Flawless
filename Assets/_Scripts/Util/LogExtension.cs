using Flawless.Battle.Skill;
using System.Text;

namespace Flawless.Util
{
    public static class LogExtension
    {
        public static string SkillLogToString(this SkillLog actionLog)
        {
            var sb = new StringBuilder();
            sb.Append($"[Turn {actionLog.TurnCount + 1}] {actionLog.Caster} : ");

            if (actionLog.Skill == null)
            {
                sb.AppendLine("스킬 사용이 차단되었다.");
                return sb.ToString();
            }

            sb.AppendLine($"{actionLog.Skill.GetType().Name} 스킬을 사용하였다.");
            if (actionLog.Blocked)
            {
                sb.AppendLine("스킬 사용이 차단되었다.");
                return sb.ToString();
            }
            if (actionLog.BlockedByCounter)
            {
                sb.AppendLine("스킬 사용이 카운터로 인해 차단되었다.");
                return sb.ToString();
            }
            if (actionLog.BlockedByPose)
            {
                sb.AppendLine("스킬 발동을 위한 자세가 맞지 않아 발동할 수 없었다.");
                return sb.ToString();
            }
            if (actionLog.DamageBlocked)
            {
                sb.AppendLine("카운터로 인해 데미지를 입히지 못했다.");
            }
            if (actionLog.DealtDamage > 0)
            {
                sb.AppendLine($"{actionLog.DealtDamage}의 데미지를 입혔다. [데미지 배율 : {actionLog.DamageMultiplier}배]");
            }
            if (actionLog.LifestealAmount > 0)
            {
                sb.AppendLine($"{actionLog.LifestealAmount} HP를 흡수했다.");
            }
            if (actionLog.HealAmount > 0)
            {
                sb.AppendLine($"{actionLog.HealAmount} HP를 회복했다.");
            }
            if (actionLog.CounteredDamage > 0)
            {
                sb.AppendLine($"반격당해 {actionLog.HealAmount}의 데미지를 입었다.");
            }

            return sb.ToString();
        }
    }
}
