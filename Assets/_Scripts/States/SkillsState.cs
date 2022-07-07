using System;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using Libplanet.Store;

namespace Flawless.States
{
    /// <summary>
    /// A <see cref="DataModel"/> representing character's set of skills owned and set.
    /// </summary>
    public class SkillsState : DataModel
    {
        public ImmutableList<string> OwnedSkills { get; private set; }
        public ImmutableList<string> EquippedSkills { get; private set; }

        public SkillsState()
            : base()
        {
            OwnedSkills = ImmutableList<string>.Empty;
            EquippedSkills = ImmutableList<string>.Empty;
        }

        private SkillsState(
            ImmutableList<string> ownedSkills,
            ImmutableList<string> equippedSkills)
            : base()
        {
            OwnedSkills = ownedSkills;
            EquippedSkills = equippedSkills;
        }

        /// <summary>
        /// Decodes a stored <see cref="SkillsState"/>.
        /// </summary>
        /// <param name="encoded">A <see cref="SkillsState"/> encoded as
        /// a <see cref="Bencodex.Types.Dictionary"/>.</param>
        public SkillsState(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }

        [Pure]
        public SkillsState SetOwnedSkills(ImmutableList<string> skills)
        {
            foreach (string ownedSkill in OwnedSkills)
            {
                if (!skills.Contains(ownedSkill))
                {
                    throw new ArgumentException($"Cannot lose skill {ownedSkill} already owned by character.");
                }
            }

            return new SkillsState(
                ownedSkills: skills,
                equippedSkills: EquippedSkills);
        }

        [Pure]
        public SkillsState SetEquippedSkills(ImmutableList<string> skills)
        {
            foreach (string skill in skills)
            {
                if (!OwnedSkills.Contains(skill))
                {
                    throw new ArgumentException($"Cannot equip skill {skill} not owned by character.");
                }
            }

            return new SkillsState(
                ownedSkills: OwnedSkills,
                equippedSkills: skills);
        }
    }
}
