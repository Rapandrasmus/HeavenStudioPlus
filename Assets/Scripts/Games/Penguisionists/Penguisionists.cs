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
    public class Penguisionists : Minigame
    {
        public enum Penguin
        {
            First,
            Second,
            Third,
        }

        #region Jumps

        public void Jump(double beat, double startBeat)
        {
            if (beat + 1 >= startBeat) _ = ScheduleInput(beat, 1, InputAction_BasicPress, Just1, Miss1, Empty);
            if (beat + 2 >= startBeat) _ = ScheduleInput(beat, 2, InputAction_BasicPress, Just2, Miss2, Empty);
            if (beat + 3 >= startBeat) _ = ScheduleInput(beat, 3, InputAction_BasicPress, Just3, Miss3, Empty);
        }

        public void TogetherJump(double beat, double startBeat)
        {
            if (beat + 2 >= startBeat) _ = ScheduleInput(beat, 2, InputAction_BasicPress, JustTogether, MissTogether, Empty);
        }

        public void WhistleJump(double beat, double startBeat)
        {
            if (beat + 1 >= startBeat) _ = ScheduleInput(beat, 1, InputAction_BasicPress, Just1, Miss1, Empty);
            if (beat + 3 >= startBeat) _ = ScheduleInput(beat, 3, InputAction_BasicPress, Just2, Miss2, Empty);
            if (beat + 4 >= startBeat) _ = ScheduleInput(beat, 4, InputAction_BasicPress, Just3, Miss3, Empty);
        }

        #endregion

        #region Setup

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

        #region Inputs

        public void Just(PlayerActionEvent caller, float state, Penguin penguin)
        {
            if (state >= 1f || state <= -1f)
            {
                return;
            }
            SoundByte.PlayOneShotGame("penguisionists/penguinjump");
        }

        public void Just1(PlayerActionEvent caller, float state)
        {
            Just(caller, state, Penguin.First);
        }

        public void Just2(PlayerActionEvent caller, float state)
        {
            Just(caller, state, Penguin.Second);
        }

        public void Just3(PlayerActionEvent caller, float state)
        {
            Just(caller, state, Penguin.Third);
        }

        public void JustTogether(PlayerActionEvent caller, float state)
        {
            if (state >= 1f || state <= -1f)
            {
                return;
            }
            SoundByte.PlayOneShotGame("penguisionists/penguinsjump");
        }

        public void Miss(PlayerActionEvent caller, Penguin penguin)
        {

        }

        public void Miss1(PlayerActionEvent caller)
        {
            Miss(caller, Penguin.First);
        }

        public void Miss2(PlayerActionEvent caller)
        {
            Miss(caller, Penguin.Second);
        }

        public void Miss3(PlayerActionEvent caller)
        {
            Miss(caller, Penguin.Third);
        }

        public void MissTogether(PlayerActionEvent caller)
        {

        }

        public void Empty(PlayerActionEvent caller) { }

        #endregion

        #region Sounds
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