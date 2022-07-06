
namespace Flawless.Battle
{
    public class CharacterStat
    {
        public int STR { get; set; }
        public int DEX { get; set; }
        public int INT { get; set; }
        public Weapon Weapon { get; set; }

        public int HP { get; set; }
        public int ATK { get; set; }
        public int DEF { get; set; }
        public int SPD { get; set; }

        public int BaseHP => STR * 10 + (Weapon?.HP ?? 0);
        public int BaseATK => STR * 1 + (Weapon?.ATK ?? 0);
        public int BaseDEF => DEX / 2 + (Weapon?.DEF ?? 0);
        public int BaseSPD => DEX + (Weapon?.SPD ?? 0);
        public int LifeStealPercentage => Weapon?.LifeStealPercentage ?? 0;

        public CharacterStat(int str, int dex, int @int)
        {
            STR = str;
            DEX = dex;
            INT = @int;

            HP = BaseHP;
            ATK = BaseATK;
            DEF = BaseDEF;
            SPD = BaseSPD;
        }
    }
}