using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SW_Character_creation;
using Races_libs;
using Skills_libs;
using Age_status_libs;
using Attribute_libs;
using Range_libs;

namespace Character_design
{
    public class Character : BaseViewModel_2
    {
        private static Character Character_instance;

        private List<Skill_Class> skills;
        private List<All_skill_template> skills_with_points;
        private List<Force_skill_class> force_skills;
        private List<All_skill_template> force_skills_with_points;
        private List<All_abilities_template> combat_abilities;
        private List<All_abilities_template> combat_abilities_with_points;
        private List<All_abilities_template> force_abilities;
        private List<All_abilities_template> force_abilities_with_points;
        private List<Abilities_sequence_template> combat_sequences_with_points;
        private List<Abilities_sequence_template> force_sequences_with_points;
        private List<All_feature_template>  positive_features_with_points,
                                            negative_features_with_points,
                                            positive_features,
                                            negative_features,
                                            charm_features,
                                            hero_features,
                                            sleep_feature,
                                            alcohol_feature,
                                            sith_feature,
                                            jedi_feature,
                                            exp_feature,
                                            armour_feature;
        

        private List<int> skill_limits;

        private Race_class character_race;
        private Atribute_class strength;
        private Atribute_class agility;
        private Atribute_class stamina;
        private Atribute_class perception;
        private Atribute_class quickness;
        private Atribute_class intelligence;
        private Atribute_class charm;
        private Atribute_class willpower;
        private Range_Class range;
        private Age_status_class age_status;


        private bool Is_forceuser,
                     is_sith,
                     is_jedi,
                     is_neutral;

        private bool saved_state,
                     features_balanced;

        private int experience,
                    experience_left,
                    experience_sold,
                    attributes,
                    attributes_left,
                    attributes_sold,
                    karma,
                    experience_sold_for_force_skills;

        private int age,
                    reaction,
                    armor,
                    watchfulness,
                    hideness,
                    force_resistance,
                    concentration;

        private string sex,
                       name;

        private int limit_all_forms,
                    limit_force_skills,
                    limit_skills,
                    limit_positive_features,
                    limit_negative_features;

        private int limit_all_forms_left,
                    limit_force_skills_left,
                    limit_skills_left,
                    limit_positive_features_left,
                    limit_negative_features_left,
                    positive_features_points_sold,
                    negative_features_points_sold,
                    positive_features_points_left,
                    negative_features_points_left;

        private sbyte scratch_penalty,
                      light_wound_penalty,
                      medium_wound_penalty,
                      tough_wound_penalty,
                      visible_scratch_penalty,
                      visible_light_wound_penalty,
                      visible_medium_wound_penalty,
                      visible_tough_wound_penalty;

        private byte scratch_lvl,
                     light_wound_lvl,
                     medium_wound_lvl,
                     tough_wound_lvl,
                     mortal_wound_lvl;

        private byte melee_attack,
                     ranged_attack;



        public List<Skill_Class> Get_skills()
        {
            return skills;
        }
        public List<Force_skill_class> Get_force_skills()
        {
            return force_skills;
        }
        public void Delete_character()
        {
            if (Character_instance != null)
            {
                Character_instance = null;
            }
        }
        public void Spend_exp_points(int cost)
        {
            Experience_sold = Experience_sold + cost;
            Experience_left = Experience - Experience_sold;
        }
        public void Refund_exp_points(int cost)
        {
            Experience_sold = Experience_sold - cost;
            Experience_left = Experience - Experience_sold;
        }
        public void Increase_atr(Attribute_libs.Atribute_class attribute)
        {
            if (Attributes_left >= attribute.Get_attribute_cost_for_atr())
            {
                Spend_atr_points(attribute.Get_attribute_cost_for_atr());
                attribute.Increase_atr(1);
                attribute.Increase_atr_for_atr();
                OnPropertyChanged("Attributes_left");
            }
            else
            {
                Spend_exp_points(attribute.Get_attribute_cost_for_exp());
                attribute.Increase_atr_for_exp();
                attribute.Increase_atr(1);
                OnPropertyChanged("Experience_left");
            }
        }
        public void Decrease_atr(Attribute_libs.Atribute_class attribute)
        {
            if (attribute.Get_atr_for_exp() > 0)
            {
                Refund_exp_points(attribute.Get_attribute_cost_for_exp());
                attribute.Decrease_atr(1);
                attribute.Decrease_atr_for_exp();
                OnPropertyChanged("Experience_left");
            }
            else
            {
                Refund_atr_points(attribute.Get_attribute_cost_for_atr());
                attribute.Decrease_atr(1);
                attribute.Decrease_atr_for_atr();
                OnPropertyChanged("Attributes_left");
            }
        }
        public void Increase_exp_sold_for_force_skills(Force_skill_class skill)
        {
            experience_sold_for_force_skills = experience_sold_for_force_skills + skill.Cost;
        }
        public void Decrease_exp_sold_for_force_skills(Force_skill_class skill)
        {
            experience_sold_for_force_skills = experience_sold_for_force_skills - skill.Cost;
        }
        public void Refund_if_not_forceuser()
        {
            Refund_exp_points(experience_sold_for_force_skills);

            // Если игрок передумал создавать персонажа адепта Силы, то возвращаем все очки опыта и обнуляем значения навыков
            foreach (Force_skill_class skill in force_skills)
            {
                skill.Score = 0;
            }
        }
        public void Save_character_to_excel_card()
        {
            Saved_state = true;
        }
        public void Change_character_state_to_unsave()
        {
            Saved_state = false;
        }
        public void Update_character_skills_list (Skill_Class skill)
        {
            bool flag = false;
            foreach (Skill_Class existed_skill in Skills_with_points)
            {
                if (skill.ID == existed_skill.ID)
                {
                    flag = true;
                    if (skill.Get_score() == 0)
                    {
                        Skills_with_points.Remove(existed_skill);
                        limit_skills_left = limit_skills_left + 1;
                    }
                    break;
                }
            }
            if (flag == false)
            {
                if (skill.Get_score() != 0)
                {
                    Skills_with_points.Add(skill);
                    limit_skills_left = limit_skills_left - 1;
                }
            }
            OnPropertyChanged("Skills_with_points");
        }
        public void Update_character_force_skills_list(Force_skill_class skill)
        {
            bool flag = false;
            foreach (Force_skill_class existed_skill in Force_skills_with_points)
            {
                if (skill.ID == existed_skill.ID)
                {
                    flag = true;
                    if (skill.Score == 0)
                    {
                        Force_skills_with_points.Remove(existed_skill);
                        Limit_force_skills_left = Limit_force_skills_left + 1;
                    }
                    break;
                }
            }
            if (flag == false)
            {
                if (skill.Score != 0)
                {
                    Force_skills_with_points.Add(skill);
                    Limit_force_skills_left = Limit_force_skills_left - 1;
                }
            }
            OnPropertyChanged("Force_skills_with_points");
        }
        public void Update_character_combat_abilities_list (All_abilities_template ability)
        {
            bool flag = false;
            foreach (All_abilities_template existed_ability in combat_abilities_with_points)
            {
                if (ability.ID == existed_ability.ID)
                {
                    flag = true;
                    if (ability.Is_chosen == false)
                    {
                        combat_abilities_with_points.Remove(existed_ability);
                    }
                    break;
                }
            }
            if (flag == false)
            {
                if (ability.Is_chosen)
                {
                    combat_abilities_with_points.Add(ability);
                }
            }
            OnPropertyChanged("Combat_abilities_with_points");
        }
        public void Update_character_combat_sequences_list(Abilities_sequence_template sequence)
        {
            bool flag = false;
            foreach (Abilities_sequence_template existed_sequnece in combat_sequences_with_points)
            {
                if (sequence.Name == existed_sequnece.Name)
                {
                    flag = true;
                    if (sequence.Base_ability_lvl != null)
                    {
                        if (sequence.Base_ability_lvl.Is_chosen == false)
                        {
                            combat_sequences_with_points.Remove(existed_sequnece);
                            Limit_all_forms_left = Limit_all_forms_left + 1;
                        }
                    }
                    else if (sequence.Adept_ability_lvl != null)
                    {
                        if (sequence.Adept_ability_lvl.Is_chosen == false)
                        {
                            combat_sequences_with_points.Remove(existed_sequnece);
                            Limit_all_forms_left = Limit_all_forms_left + 1;
                        }
                    }
                    else if (sequence.Master_ability_lvl.Is_chosen == false)
                    {
                        combat_sequences_with_points.Remove(existed_sequnece);
                        Limit_all_forms_left = Limit_all_forms_left + 1;
                    }
                    break;
                }
            }
            if (flag == false)
            {
                if (sequence.Base_ability_lvl.Is_chosen)
                {
                    combat_sequences_with_points.Add(sequence);
                    Limit_all_forms_left = Limit_all_forms_left - 1;
                }
            }
            OnPropertyChanged("Combat_sequences_with_points");
        }
        public void Update_character_force_abilities_list(All_abilities_template ability)
        {
            bool flag = false;
            foreach (All_abilities_template existed_ability in force_abilities_with_points)
            {
                if (ability.ID == existed_ability.ID)
                {
                    flag = true;
                    if (ability.Is_chosen == false)
                    {
                        force_abilities_with_points.Remove(existed_ability);
                    }
                    break;
                }
            }
            if (flag == false)
            {
                if (ability.Is_chosen)
                {
                    force_abilities_with_points.Add(ability);
                }
            }
            OnPropertyChanged("Force_abilities_with_points");
        }
        public void Update_character_force_sequences_list(Abilities_sequence_template sequence)
        {
            bool flag = false;
            foreach (Abilities_sequence_template existed_sequnece in force_sequences_with_points)
            {
                if (sequence.Name == existed_sequnece.Name)
                {
                    flag = true;
                    if (sequence.Base_ability_lvl != null)
                    {
                        if (sequence.Base_ability_lvl.Is_chosen == false)
                        {
                            force_sequences_with_points.Remove(existed_sequnece);
                            Limit_all_forms_left = Limit_all_forms_left + 1;
                        }
                    }
                    else if (sequence.Adept_ability_lvl != null)
                    {
                        if (sequence.Adept_ability_lvl.Is_chosen == false)
                        {
                            force_sequences_with_points.Remove(existed_sequnece);
                            Limit_all_forms_left = Limit_all_forms_left + 1;
                        }
                    }
                    else if (sequence.Master_ability_lvl.Is_chosen == false)
                    {
                        force_sequences_with_points.Remove(existed_sequnece);
                        Limit_all_forms_left = Limit_all_forms_left + 1;
                    }
                    break;
                }
            }
            if (flag == false)
            {
                if (sequence.Base_ability_lvl.Is_chosen)
                {
                    force_sequences_with_points.Add(sequence);
                    Limit_all_forms_left = Limit_all_forms_left - 1;
                }
            }
            OnPropertyChanged("Force_sequences_with_points");
        }
        public void Update_character_positive_feature_list(All_feature_template feature)
        {
            bool flag = false;
            foreach (All_feature_template existed_feature in Positive_features_with_points)
            {
                if (feature.ID == existed_feature.ID)
                {
                    flag = true;
                    Positive_features_with_points.Remove(existed_feature);
                    break;
                }
            }
            if (flag == false)
            {
                Positive_features_with_points.Add(feature);
            }
            OnPropertyChanged("Positive_features_with_points");
        }
        public void Update_character_negative_feature_list(All_feature_template feature)
        {
            bool flag = false;
            foreach (All_feature_template existed_feature in Negative_features_with_points)
            {
                if (feature.ID == existed_feature.ID)
                {
                    flag = true;
                    Negative_features_with_points.Remove(existed_feature);
                    break;
                }
            }
            if (flag == false)
            {
                Negative_features_with_points.Add(feature);
            }
            OnPropertyChanged("Negative_features_with_points");
        }
        public void Calculate_reaction(int bonus)
        {
            Reaction = Reaction + bonus;
        }
        public void Calculate_armor(int bonus)
        {
            Armor = Armor + bonus;
        }
        public void Calculate_watchfullness (int bonus)
        {
            Watchfullness = Watchfullness + bonus;
        }
        public void Calculate_hideness (int bonus)
        {
            Hideness = Hideness + bonus;
        }
        public void Calculate_force_resistance (int bonus)
        {
            Force_resistance = Force_resistance + bonus;
        }
        public void Calculate_concentration (int bonus)
        {
            Concentration = Concentration + bonus;
        }
        public void Update_combat_parameters(Atribute_class attribute, int bonus)
        {
            switch (attribute.Get_atribute_code())
            {
                case 1: break;
                case 2:
                    Calculate_reaction(bonus);
                    Calculate_hideness(bonus);
                    break;

                case 3: break;
                case 4:
                    Calculate_reaction(bonus);
                    Calculate_watchfullness(bonus);
                    break;

                case 5: Calculate_reaction(bonus); break;
                case 6: Calculate_reaction(bonus); break;
                case 7: break;
                case 8: 
                    Calculate_force_resistance(bonus); 
                    if (Forceuser)
                    {
                        Calculate_concentration(bonus); break;
                    }
                    break;
            }
        }
        public void Update_combat_parameters(Skill_Class skill, int bonus)
        {
            switch (skill.Name)
            {
                case "Наблюдательность":    Calculate_watchfullness(bonus); break;
                case "Скрытность":          Calculate_hideness(bonus);      break;
                case "Сопротивление":
                    if (Forceuser == false)
                    {
                        Calculate_force_resistance(bonus);
                    }
                    break;
            }
        }
        public void Update_combat_parameters_due_ForceSkill(Force_skill_class skill, int bonus)
        {
            switch (skill.Name)
            {
                case "Стойкость к Силе": Calculate_force_resistance(bonus); break;
                case "Поток Силы": Calculate_concentration(bonus); break;
            }
        }
        public void Learn_combat_ability(All_abilities_template ability, Abilities_sequence_template sequence)
        {
            foreach (All_abilities_template character_ability in combat_abilities)
            {
                if (character_ability.ID == ability.ID)
                {
                    character_ability.Is_chosen = true;

                    if (sequence.Is_chosen != true)
                    {
                        sequence.Is_chosen = true;
                    }

                    Spend_exp_points(ability.Cost);
                    Update_character_combat_abilities_list(ability);
                    Set_sequence_level(character_ability, sequence);
                    Update_character_combat_sequences_list(sequence);

                    Calculate_reaction          (ability.Reaction_bonus); 
                    Calculate_armor             (ability.Armor_bonus);
                    Calculate_watchfullness     (ability.Watchfullness_bonus);
                    Calculate_hideness          (ability.Stealthness_bonus);
                    Calculate_force_resistance  (ability.Force_resistance_bonus);
                    Calculate_concentration     (ability.Concentration_bonus);

                    Apply_combat_ability_skill_bonus(ability);
                    break;
                }
            }
        }
        public void Delete_combat_ability(All_abilities_template ability, Abilities_sequence_template sequence)
        {
            foreach (All_abilities_template character_ability in combat_abilities)
            {
                if (character_ability.ID == ability.ID)
                {
                    character_ability.Is_chosen = false;

                    if (character_ability == sequence.Base_ability_lvl)
                    {
                        sequence.Is_chosen = false;
                    }

                    Refund_exp_points(character_ability.Cost);
                    Update_character_combat_abilities_list(character_ability);
                    Set_sequence_level(character_ability, sequence);
                    Update_character_combat_sequences_list(sequence);

                    Calculate_reaction          (-ability.Reaction_bonus);
                    Calculate_armor             (-ability.Armor_bonus);
                    Calculate_watchfullness     (-ability.Watchfullness_bonus);
                    Calculate_hideness          (-ability.Stealthness_bonus);
                    Calculate_force_resistance  (-ability.Force_resistance_bonus);
                    Calculate_concentration     (-ability.Concentration_bonus);

                    UnApply_combat_ability_skill_bonus(ability);
                    break;
                }
            }
        }
        public void Learn_force_ability(All_abilities_template ability, Abilities_sequence_template sequence)
        {
            foreach (All_abilities_template character_ability in force_abilities)
            {
                if (character_ability.ID == ability.ID)
                {
                    character_ability.Is_chosen = true;

                    if (sequence.Is_chosen != true)
                    {
                        sequence.Is_chosen = true;
                    }

                    Spend_exp_points(ability.Cost);
                    Update_character_force_abilities_list(ability);
                    Set_sequence_level(character_ability, sequence);
                    Update_character_force_sequences_list(sequence);

                    Calculate_reaction          (ability.Reaction_bonus);
                    Calculate_armor             (ability.Armor_bonus);
                    Calculate_watchfullness     (ability.Watchfullness_bonus);
                    Calculate_hideness          (ability.Stealthness_bonus);
                    Calculate_force_resistance  (ability.Force_resistance_bonus);
                    Calculate_concentration     (ability.Concentration_bonus);

                    break;
                }
            }
        }
        public void Delete_force_ability(All_abilities_template ability, Abilities_sequence_template sequence)
        {
            foreach (All_abilities_template character_ability in force_abilities)
            {
                if (character_ability.ID == ability.ID)
                {
                    character_ability.Is_chosen = false;

                    if (character_ability == sequence.Base_ability_lvl)
                    {
                        sequence.Is_chosen = false;
                    }

                    Refund_exp_points(character_ability.Cost);
                    Update_character_force_abilities_list(character_ability);
                    Set_sequence_level(character_ability, sequence);
                    Update_character_force_sequences_list(sequence);

                    Calculate_reaction          (-ability.Reaction_bonus);
                    Calculate_armor             (-ability.Armor_bonus);
                    Calculate_watchfullness     (-ability.Watchfullness_bonus);
                    Calculate_hideness          (-ability.Stealthness_bonus);
                    Calculate_force_resistance  (-ability.Force_resistance_bonus);
                    Calculate_concentration     (-ability.Concentration_bonus);

                    break;
                }
            }
        }
        public void Learn_positive_feature(All_feature_template feature)
        {
            foreach (All_feature_template character_feature in positive_features)
            {
                if (character_feature.ID == feature.ID)
                {
                    character_feature.Is_chosen = true;
                    Limit_positive_features_left = Limit_positive_features_left - 1;
                    Update_character_positive_feature_list(character_feature);

                    Strength.Increase_atr(character_feature.Strength_bonus);
                    Update_combat_parameters(Strength, character_feature.Strength_bonus);

                    Agility.Increase_atr(character_feature.Agility_bonus);
                    Update_combat_parameters(Agility, character_feature.Agility_bonus);

                    Stamina.Increase_atr(character_feature.Stamina_bonus);
                    Update_combat_parameters(Stamina, character_feature.Stamina_bonus);

                    Quickness.Increase_atr(character_feature.Quickness_bonus);
                    Update_combat_parameters(Quickness, character_feature.Quickness_bonus);

                    Perception.Increase_atr(character_feature.Perception_bonus);
                    Update_combat_parameters(Perception, character_feature.Perception_bonus);

                    Intelligence.Increase_atr(character_feature.Intelligence_bonus);
                    Update_combat_parameters(Intelligence, character_feature.Intelligence_bonus);

                    Charm.Increase_atr(character_feature.Charm_bonus);
                    Update_combat_parameters(Charm, character_feature.Charm_bonus);

                    Willpower.Increase_atr(character_feature.Willpower_bonus);
                    Update_combat_parameters(Willpower, character_feature.Willpower_bonus);

                    Experience = Experience + character_feature.Exp_bonus;
                    
                    Karma = Karma + character_feature.Karma_bonus;

                    Scratch_penalty = (sbyte)(Scratch_penalty + character_feature.Scratch_penalty_bonus);
                    Light_wound_penalty = (sbyte)(Light_wound_penalty + character_feature.Light_wound_penalty_bonus);
                    Medium_wound_penalty = (sbyte)(Medium_wound_penalty + character_feature.Medium_wound_penalty_bonus);
                    Tough_wound_penalty = (sbyte)(Tough_wound_penalty + character_feature.Tough_wound_penalty_bonus);

                    break;
                }
            }
        }
        public void Learn_negative_feature(All_feature_template feature)
        {
            foreach (All_feature_template character_feature in negative_features)
            {
                if (character_feature.ID == feature.ID)
                {
                
                    character_feature.Is_chosen = true;
                    Limit_negative_features_left = Limit_negative_features_left - 1;
                    Update_character_negative_feature_list(character_feature);

                    Strength.Increase_atr(character_feature.Strength_bonus);
                    Update_combat_parameters(Strength, character_feature.Strength_bonus);

                    Agility.Increase_atr(character_feature.Agility_bonus);
                    Update_combat_parameters(Agility, character_feature.Agility_bonus);

                    Stamina.Increase_atr(character_feature.Stamina_bonus);
                    Update_combat_parameters(Stamina, character_feature.Stamina_bonus);

                    Quickness.Increase_atr(character_feature.Quickness_bonus);
                    Update_combat_parameters(Quickness, character_feature.Quickness_bonus);

                    Perception.Increase_atr(character_feature.Perception_bonus);
                    Update_combat_parameters(Perception, character_feature.Perception_bonus);

                    Intelligence.Increase_atr(character_feature.Intelligence_bonus);
                    Update_combat_parameters(Intelligence, character_feature.Intelligence_bonus);

                    Charm.Increase_atr(character_feature.Charm_bonus);
                    Update_combat_parameters(Charm, character_feature.Charm_bonus);

                    Willpower.Increase_atr(character_feature.Willpower_bonus);
                    Update_combat_parameters(Willpower, character_feature.Willpower_bonus);

                    Experience = Experience + character_feature.Exp_bonus;
                    
                    Karma = Karma + character_feature.Karma_bonus;

                    Scratch_penalty = (sbyte)(Scratch_penalty + character_feature.Scratch_penalty_bonus);
                    Light_wound_penalty = (sbyte)(Light_wound_penalty + character_feature.Light_wound_penalty_bonus);
                    Medium_wound_penalty = (sbyte)(Medium_wound_penalty + character_feature.Medium_wound_penalty_bonus);
                    Tough_wound_penalty = (sbyte)(Tough_wound_penalty + character_feature.Tough_wound_penalty_bonus);

                    break;
                }
            }
        }
        public void Delete_positive_feature(All_feature_template feature)
        {
            foreach (All_feature_template character_feature in positive_features)
            {
                if (character_feature.ID == feature.ID)
                {
                    character_feature.Is_chosen = false;
                    Limit_positive_features_left = Limit_positive_features_left + 1;
                    Update_character_positive_feature_list(character_feature);

                    Strength.Increase_atr(-character_feature.Strength_bonus);
                    Update_combat_parameters(Strength, -character_feature.Strength_bonus);

                    Agility.Increase_atr(-character_feature.Agility_bonus);
                    Update_combat_parameters(Agility, -character_feature.Agility_bonus);

                    Stamina.Increase_atr(-character_feature.Stamina_bonus);
                    Update_combat_parameters(Stamina, -character_feature.Stamina_bonus);

                    Quickness.Increase_atr(-character_feature.Quickness_bonus);
                    Update_combat_parameters(Quickness, -character_feature.Quickness_bonus);

                    Perception.Increase_atr(-character_feature.Perception_bonus);
                    Update_combat_parameters(Perception, -character_feature.Perception_bonus);

                    Intelligence.Increase_atr(-character_feature.Intelligence_bonus);
                    Update_combat_parameters(Intelligence, -character_feature.Intelligence_bonus);

                    Charm.Increase_atr(-character_feature.Charm_bonus);
                    Update_combat_parameters(Charm, -character_feature.Charm_bonus);

                    Willpower.Increase_atr(-character_feature.Willpower_bonus);
                    Update_combat_parameters(Willpower, -character_feature.Willpower_bonus);

                    Experience = Experience - character_feature.Exp_bonus;
                    //Experience_left = Experience_left - character_feature.Exp_bonus;

                    Karma = Karma - character_feature.Karma_bonus;

                    Scratch_penalty = (sbyte)(Scratch_penalty - character_feature.Scratch_penalty_bonus);
                    Light_wound_penalty = (sbyte)(Light_wound_penalty - character_feature.Light_wound_penalty_bonus);
                    Medium_wound_penalty = (sbyte)(Medium_wound_penalty - character_feature.Medium_wound_penalty_bonus);
                    Tough_wound_penalty = (sbyte)(Tough_wound_penalty - character_feature.Tough_wound_penalty_bonus);

                    break;
                }
            }
        }
        public void Delete_negative_feature(All_feature_template feature)
        {
            foreach (All_feature_template character_feature in negative_features)
            {
                if (character_feature.ID == feature.ID)
                {
                    character_feature.Is_chosen = false;
                    Limit_negative_features_left = Limit_negative_features_left + 1;
                    Update_character_negative_feature_list(character_feature);

                    Strength.Increase_atr(-character_feature.Strength_bonus);
                    Update_combat_parameters(Strength, -character_feature.Strength_bonus);

                    Agility.Increase_atr(-character_feature.Agility_bonus);
                    Update_combat_parameters(Agility, -character_feature.Agility_bonus);

                    Stamina.Increase_atr(-character_feature.Stamina_bonus);
                    Update_combat_parameters(Stamina, -character_feature.Stamina_bonus);

                    Quickness.Increase_atr(-character_feature.Quickness_bonus);
                    Update_combat_parameters(Quickness, -character_feature.Quickness_bonus);

                    Perception.Increase_atr(-character_feature.Perception_bonus);
                    Update_combat_parameters(Perception, -character_feature.Perception_bonus);

                    Intelligence.Increase_atr(-character_feature.Intelligence_bonus);
                    Update_combat_parameters(Intelligence, -character_feature.Intelligence_bonus);

                    Charm.Increase_atr(-character_feature.Charm_bonus);
                    Update_combat_parameters(Charm, -character_feature.Charm_bonus);

                    Willpower.Increase_atr(-character_feature.Willpower_bonus);
                    Update_combat_parameters(Willpower, -character_feature.Willpower_bonus);

                    Experience = Experience - character_feature.Exp_bonus;
                    Experience_left = Experience_left - character_feature.Exp_bonus;

                    Karma = Karma - character_feature.Karma_bonus;

                    Scratch_penalty         = (sbyte)(Scratch_penalty - character_feature.Scratch_penalty_bonus);
                    Light_wound_penalty     = (sbyte)(Light_wound_penalty - character_feature.Light_wound_penalty_bonus);
                    Medium_wound_penalty    = (sbyte)(Medium_wound_penalty - character_feature.Medium_wound_penalty_bonus);
                    Tough_wound_penalty     = (sbyte)(Tough_wound_penalty - character_feature.Tough_wound_penalty_bonus);

                    break;
                }
            }
            
        }
        public void Spend_positive_feature_points(int cost)
        {
            Positive_features_points_sold = Positive_features_points_sold + cost;
            Positive_features_points_left = Positive_features_points_left - cost;
        }
        public void Refund_positive_feature_points(int cost)
        {
            Positive_features_points_sold = Positive_features_points_sold - cost;
            Positive_features_points_left = Positive_features_points_left + cost;
        }
        public void Spend_negative_feature_points(int cost)
        {
            Negative_features_points_sold = Negative_features_points_sold + cost;
            Negative_features_points_left = Negative_features_points_left - cost;
        }
        public void Refund_negative_feature_points(int cost)
        {
            Negative_features_points_sold = Negative_features_points_sold - cost;
            Negative_features_points_left = Negative_features_points_left + cost;
        }
        public int Return_combat_ability_skill_limit(All_skill_template skill)
        {
            int result = 0;
            if (Combat_abilities_with_points.Count > 0)
            {
                foreach (All_abilities_template ability in Combat_abilities_with_points)
                {
                    if (ability.Skill_bonuses[skill.ID - 1] > 0)
                    {
                        result = result + ability.Skill_bonuses[skill.ID - 1];
                    }
                }
            }
            return result;
        }
        public int Return_skill_limit(All_skill_template skill)
        {
            int result = 0;
            switch (skill.Skill_type)
            {
                case 1:
                    if (Age_status.Skill_limit <= Range.Combat_skill_limit) { result = Age_status.Skill_limit; }
                    else { result = Range.Combat_skill_limit; }
                    break;
                case 2:
                    if (Age_status.Skill_limit <= Range.Surviving_skill_limit) { result = Age_status.Skill_limit; }
                    else { result = Range.Surviving_skill_limit; }
                    break;
                case 3:
                    if (Age_status.Skill_limit <= Range.Charming_skill_limit) { result = Age_status.Skill_limit; }
                    else { result = Range.Charming_skill_limit; }
                    break;
                case 4:
                    if (Age_status.Skill_limit <= Range.Tech_skill_limit) { result = Age_status.Skill_limit; }
                    else { result = Range.Tech_skill_limit; }
                    break;
                case 5:
                    if (Age_status.Skill_limit <= Range.Specific_skill_limit) { result = Age_status.Skill_limit; }
                    else { result = Range.Specific_skill_limit; }
                    break;
            }
            return result;
        }
        // TODO: переделать алгоритм подсчета лимита прокачки навыков на более централизованный способ
        public int Return_summ_skill_limit()
        {
            int result = 0;
            return result;
        }
        public int Return_race_skill_bonus(All_skill_template skill)
        {
            return Character_race.Race_skill_bonus[skill.ID - 1];
        }
        public int Return_total_feature_score()
        {
            int result = Positive_features_points_sold - Negative_features_points_sold;
            features_balanced = false;
            if (result == 0)
            {
                features_balanced = true;
            }
            return result;
        }
        public void Refresh_fields()
        {
            OnPropertyChanged("Skills_with_points");
            OnPropertyChanged("Force_skills_with_points");
        }


        public static Character GetInstance()
        {
            if (Character_instance == null)
            {
                Character_instance = new Character();
            }
            return Character_instance;
        }



        public Atribute_class Strength
        {
            get { return strength; }
            set { strength = value; OnPropertyChanged("Strength"); }
        }
        public Atribute_class Agility
        {
            get { return agility; }
            set { agility = value; OnPropertyChanged("Agility"); }
        }
        public Atribute_class Stamina
        {
            get { return stamina; }
            set { stamina = value; OnPropertyChanged("Stamina"); }
        }
        public Atribute_class Perception
        {
            get { return perception; }
            set { perception = value; OnPropertyChanged("Perception"); }
        }
        public Atribute_class Quickness
        {
            get { return quickness; }
            set { quickness = value; OnPropertyChanged("Quickness"); }
        }
        public Atribute_class Intelligence
        {
            get { return intelligence; }
            set { intelligence = value; OnPropertyChanged("Intelligence"); }
        }
        public Atribute_class Charm
        {
            get { return charm; }
            set { charm = value; OnPropertyChanged("Charm"); }
        }
        public Atribute_class Willpower
        {
            get { return willpower; }
            set { willpower = value; OnPropertyChanged("Willpower"); }
        }
        public Age_status_class Age_status
        {
            get { return age_status; }
            set { age_status = value; OnPropertyChanged("Age_status"); }
        }
        public Range_Class Range
        {
            get { return range; }
            set { range = value; OnPropertyChanged("Range"); }
        }
        public Race_class Character_race
        {
            get { return character_race; }
            set { character_race = value; OnPropertyChanged("Character_race"); }
        }
        public bool Forceuser
        {
            get { return Is_forceuser; }
            set { Is_forceuser = value; OnPropertyChanged("Forceuser"); }
        }
        public int Experience
        {
            get { return experience; }
            set
            {
                experience = value;
                Spend_exp_points(0);
                OnPropertyChanged("Experience");
            }
        }
        public int Experience_left
        {
            get { return experience_left; }
            set { experience_left = value; OnPropertyChanged("Experience_left"); }
        }
        public int Experience_sold
        {
            get { return experience_sold; }
            set { experience_sold = value; OnPropertyChanged("Experience_sold"); }
        }
        public int Attributes
        {
            get { return attributes; }
            set
            {
                attributes = value;
                Spend_atr_points(0);
                OnPropertyChanged("Attributes");
            }
        }
        public int Attributes_left
        {
            get { return attributes_left; }
            set { attributes_left = value; OnPropertyChanged("Attributes_left"); }
        }
        public int Attributes_sold
        {
            get { return attributes_sold; }
            set { attributes_sold = value; OnPropertyChanged("Attributes_sold"); }
        }
        public int Positive_features_points_sold
        {
            get { return positive_features_points_sold; }
            set { positive_features_points_sold = value; OnPropertyChanged("Positive_features_points_sold"); }
        }
        public int Negative_features_points_sold
        {
            get { return negative_features_points_sold; }
            set { negative_features_points_sold = value; OnPropertyChanged("Negative_features_points_sold"); }
        }
        public int Positive_features_points_left
        {
            get { return positive_features_points_left; }
            set { positive_features_points_left = value; OnPropertyChanged("Positive_features_points_left"); }
        }
        public int Negative_features_points_left
        {
            get { return negative_features_points_left; }
            set { negative_features_points_left = value; OnPropertyChanged("Negative_features_points_left"); }
        }
        public List<Skill_Class> Skills
        {
            get { return skills; }
            set { skills = value; OnPropertyChanged("Skills"); }
        }
        public List<All_skill_template> Skills_with_points
        {
            get 
            { 
                foreach (var count in skills_with_points)
                {
                    count.Skill_max_score = Return_skill_limit(count) + Return_race_skill_bonus(count);
                }
                return skills_with_points; 
            }
        }
        public List<All_skill_template> Force_skills_with_points
        {
            get 
            {
                foreach (var count in force_skills_with_points)
                {
                    count.Skill_max_score = Age_status.Force_skill_limit;
                }
                return force_skills_with_points; 
            }
        }
        public List<All_feature_template> Positive_features_with_points
        {
            get { return positive_features_with_points; }
        }
        public List<All_feature_template> Negative_features_with_points
        {
            get { return negative_features_with_points; }
        }
        public List<All_feature_template> Positive_features
        {
            get { return positive_features; }
        }
        public List<All_feature_template> Negative_features
        {
            get { return negative_features; }
        }
        public List<All_feature_template> Charm_features
        {
            get { return charm_features; }
        }
        public List<All_feature_template> Hero_features
        {
            get { return hero_features; }
        }
        public List<All_feature_template> Sleep_feature
        {
            get { return sleep_feature; }
        }
        public List<All_feature_template> Alcohol_feature
        {
            get { return alcohol_feature; }
        }
        public List<All_feature_template> Sith_feature
        {
            get { return sith_feature; }
        }
        public List<All_feature_template> Jedi_feature
        {
            get { return jedi_feature; }
        }
        public List<All_feature_template> Exp_feature
        {
            get { return exp_feature; }
        }
        public List<All_feature_template> Armor_feature
        {
            get { return armour_feature; }
        }
        public int Age
        {
            get { return age; }
            set { age = value; OnPropertyChanged("Age"); }
        }
        public string Sex
        {
            get { return sex; }
            set { sex = value; OnPropertyChanged("Sex"); }
        }
        public int Karma
        {
            get { return karma; }
            set { karma = value; OnPropertyChanged("Karma"); }
        }
        public bool Is_sith
        {
            get { return is_sith; }
            set { is_sith = value; OnPropertyChanged("Is_sith"); }
        }
        public bool Is_jedi
        {
            get { return is_jedi; }
            set { is_jedi = value; OnPropertyChanged("Is_jedi"); }
        }
        public bool Is_neutral
        {
            get { return is_neutral; }
            set { is_neutral = value; OnPropertyChanged("Is_neutral"); }
        }
        public bool Saved_state
        {
            get { return saved_state; }
            set { saved_state = value; OnPropertyChanged("Saved_state"); }
        }
        public bool Features_balanced
        {
            get { return features_balanced; }
            set { features_balanced = value; OnPropertyChanged("features_balanced"); }
        }
        public int Reaction
        {
            get { return reaction; }
            set { reaction = value; OnPropertyChanged("Reaction"); }
        }
        public int Armor
        {
            get { return armor; }
            set { armor = value; OnPropertyChanged("Armor"); }
        }
        public int Watchfullness
        {
            get { return watchfulness; }
            set { watchfulness = value; OnPropertyChanged("Watchfulness"); }
        }
        public int Hideness
        {
            get { return hideness; }
            set { hideness = value; OnPropertyChanged("Hideness"); }
        }
        public int Force_resistance
        {
            get { return force_resistance; }
            set { force_resistance = value; OnPropertyChanged("Force_resistance"); }
        }
        public int Concentration
        {  
            get { return concentration; }
            set { concentration = value; OnPropertyChanged("Concentration"); }
        }
        public List<All_abilities_template> Combat_abilities
        {
            get { return combat_abilities; }
            set { combat_abilities = value;OnPropertyChanged("Combat_abilities"); }
        }
        public List<All_abilities_template> Force_abilities
        {
            get { return force_abilities; }
            set { force_abilities = value; OnPropertyChanged("Force_abilities"); }
        }
        public List<All_abilities_template> Combat_abilities_with_points
        {
            get { return combat_abilities_with_points; }
            set { combat_abilities_with_points = value; OnPropertyChanged("Combat_abilities_with_points"); }
        }
        public List<All_abilities_template> Force_abilities_with_points
        {
            get { return force_abilities_with_points; }
            set { force_abilities_with_points = value; OnPropertyChanged("Force_abilities_with_points"); }
        }
        public List<Abilities_sequence_template> Combat_sequences_with_points
        {
            get { return combat_sequences_with_points; }
            set { combat_sequences_with_points = value; OnPropertyChanged("Combat_sequences_with_points"); }
        }
        public List<Abilities_sequence_template> Force_sequences_with_points
        {
            get { return force_sequences_with_points; }
            set { force_sequences_with_points = value; OnPropertyChanged("Force_sequences_with_points"); }
        }
        public List<int> Skill_limits
        {
            get { return skill_limits; }
            set { skill_limits = value; OnPropertyChanged("Skill_limits"); }
        }
        public int Limit_all_forms_left
        {
            get { return limit_all_forms_left; }
            set { limit_all_forms_left = value; OnPropertyChanged("Limit_all_forms_left"); }
        }
        public int Limit_force_skills_left
        {
            get { return limit_force_skills_left; }
            set { limit_force_skills_left = value; OnPropertyChanged("Limit_force_skills_left"); }
        }
        public int Limit_skills_left
        {
            get { return limit_skills_left; }
            set { limit_skills_left = value; OnPropertyChanged("Limit_skills_left"); }
        }
        public int Limit_positive_features_left
        {
            get { return limit_positive_features_left; }
            set { limit_positive_features_left = value; OnPropertyChanged("Limit_positive_features_left"); }
        }
        public int Limit_negative_features_left
        {
            get { return limit_negative_features_left; }
            set { limit_negative_features_left = value; OnPropertyChanged("Limit_negative_features_left"); }
        }
        public sbyte Scratch_penalty
        {
            get 
            {
                if (visible_scratch_penalty == 0) { return visible_scratch_penalty; }
                else { return scratch_penalty; }
            }
            set 
            { 
                if (Scratch_penalty + value > 0)
                {
                    visible_scratch_penalty = 0;
                }
                scratch_penalty = value; OnPropertyChanged("Scratch_penalty"); 
            }
        }
        public sbyte Light_wound_penalty
        {
            get 
            {
                if (visible_light_wound_penalty == 0) { return visible_light_wound_penalty; }
                else { return light_wound_penalty; }
            }
            set 
            {
                if (Light_wound_penalty + value > 0)
                {
                    visible_light_wound_penalty = 0;
                }
                light_wound_penalty = value; OnPropertyChanged("Light_wound_penalty"); 
            }
        }
        public sbyte Medium_wound_penalty
        {
            get 
            {
                if (visible_medium_wound_penalty == 0) { return visible_medium_wound_penalty; }
                else { return medium_wound_penalty; }
            }
            set 
            {
                if (Medium_wound_penalty + value > 0)
                {
                    visible_medium_wound_penalty = 0;
                }
                medium_wound_penalty = value; OnPropertyChanged("Medium_wound_penalty"); 
            }
        }
        public sbyte Tough_wound_penalty
        {
            get 
            {
                if (visible_tough_wound_penalty == 0) { return visible_tough_wound_penalty; }
                else { return tough_wound_penalty; }
            }
            set 
            {
                if (Tough_wound_penalty + value > 0)
                {
                    visible_tough_wound_penalty = 0;
                }
                tough_wound_penalty = value; OnPropertyChanged("Tough_wound_penalty"); 
            }
        }
        public byte Scratch_lvl
        {
            get { return scratch_lvl; }
            set { scratch_lvl = value; OnPropertyChanged("Scratch_lvl"); }
        }
        public byte Light_wound_lvl
        {
            get { return light_wound_lvl; }
            set { light_wound_lvl = value; OnPropertyChanged("Light_wound_lvl"); }
        }
        public byte Medium_wound_lvl
        {
            get { return medium_wound_lvl; }
            set { medium_wound_lvl = value; OnPropertyChanged("Medium_wound_lvl"); }
        }
        public byte Tough_wound_lvl
        {
            get { return tough_wound_lvl; }
            set { tough_wound_lvl = value; OnPropertyChanged("Tough_wound_lvl"); }
        }
        public byte Mortal_wound_lvl
        {
            get { return mortal_wound_lvl; }
            set { mortal_wound_lvl = value; OnPropertyChanged("Mortal_wound_lvl"); }
        }
        public byte Melee_attack
        {
            get { return melee_attack; }
            set { melee_attack = value; OnPropertyChanged("Melee_attack"); }
        }
        public byte Ranged_attack
        {
            get { return ranged_attack; }
            set { ranged_attack = value; OnPropertyChanged("Ranged_attack"); }
        }
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }



        public Character()
        {
            Character_race  = Main_model.GetInstance().Race_Manager.Get_Race_list()[0];
            Age_status      = Main_model.GetInstance().Age_status_Manager.Age_Statuses()[0]; // устанавливаем возрастной статус "Неизвестно" персонажу
            Range           = Main_model.GetInstance().Range_Manager.Ranges()[0]; // устанавливаем ранг "Рядовой" персонажу

            Strength        = Main_model.GetInstance().Attribute_Manager.Get_strength();
            Agility         = Main_model.GetInstance().Attribute_Manager.Get_agility();
            Stamina         = Main_model.GetInstance().Attribute_Manager.Get_stamina();
            Quickness       = Main_model.GetInstance().Attribute_Manager.Get_quickness();
            Perception      = Main_model.GetInstance().Attribute_Manager.Get_perception();
            Intelligence    = Main_model.GetInstance().Attribute_Manager.Get_intelligence();
            Charm           = Main_model.GetInstance().Attribute_Manager.Get_charm();
            Willpower       = Main_model.GetInstance().Attribute_Manager.Get_willpower();

            skills = new List<Skill_Class>();
            skill_limits = new List<int>();
            foreach (Skill_Class Skill in Main_model.GetInstance().Skill_Manager.Get_skills())
            {
                skills.Add(Skill);
                skill_limits.Add(new int());
            }

            force_skills = new List<Force_skill_class>();
            foreach (Force_skill_class force_skill in Main_model.GetInstance().Force_skill_Manager.Get_Force_Skills())
            {
                force_skills.Add(force_skill);
            }
            
            combat_abilities = new List<All_abilities_template>();
            foreach (Combat_abilities_template combat_ability in Main_model.GetInstance().Combat_ability_Manager.Get_abilities())
            {
                combat_abilities.Add(combat_ability);
            }

            force_abilities = new List<All_abilities_template>();
            foreach (Force_abilities_template force_ability in Main_model.GetInstance().Force_ability_Manager.Get_abilities())
            {
                force_abilities.Add(force_ability);          
            }

            charm_features      = new List<All_feature_template>();
            hero_features       = new List<All_feature_template>();
            positive_features   = new List<All_feature_template>();
            sleep_feature       = new List<All_feature_template>();
            alcohol_feature     = new List<All_feature_template>();
            sith_feature        = new List<All_feature_template>();
            jedi_feature        = new List<All_feature_template>();
            exp_feature         = new List<All_feature_template>();
            armour_feature      = new List<All_feature_template>();
            foreach (All_feature_template feature in Main_model.GetInstance().Feature_Manager.Get_positive_features())
            {
                positive_features.Add(feature);
                switch(feature.ID)
                {
                    case 20: charm_features.Add(feature); break;
                    case 22: sleep_feature. Add(feature); break;
                    case 28: alcohol_feature.Add(feature); break;
                    case 35: charm_features.Add(feature); break;
                    case 37: armour_feature.Add(feature); break;
                    case 39: charm_features.Add(feature); break;
                    case 40: exp_feature.Add(feature); break;
                    case 43: armour_feature.Add(feature); break;
                    case 45: exp_feature.Add(feature); break;
                    case 46: exp_feature.Add(feature); break;
                    case 90: jedi_feature.Add(feature); break;
                    case 91: sith_feature.Add(feature); break;
                }
            }

            negative_features = new List<All_feature_template>();
            foreach (All_feature_template feature in Main_model.GetInstance().Feature_Manager.Get_negative_features())
            {
                negative_features.Add(feature);
                switch (feature.ID)
                {
                    case 49: charm_features.Add(feature); break;
                    case 56: sleep_feature. Add(feature); break;
                    case 58: alcohol_feature.Add(feature); break;
                    case 59: charm_features.Add(feature); break;
                    case 61: charm_features.Add(feature); break;
                    case 66: charm_features.Add(feature); break;
                    case 69: charm_features.Add(feature); break;
                    case 70: hero_features. Add(feature); break;
                    case 78: charm_features.Add(feature); break;
                    case 79: hero_features. Add(feature); break;
                    case 81: hero_features. Add(feature); break;
                    case 86: armour_feature.Add(feature); break;
                    case 102: jedi_feature.Add(feature); break;
                    case 101: sith_feature.Add(feature); break;
                }
            }

            limit_all_forms         = 8;
            limit_force_skills      = 17;
            limit_skills            = 34;
            limit_positive_features = 8;
            limit_negative_features = 8;

            limit_all_forms_left            = limit_all_forms;
            limit_force_skills_left         = limit_force_skills;
            limit_skills_left               = limit_skills;
            limit_positive_features_left    = limit_positive_features;
            limit_negative_features_left    = limit_negative_features;

            Positive_features_points_left = 10;
            Negative_features_points_left = 10;

            scratch_penalty = 0;
            light_wound_penalty = -2;
            medium_wound_penalty = -4;
            tough_wound_penalty = -8;
            visible_scratch_penalty = scratch_penalty;
            visible_light_wound_penalty = light_wound_penalty;
            visible_medium_wound_penalty = medium_wound_penalty;
            visible_tough_wound_penalty = tough_wound_penalty;

            Scratch_lvl = 2;
            Light_wound_lvl = 8;
            Medium_wound_lvl = 14;
            Tough_wound_lvl = 20;
            Mortal_wound_lvl = 26;

            Melee_attack = 0;
            Ranged_attack = 0;

            combat_abilities_with_points    = new List<All_abilities_template>();
            force_abilities_with_points     = new List<All_abilities_template>();
            skills_with_points              = new List<All_skill_template>();
            force_skills_with_points        = new List<All_skill_template>();
            combat_sequences_with_points    = new List<Abilities_sequence_template>();
            force_sequences_with_points     = new List<Abilities_sequence_template>();
            positive_features_with_points   = new List<All_feature_template>();
            negative_features_with_points   = new List<All_feature_template>();
            
            Saved_state = false;
            Forceuser = false;
            Is_neutral = true;
            Is_sith = false;
            Is_jedi = false;
            Features_balanced = true;
        }



        private void Spend_atr_points(int cost)
        {
            Attributes_sold = Attributes_sold + cost;
            Attributes_left = Attributes - Attributes_sold;
        }
        private void Refund_atr_points(int cost)
        {
            Attributes_sold = Attributes_sold - cost;
            Attributes_left = Attributes - Attributes_sold;
        }
        private void Apply_combat_ability_skill_bonus(All_abilities_template ability)
        {
            int i = 0;
            foreach (int skill_bonus in ability.Skill_bonuses)
            {
                Skills[i].Set_score(Skills[i].Get_score() + skill_bonus);

                Update_character_skills_list(Skills[i]);
                i = i + 1;
            }
        }
        private void UnApply_combat_ability_skill_bonus(All_abilities_template ability)
        {
            int i = 0;
            foreach (int skill_bonus in ability.Skill_bonuses)
            {
                Skills[i].Set_score(Skills[i].Get_score() - skill_bonus);

                Update_character_skills_list(Skills[i]);
                i = i + 1;
            }
        }
        private void Set_sequence_level (All_abilities_template ability, Abilities_sequence_template sequence)
        {
            if (sequence.Base_ability_lvl != null )
            {
                if (sequence.Base_ability_lvl == ability && ability.Is_chosen)
                {
                    sequence.Level = "Ученик";
                }
                else if (sequence.Base_ability_lvl == ability && ability.Is_chosen == false)
                {
                    sequence.Level = "Не выбрано";
                }
            }
            if (sequence.Adept_ability_lvl != null)
            {
                if (sequence.Adept_ability_lvl == ability && ability.Is_chosen)
                {
                    sequence.Level = "Адепт";
                }
                else if (sequence.Adept_ability_lvl == ability && ability.Is_chosen == false)
                {
                    sequence.Level = "Ученик";
                }
            }
            if (sequence.Master_ability_lvl != null)
            {
                if (sequence.Master_ability_lvl == ability && ability.Is_chosen)
                {
                    sequence.Level = "Мастер";
                }
                else if (sequence.Master_ability_lvl == ability && ability.Is_chosen == false)
                {
                    sequence.Level = "Адепт";
                }
            }
        }
    }
}
