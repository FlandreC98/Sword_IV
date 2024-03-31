
using rail;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Sword_IV.Items
{

    public class Sword : ModItem
    {
        public readonly float FRAME_TICK = 1 / 60f;
        private bool active = false;


        private int extraHpScale = 30;
        private int extraMpScale = 30;

        private readonly float convertTick = 0.22f;
        private float currentConvertTimer = 0f;
        private int mpLossScale = 30;
        private int hpAddScale = 70;

        private int extraHpTranslate = 30;
        private int extraMpTranslate = 30;

        /// <summary>
        /// 生命恢复效率，每两秒
        /// </summary>
        private int recoverHpRegen = 20;

        private int defense = 40;
        private float endurance = 0.5f;
        private float hpTrigger = 0.4f;



        float duration = 5;
        float currentDuration = 0;


        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(
            extraHpScale, extraMpScale,
            hpTrigger * 100, duration, endurance * 100,
            mpLossScale, hpAddScale
            );

        /// <summary>
        /// 合成表
        /// </summary>
        public override void AddRecipes()
        {

            CreateRecipe()
                // 这样可以生成50个
                // CreateRecipe(50);
                // 合成材料，需要10个泥土块
                // 哦对了，如果你写俩种相同的材料的话，它们会分别显示而不是合并成一个（我家门前有两颗树，一颗是枣树，另一颗还是枣树）
                .AddIngredient(ItemID.Wood, 10)
                // 使用ModContent.ItemType<xxx>()来获取你的物品的type，不过在这里，你可以使用Type
                //.AddIngredient(/*ModContent.ItemType<SkirtSword>()*/Type, 1)
                // 以及在工作台旁边
                //.AddTile(TileID.WorkBenches)
                // 把这个合成表装进tr的系统里
                .Register();

        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
            Item.defense = defense;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.lifeRegen += recoverHpRegen;
            player.statLifeMax2 += player.statLifeMax2 * extraHpScale / 100;
            player.statManaMax2 += player.statManaMax2 * extraMpScale / 100;

            IsActive(ref player);
            if (currentDuration > 0)
            {
                currentDuration -= FRAME_TICK;
                active = true;
            }
            else if (active) 
            {
                active = false;
                ResetTimer();
            }
               
            if (active)
            {
                int offset = player.statManaMax2 * mpLossScale / 100;
                if (player.statMana > offset && player.statLife < player.statLifeMax2 - offset) {
                    currentConvertTimer += FRAME_TICK;
                    MpTranslate2Hp(player, offset);
                }
                player.endurance += endurance;
                player.lifeRegenTime += 100;
            }
            
        }

        private void IsActive(ref Player player) {
            bool resule = (player.statDefense < defense || player.statLife <= (player.statLifeMax2 * hpTrigger));
            if (resule && currentDuration <= 0)
            {
                currentDuration = duration;
            }
        }

        private void MpTranslate2Hp(Player player, int offset) 
        {
            if (currentConvertTimer > convertTick)
            {
                player.statMana -= offset;
                player.Heal(offset * hpAddScale /100);
                currentConvertTimer = 0;
            }
        }
        private void ResetTimer()
        {
            currentConvertTimer = 0;
            currentDuration = 0;
        }

    }
}