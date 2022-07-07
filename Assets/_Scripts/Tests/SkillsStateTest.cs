using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NUnit.Framework;
using Flawless.States;

public class SkillsStateTest
{
    [Test]
    public void SetSkills()
    {
        SkillsState skillsState = new SkillsState();
        List<string> ownedSkills = new List<string>() { "a", "b", "c" };
        List<string> equippedSkills = new List<string>() { "b", "c" };
        List<string> invalidNewOwnedSkills = new List<string>() { "a", "c", "d", "e", "f" };
        List<string> validNewOwnedSkills = new List<string>() { "a", "b", "c", "g" };
        List<string> invalidNewEquippedSkills = new List<string>() { "a", "d" };
        List<string> validNewEqippedSkills = new List<string>() { "a" };

        skillsState = skillsState.SetOwnedSkills(ownedSkills.ToImmutableList());
        skillsState = skillsState.SetEquippedSkills(equippedSkills.ToImmutableList());

        Assert.Throws<ArgumentException>(() => skillsState.SetOwnedSkills(invalidNewOwnedSkills.ToImmutableList()));
        Assert.Throws<ArgumentException>(() => skillsState.SetEquippedSkills(invalidNewEquippedSkills.ToImmutableList()));

        skillsState = skillsState.SetOwnedSkills(validNewOwnedSkills.ToImmutableList());
        skillsState = skillsState.SetEquippedSkills(validNewEqippedSkills.ToImmutableList());
    }
}
