﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectB.Model.Figures
{
    class Mag : Pawn
    {
        #region properties

        public const int BASE_HP = 20;
        public const int ATTACK = 2;
        public const int DEF = 10;
        public const int CONDITION = 1;
        public const int BASE_MANNA = 7;
        public const string IMAGE_PATH_TRUE = "witch64.png";
        public const string IMAGE_PATH_FALSE = "witch64.png";
        public override int BaseAttack()
        {
            return ATTACK;
        }

        public override int BaseCondition()
        {
            return CONDITION;
        }

        public override int BaseDef()
        {
            return DEF;
        }

        public override int BaseHealth()
        {
            return BASE_HP;
        }

        public override int BaseManna()
        {
            return BASE_MANNA;
        }

        public override string ImagePath()
        {
            return ImagePath();
        }

        #endregion


        #region methods  

        public override void Def(int dmg)
        {
            base.Def(dmg);
            //TODO logika obrony tank, jesli będzie miała cos więcej niż bazowa implementacja w klasie Pawn. Jak nie to można skasować. To samo tyczy się wszytkich metod override
        }


        public override void Move()
        {
            base.Move();
        }

        public override void NormalAttack(Pawn pawnToAttack, double attackBonus)
        {
            base.NormalAttack(pawnToAttack, attackBonus);
        }

        public override void SkillAttack(Pawn pawnToAttack, double attackBonus)
        {
            base.SkillAttack(pawnToAttack, attackBonus);
        }

        public Mag(bool owner):base(owner)
        {
                        
        }

        #endregion


    }
}
