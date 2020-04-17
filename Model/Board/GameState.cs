﻿using ProjectB.Model.Figures;
using ProjectB.Model.Help;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectB.Model.Board
{
    public class GameState
    {


        public static int HEIGHT = 11;
        public static int WIDTH = 11;

        private readonly Arena A = new Arena();



        private byte move = 0; //0 zaznacz| 1 porusz sie | 2 atak
        private bool turn = true; //czyja kolej
        private bool attackType; //true - primary, false - extra
        private bool attackChosen = false; //czy został wybrany atak

        private List<Cord> lastFields = new List<Cord>();
        private List<Cord> possibleAttackFields = new List<Cord>();
        private List<Cord> markedAttackFields = new List<Cord>();
        private List<MagSkill> magSkills = new List<MagSkill>();

        private Cord cordToMove;
        private Cord cordToAttack;




        public delegate void ShowPawnInfo(string imgPath, string floorPath, string baseInfo, string precInfo);
        public event ShowPawnInfo ShowPawnEvent;

        public delegate void OnAttackStart(bool primaryAttack, bool extraAttack);
        public event OnAttackStart StartAttack;

        public delegate void FieldToAttackSelected();
        public event FieldToAttackSelected SelectedFieldToAttack;

        public delegate void EndRoundD();
        public event EndRoundD EndRoundEvent;


        public List<Cord> HandleInput(Cord C) //metoda zwraca kordy wszytkich pol na których sie cos zmieniło żeby okno moglo je zaktualizować
        {
            if (PAt(C) != null) //pole z pionkeim
            {
                ShowPawnEvent(A.PAt(C).ImgPath, A[C].FloorPath(), PAt(C).BaseInfo, PAt(C).PrecInfo);
            }
            else //sama podloga
            {
                ShowPawnEvent(null, A[C].FloorPath(), A[C].FloorBaseInfo, A[C].FloorPrecInfo);
            }

            Console.WriteLine("HandleInput dla pola; " + C + ". Move = " + move);

            if (move == 0)//gracz wybiera pionka którym chce sie ruszyć
            {
                return ShowPossibleMove(C);
            }
            else if (move == 1) // gracz wybiera miejsce w które chce się ruszyć
            {
                return MovePawnToField(C);
            }
            else if (move == 2) //gracz wybiera pole które chce zaatakować
            {
                return AttackField(C);
            }
            return null;
        }

        public List<Cord> AttackField(Cord C) //wybor pionka do zaatakowania
        {

            if (A[C].FloorStatus == FloorStatus.Attack)
            {
                cordToAttack = C;
                SelectedFieldToAttack?.Invoke();

                foreach (Cord cor in markedAttackFields)
                {
                    A[cor].FloorStatus = FloorStatus.Normal;
                }

                A[C].FloorStatus = FloorStatus.Attack;
                move = 3;
                return markedAttackFields;
            }
            return null;
        }

        public List<Cord> ExecuteAttack(int bonus1, int bonus2)
        {
            string x = attackType ? "Podstawowym" : "Extra";
            Console.WriteLine($"Pionek na polu {cordToMove} z bonusem {bonus1} atakuje atakiem {x} pionka na polu {cordToAttack} z bonusem {bonus2}");


            if (attackType)
            {
                return (PAt(cordToMove).NormalAttack(A, cordToAttack)).Concat(EndRound()).ToList();
            }
            else
            {
                return (PAt(cordToMove).SkillAttack(this, cordToAttack)).Concat(EndRound()).ToList();
            }
        }

        public List<Cord> MarkFieldsToAttack(bool attackType)
        {
            if (!attackChosen)
            {
                this.attackType = attackType;
                markedAttackFields = PAt(cordToMove).MarkFieldsToAttack(possibleAttackFields, A, attackType);
                attackChosen = true;
                return possibleAttackFields;
            }
            return null;
        }


        public List<Cord> ShowPossiblePrimaryAttack()
        {

            if (!attackChosen)
            {
                return possibleAttackFields = PAt(cordToMove).ShowPossibleAttack(cordToMove, A, true);
            }
            else
            {
                return null;
            }
        }

        public List<Cord> ShowPossibleExtraAttack()
        {

            if (!attackChosen)
            {
                return possibleAttackFields = PAt(cordToMove).ShowPossibleAttack(cordToMove, A, false);
            }
            else
            {
                return null;
            }
        }


        public List<Cord> HideAttackFields()
        {
            List<Cord> cordsToUpdate = new List<Cord>();

            if (!attackChosen)
            {
                foreach (Cord C in possibleAttackFields)
                {
                    cordsToUpdate.Add(C);
                    A[C].FloorStatus = FloorStatus.Normal;
                }
            }
            return cordsToUpdate;
        }


        private List<Cord> MovePawnToField(Cord C)
        {

            List<Cord> cordsToUpdate = new List<Cord>();

            if (A[C].FloorStatus == FloorStatus.Move) // move field to cord
            {
                if (!cordToMove.Equals(C)) //ruch na inne pole niz obecne
                {
                    A[C].PawnOnField = PAt(cordToMove);

                    A[cordToMove].PawnOnField = null;
                    cordToMove = C;
                    move = 2;

                    StartAttack?.Invoke(PAt(C).IsSomeoneToAttack(C, A, true), PAt(C).IsSomeoneToAttack(C, A, false));

                }
                else // nacisniecie na pole na ktorym byl pionek, anulowanie ruchu
                {
                    move = 0;
                }
                foreach (Cord cord in lastFields)
                {
                    A[cord].FloorStatus = FloorStatus.Normal;
                    cordsToUpdate.Add(cord);
                }
                ShowPawnEvent(PAt(C).ImgPath, A[C].FloorPath(), PAt(C).BaseInfo, PAt(C).PrecInfo);
            }
            return cordsToUpdate;
        }

        private List<Cord> ShowPossibleMove(Cord C)
        {

            if (PAt(C) != null && PAt(C).Owner == turn) //click on own pawn
            {
                move = 1;
                cordToMove = C;
                return lastFields = PAt(C).ShowPossibleMove(C, A);
            }
            else //click on empty field or enemy pawn
            {
                lastFields.Clear();
                return lastFields;
            }
        }

        public List<Cord> EndRound()
        {
            Console.WriteLine($"End round, move = {move}");
            turn ^= true;
            attackChosen = false;

            EndRoundEvent?.Invoke();
            if (move == 0)
            {
                return ExecuteMagSkills();
            }
            else if (move == 1)
            {
                move = 0;
                foreach (Cord C in lastFields)
                {
                    A[C].FloorStatus = FloorStatus.Normal;
                }
                return ExecuteMagSkills().Concat(lastFields).ToList();
            }
            else if (move == 2 || move == 3)
            {
                move = 0;
                foreach (Cord C in markedAttackFields)
                {
                    A[C].FloorStatus = FloorStatus.Normal;
                }
                return ExecuteMagSkills().Concat(markedAttackFields).ToList();
            }

            throw new NotImplementedException();
        }


        public Pawn PAt(Cord cord) => A.PAt(cord);
        public Field At(Cord cord) => A[cord];




        public void AddMagSkillAttack(Cord attackPlace, bool attackOwner, byte roundsToExec, int dmg)
        {
            magSkills.Add(new MagSkill(attackPlace, attackOwner, dmg, roundsToExec));
        }


        private List<Cord> ExecuteMagSkills()
        {
            Console.WriteLine($"Executing mag skill. Skills count {magSkills.Count}");
            List<Cord> cordsToUpdate = new List<Cord>();

            for (int i = 0; i < magSkills.Count;)
            {
                Console.WriteLine(magSkills[i]);
                magSkills[i].RoundsToExec--;
                if (magSkills[i].RoundsToExec == 1) //executing
                {
                    cordsToUpdate = cordsToUpdate.Concat(magSkills[i].Execute(A)).ToList();
                    i++;
                }
                else if (magSkills[i].RoundsToExec == 0) //clearing
                {
                    cordsToUpdate = cordsToUpdate.Concat(magSkills[i].Clear(A)).ToList();
                    magSkills.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }


            return cordsToUpdate;
        }


    }
}



