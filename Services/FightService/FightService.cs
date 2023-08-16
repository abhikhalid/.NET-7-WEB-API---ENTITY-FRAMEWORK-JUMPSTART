using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Dtos.Fight;

namespace Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;

        public FightService(DataContext context)
        {
            _context = context;
        }


        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();

            try
            {
                var attacker = await _context.Characters
                            .Include(c => c.Weapon)
                            .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                var opponent = await _context.Characters
                               .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (attacker is null || opponent is null || attacker.Weapon is null)
                {
                    throw new Exception("Something fishy is going on here...");
                }

                int damage = DoWeaponAttack(attacker, opponent);

                if (opponent.HitPoints <= 0)
                {
                    response.Message = opponent.Name + "has been defeated";
                }

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex){
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private static int DoWeaponAttack(Character attacker, Character opponent)
        {
            if(attacker.Weapon is null){
                throw new Exception("Attacker has no weapon!");
            }

            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defeats);

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();

            try
            {
                var attacker = await _context.Characters
                            .Include(c => c.Skills)
                            .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                var opponent = await _context.Characters
                               .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (attacker is null || opponent is null || attacker.Skills is null)
                {
                    throw new Exception("Something fishy is going on here...");
                }

                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);

                if (skill is null)
                {
                    response.Success = false;
                    response.Message = attacker.Name + " does not know that skill!";
                    return response;
                }

                int damage = DoSkillAttack(attacker, opponent, skill);

                if (opponent.HitPoints <= 0)
                {
                    response.Message = opponent.Name + "has been defeated";
                }

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex){
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private static int DoSkillAttack(Character attacker, Character opponent, Skill skill)
        {
            int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defeats);

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            var response = new ServiceResponse<FightResultDto>{
                Data = new FightResultDto()
            };

            try{
                //we've got all our fighters
                var characters = await _context.Characters
                                        .Include(c => c.Weapon)
                                        .Include(c => c.Skills)
                                        .Where(c => request.CharacterIds.Contains(c.Id))
                                        .ToListAsync();

                bool defeated = false;

                // while loop stops when the first character is defeated
                while(!defeated)
                {
                    foreach(var attacker in characters){
                        var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        var oponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        // I want to see the used weapon or skill of every single attack.
                        string attackUsed = string.Empty;
                        // The next step is to decide if the attacker uses his weapon or one of his skills.
                        bool useWeapon = new Random().Next(2) == 0;

                        if(useWeapon && attacker.Weapon is not null){
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker,oponent);

                        }else if(!useWeapon && attacker.Skills is not null){

                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skill.Name;
                            damage = DoSkillAttack(attacker,oponent,skill);
                        }else{
                            response.Data.Log.Add(attacker.Name + " was not able to attack!");
                            continue;
                        }

                        response.Data.Log.Add($"{attacker.Name} attacks {oponent.Name}  using {attackUsed}  with {(damage>=0 ? damage:0)} damage");

                        if(oponent.HitPoints <=0){
                            defeated = true;
                            attacker.Victories++;
                            oponent.Defeats++;
                            response.Data.Log.Add(oponent.Name + " has been defeated!");
                            response.Data.Log.Add(attacker.Name + " wins with "+attacker.HitPoints+"HP Left!");
                            break;
                        }
                    }
                }

                characters.ForEach(c => {
                    c.Fights++;
                    c.HitPoints = 100;
                });

                await _context.SaveChangesAsync();
            }
            catch(Exception ex){
                response.Success = false;
                response.Message = ex.Message;
            }
        }
    }
}