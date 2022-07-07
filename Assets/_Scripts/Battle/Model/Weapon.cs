
using System;

namespace Flawless.Battle
{
    [Serializable]
    public class Weapon
    {
        public int ID { get; private set; }
        public int Grade { get; private set; }
        public int HP { get; private set; }
        public int ATK { get; private set; }
        public int DEF { get; private set; }
        public int SPD { get; private set; }
        public int LifestealPercentage { get; private set; }

        public Weapon(int id, int grade, int hp, int atk, int def, int spd, int lifestealPercentage)
        {
            ID = id;
            Grade = grade;
            HP = hp;
            ATK = atk;
            DEF = def;
            SPD = spd;
            LifestealPercentage = lifestealPercentage;
        }
    }
}
