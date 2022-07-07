using Flawless.Battle.Skill;
using System.Text;

namespace Flawless.Util
{
    public static class LogExtension
    {
        public static string GetSkillDescription(string skillName)
        {
            var name = LocalizationManager.Instance.GetSkillName(skillName);
            var desc = LocalizationManager.Instance.GetSkillDescription(skillName);
            return $"{name} : {desc}";
        }

        public static string PoseToString(this PoseType type)
        {
            switch (type)
            {
                case PoseType.High:
                    return "높은 자세";
                case PoseType.Low:
                    return "낮은 자세";
                case PoseType.Special:
                    return "찌르기 자세";
            }

            return null;
        }

        public static string SkillLogToString(this SkillLog actionLog)
        {
            var sb = new StringBuilder();
            sb.Append($"[Turn {actionLog.TurnCount + 1}] {actionLog.CasterName} : ");

            if (actionLog.Skill == null)
            {
                sb.AppendLine("아무 행동도 하지 않았다.");
                return sb.ToString();
            }

            var skillName = LocalizationManager.Instance.GetSkillName(actionLog.Skill.GetType().Name);
            sb.AppendLine($"{skillName} 스킬을 사용하였다. (스피드 : {actionLog.Speed})");
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
            if (actionLog.BlockedByCooldown)
            {
                sb.AppendLine("쿨타임 중이라 발동할 수 없었다.");
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
