using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.MyClasses.Helpers
{
    public static class SkillsHelper
    {
        private static readonly SkillType[] skills = {
            SkillType.MagicalDamageBonusPassive1,
            SkillType.MagicalDamageBonusAura1,
            SkillType.MagicalDamageBonusPassive2,
            SkillType.MagicalDamageBonusAura2,
            SkillType.FrostBolt,

            SkillType.RangeBonusPassive1,
            SkillType.RangeBonusAura1,
            SkillType.RangeBonusPassive2,
            SkillType.RangeBonusAura2,
            SkillType.AdvancedMagicMissile,

            SkillType.StaffDamageBonusPassive1,
            SkillType.StaffDamageBonusAura1,
            SkillType.StaffDamageBonusPassive2,
            SkillType.StaffDamageBonusAura2,
            SkillType.Fireball,

            SkillType.MovementBonusFactorPassive1,
            SkillType.MovementBonusFactorAura1,
            SkillType.MovementBonusFactorPassive2,
            SkillType.MovementBonusFactorAura2,
            SkillType.Haste,

            SkillType.MagicalDamageAbsorptionPassive1,
            SkillType.MagicalDamageAbsorptionAura1,
            SkillType.MagicalDamageAbsorptionPassive2,
            SkillType.MagicalDamageAbsorptionAura2,
            SkillType.Shield,
        };

        public static SkillType? GetSkillToLearn()
        {
            var level = Tick.Self.Level;
            if (level == 0)
            {
                return null;
            }

            if (level > skills.Length)
            {
                return null;
            }
            return skills[level - 1];
        }
    }
}
