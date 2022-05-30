using System.IO;
using System.Text.RegularExpressions;

class patch_Draw : Draw
{
    extern public static void orig_UpdateCameraMode();
    new public static void UpdateCameraMode()
    {
        // We don't want to display the hub on the title,
        // under any circumstances
        if (patch_World.inCustomHub)
        {
            World.FirstLevelSinceStartup = false;
        }
        orig_UpdateCameraMode();
    }

    extern public static void orig_ConstructCreditsString();
    new public static void ConstructCreditsString()
    {
        if (patch_World.inCustomHub)
        {
            string path;
            if (patch_World.paths.TryGetValue("credits", out path))
            {
                creditsString = World.ReadTextFile(path);
                creditsLines = Regex.Matches(creditsString, "\n").Count;
                return;
            }
        }
        orig_ConstructCreditsString();
    }

    extern public static void orig_PerpetualZoomOutPickNewBlock();
    new public static void PerpetualZoomOutPickNewBlock()
    {
        if (!patch_World.inCustomHub
            || !patch_LoadLevel.IsFirstArea(ZoomOutAnimFocusBlock.SubLevel))
        {
            orig_PerpetualZoomOutPickNewBlock();
            return;
        }
        // This tells parabox to still run its checks, but take this to be the last block
        int oldEffect = ZoomOutAnimFocusBlock.SpecialEffect;
        ZoomOutAnimFocusBlock.SpecialEffect = 6;
        orig_PerpetualZoomOutPickNewBlock();
        ZoomOutAnimFocusBlock.SpecialEffect = oldEffect;
    }
    
    //epsilon jank 
    extern public static void orig_DrawInfEffect(
      double xCenter,
      double yCenter,
      double width,
      double height,
      Block block,
      bool cloneFade);
    new public static void DrawInfEffect(
      double xCenter,
      double yCenter,
      double width,
      double height,
      Block block,
      bool cloneFade) {
        float num1 = (float) (width + height) / 2f / (float) Draw.screenHeight;
        if (World.ScreenshottingBlocks)
            num1 *= 0.2f;
        float num2 = (float) (-(double) Mathf.Log10(num1 + 0.5f) + 0.22);
        if (cloneFade) {
            float num3 = Draw.AnimT / Draw.AnimLength;
            num2 *= 1f - num3;
        }
        Texture texture;
        Color white;
        int num4;
        bool amognus = false;
        if (block.IsSomeInfEnterBlock) {
            texture = Draw.instance.InfParticleOutlineTex;
            white = Color.white;
            white.a = num2;
            if (block.SomeInfEnterNums != null && block.SomeInfEnterNums.Count > 1) {
                amognus = true;
            }
            num4 = block.SomeInfEnterNum + 1;
        } else {
            texture = Draw.instance.InfParticleTex;
            white = Color.white;
            white.a = num2;
            num4 = block.SubLevel.infExitBlocks.IndexOf(block) + 1;
        }
        if (amognus) {
            int t = block.SomeInfEnterNums.Count;
            foreach (int n in block.SomeInfEnterNums) if (n + 1 > t) t = n + 1;

            float num3 = (float) width / (float) (t + 1);
            float num5 = (float) height / (float) (t + 1);
            for (int index = 0; index < block.SomeInfEnterNums.Count; ++index) {
                num4 = block.SomeInfEnterNums[index] + 1;
                float num6 = ((float) height - num5 * (float) t) / (float) (t + 1);
                float num69 = ((float) width - num3 * (float) t) / (float) (t + 1);
                float num7 = num3 * 1.8f;
                float num8 = num5 * 1.8f;
                Draw.SetDrawRect(xCenter - (double) num7 / 2.0 - ((double) num3 / 2.0 + (double) num69 / 2.0) * (double) (block.SomeInfEnterNums.Count - 1) + (num3 + num69) * index, yCenter - (double) num8 / 2.0 - ((double) num5 / 2.0 + (double) num6 / 2.0) * (double) (num4 - 1), (double) num7, (double) num8);
                for (int index2 = 0; index2 < num4; ++index2) {
                    Draw.DrawTexture(texture, white);
                    Draw.drawRect.y += num5 + num6;
                }
            }
        } else if (num4 <= 0)
            Debug.LogWarning((object) ("numSymbols was <0: " + num4.ToString() + " this should not happen."));
        else if (num4 == 1) {
            double width1 = width * 0.9;
            double height1 = height * 0.9;
            Draw.SetDrawRect(xCenter - width1 / 2.0, yCenter - height1 / 2.0, width1, height1);
            Draw.DrawTexture(texture, white);
        } else {
            double num3 = width / (double) (num4 + 1);
            float num5 = (float) height / (float) (num4 + 1);
            float num6 = ((float) height - num5 * (float) num4) / (float) (num4 + 1);
            float num7 = (float) (num3 * 1.8);
            float num8 = num5 * 1.8f;
            Draw.SetDrawRect(xCenter - (double) num7 / 2.0, yCenter - (double) num8 / 2.0 - ((double) num5 / 2.0 + (double) num6 / 2.0) * (double) (num4 - 1), (double) num7, (double) num8);
            for (int index = 0; index < num4; ++index) {
                Draw.DrawTexture(texture, white);
                Draw.drawRect.y += num5 + num6;
            }
        }
    }
}
