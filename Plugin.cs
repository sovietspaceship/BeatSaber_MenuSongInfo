using BS_Utils.Utilities;
using IPA;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.Components;
using UnityEngine;
using System.Linq;
using IPALogger = IPA.Logging.Logger;
using System;


namespace MenuSongInfo
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static IPALogger Logger { get; private set; }

        [Init]
        public Plugin(IPALogger logger)
        {
            Logger = logger;
        }

            [OnStart]
        public void OnStart()
        {
            BSEvents.gameSceneLoaded += OnGameSceneLoaded;
        }

        private void OnGameSceneLoaded()
        {
            try
            {
                var canvas = GameObject.Find("Wrapper/StandardGameplay/PauseMenu/Wrapper/MenuWrapper/Canvas").GetComponent<Canvas>();
                if (!canvas) { return; }

                var toggleObject = new TextTag().CreateObject(canvas.transform);
                (toggleObject.transform as RectTransform).anchoredPosition = new Vector2(5, 0);
                (toggleObject.transform as RectTransform).sizeDelta = new Vector2(-130, 7);

                IDifficultyBeatmap CurrentDifficultyBeatmap = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap;

                FormattableText textObject = toggleObject.transform.GetComponent<FormattableText>();
                textObject.text = "100%";

                ScoreController scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().LastOrDefault(x => x.isActiveAndEnabled);
                int score = 0;

                Action UpdateText = () => {
                    string Acc = (100 * (float)score / scoreController.immediateMaxPossibleRawScore).ToString("0.00") + "%";
                    textObject.text = Acc;
                };

                scoreController.scoreDidChangeEvent += (int scoreBefore, int scoreAfter) => {
                    score = scoreAfter;
                    UpdateText();
                };

                scoreController.noteWasMissedEvent += (NoteData noteData, int multiplier) => {
                    UpdateText();
                };
            }
            catch (Exception e) {
                Logger.Error(e);
            }
        }
    }
}
