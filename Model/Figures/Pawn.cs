﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectB.Model.Figures
{
    public abstract class Pawn //To jest klasa bazowa czyli pionek
    {


        #region properties
        public const string imagePath = "/ProjectB;component/Res/Images/{0}";
        public abstract int BaseHealth();
        public abstract int BaseAttack();
        public abstract int BaseDef();
        public abstract int BaseCondition();
        public abstract int BaseManna();
        public abstract string ImagePath();

        protected int hp;
        public int HP
        {
            get
            {
                return hp;
            }
            protected set
            {
                hp = value;
            }
        }

        private int manna;

        public int Manna
        {
            get
            {
                return manna;
            }
            set
            {
                manna = value;
            }
        }

        public bool Owner
        {
            get;
            set;
        }

        public string ImgPath { get { return string.Format(imagePath, ImagePath()); } }


        #endregion


        public Pawn(bool owner)
        {
            Owner = owner;
            HP = BaseHealth();
            Manna = BaseManna();
        }

        #region methods


        public virtual void NormalAttack(Pawn pawnToAttack, double attackBonus)
        {
            //TODO jak bedzie znana logika gry
        }

        public virtual void SkillAttack(Pawn pawnToAttack, double attackBonus)
        {
            //TODO jak bedzie znana logika gry
        }


        public virtual void Def(int dmg)
        {
            //TODO jak bedzie znana logika gry
        }

        public virtual void Move()
        {
            //TODO jak bedzie znana logika gry
        }

        //testowa metoda 
        public virtual void TestHPDown()
        {
            HP--;
            //TODO jak bedzie znana logika gry
        }



        #endregion



    }
}


