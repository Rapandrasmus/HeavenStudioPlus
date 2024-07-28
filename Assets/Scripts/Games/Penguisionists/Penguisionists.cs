using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;

using Jukebox;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;

    public static class PcoPenguisionistsLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("penguisionists", "Penguisionists", "ffffff", false, false, new List<GameAction>()
            {
                new("jump", "Jump")
                {
                    preFunction = delegate { Penguisionists.JumpSound(eventCaller.currentEntity.beat); },
                    defaultLength = 4,
                },
                new("togetherJump", "Together Jump")
                {
                    preFunction = delegate { Penguisionists.TogetherJumpSound(eventCaller.currentEntity.beat); },
                    defaultLength = 3,
                },
                new("whistleJump", "Whistle Jump")
                {
                    preFunction = delegate { Penguisionists.WhistleJumpSound(eventCaller.currentEntity.beat); },
                    defaultLength = 5,
                }
            }
            );
        }
    }
}

namespace HeavenStudio.Games
{
    using HeavenStudio.Games.Scripts_MeatGrinder;
    using Scripts_Penguisionists;
    public class Penguisionists : Minigame
    {
        #region Serialised Properties

        [SerializeField] private Penguin _penguin1;
        [SerializeField] private Penguin _penguin2;
        [SerializeField] private Penguin _penguin3;

        #endregion

        #region Jumps

        private void Jump(double beat, double startBeat)
        {
            PenguinsJump(beat, 1, 2, 3, startBeat);
        }

        private void TogetherJump(double beat, double startBeat)
        {
            PenguinsJump(beat, 2, 2, 2, startBeat, true);
        }

        private void WhistleJump(double beat, double startBeat)
        {
            PenguinsJump(beat, 1, 3, 4, startBeat);
        }

        private void PenguinsJump(double baseBeat, double beat1, double beat2, double beat3, double startBeat, bool together = false)
        {
            _penguin1.QueueJump(baseBeat + beat1, startBeat, together);
            _penguin2.QueueJump(baseBeat + beat2, startBeat, together);
            _penguin3.QueueJump(baseBeat + beat3, startBeat, together);
        }

        #endregion

        #region Setup

        public void Awake()
        {
            _penguin1.Init(this);
            _penguin2.Init(this);
            _penguin3.Init(this);
        }

        public override void OnGameSwitch(double beat)
        {
            HandleEvents(beat);
        }

        public override void OnPlay(double beat)
        {
            HandleEvents(beat);
        }

        private void HandleEvents(double beat)
        {
            var jumps = EventCaller.GetAllInGameManagerList("penguisionists", new string[] { "jump", "togetherJump", "whistleJump" }).FindAll(x => x.beat + x.length > beat);

            foreach (var jump in jumps)
            {
                switch (jump.datamodel)
                {
                    case "penguisionists/jump":
                        Jump(jump.beat, beat);
                        break;
                    case "penguisionists/togetherJump":
                        TogetherJump(jump.beat, beat);
                        break;
                    case "penguisionists/whistleJump":
                        WhistleJump(jump.beat, beat);
                        break;
                    default: break;
                }
            }
        }

        #endregion

        #region PreSounds
        public static void JumpSound(double beat)
        {
            _ = MultiSound.Play(new MultiSound.Sound[]
            {
                new("penguisionists/kewk4", beat)
            }, forcePlay: true);
        }

        public static void TogetherJumpSound(double beat)
        {
            _ = MultiSound.Play(new MultiSound.Sound[]
            {
                new("penguisionists/kewk1", beat),
                new("penguisionists/kewk2", beat + 0.5),
                new("penguisionists/kewk3", beat + 1.5)
            }, forcePlay: true);
        }

        public static void WhistleJumpSound(double beat)
        {
            _ = MultiSound.Play(new MultiSound.Sound[]
            {
                new("penguisionists/whistle", beat)
            }, forcePlay: true);
        }

        #endregion
    }
}