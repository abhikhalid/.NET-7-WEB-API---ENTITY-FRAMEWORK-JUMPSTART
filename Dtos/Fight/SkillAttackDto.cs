using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dtos.Fight
{
    public class SkillAttackDto
    {
        public int AttackerId { get; set; }

        public int OpponentId { get; set; }

        //since, character has multiple skill, we are interested to know which skill attacker would use to attack
        public int SkillId { get; set; }
    }
}