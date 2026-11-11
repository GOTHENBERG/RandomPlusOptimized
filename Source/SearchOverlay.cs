using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RandomPlus
{
    /// <summary>
    /// Renders an interactive HUD overlay over the pawn portrait area while a search session is active.
    /// Displays a live reroll counter, candidate preview, and a Stop button while preventing
    /// wasteful portrait rendering of churning intermediate candidates.
    /// </summary>
    [HarmonyPatch(typeof(StartingPawnUtility), "DrawPortraitArea")]
    static class Patch_SearchOverlay
    {
        [HarmonyPrefix]
        static bool Prefix(Rect rect, int pawnIndex)
        {
            if (!SearchSession.IsActive || pawnIndex != SearchSession.SearchingPawnIndex)
                return true;

            // Pump up to 8ms of candidates per frame
            SearchSession.PumpStep(8.0);

            Widgets.DrawMenuSection(rect);

            var previousAnchor = Text.Anchor;
            var previousFont = Text.Font;
            Text.Anchor = TextAnchor.MiddleCenter;

            Text.Font = GameFont.Small;
            Widgets.Label(Band(rect, 0.24f), "RandomPlus.SearchOverlay.Title".Translate());

            Text.Font = GameFont.Medium;
            Widgets.Label(Band(rect, 0.39f),
                $"{RandomSettings.RandomRerollCounter():N0} / {RandomSettings.PawnFilter.RerollLimit:N0}");

            Text.Font = GameFont.Small;
            Widgets.Label(Band(rect, 0.54f), SearchSession.SampledCandidateInfo);

            var stopArea = Band(rect, 0.69f);
            var stopButton = new Rect(stopArea.x + ((stopArea.width - 130f) / 2f), stopArea.y, 130f, 30f);
            if (Widgets.ButtonText(stopButton, "RandomPlus.SearchOverlay.StopButton".Translate()))
            {
                SearchSession.Cancel();
            }

            Text.Anchor = previousAnchor;
            Text.Font = previousFont;
            return false;
        }

        private static Rect Band(Rect rect, float fraction) =>
            new Rect(rect.x, rect.y + (rect.height * fraction), rect.width, 30f);
    }
}
