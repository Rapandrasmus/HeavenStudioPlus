using HeavenStudio.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio.Games.Scripts_Penguisionists
{
    public class Penguin : MonoBehaviour
    {
        [SerializeField] private bool _playTogether = false;
        private Penguisionists _game;

        public void Init(Penguisionists game)
        {
            _game = game;
        }

        public void QueueJump(double beat, double startBeat, bool together = false)
        {
            if (beat >= startBeat) _ = _game.ScheduleInput(beat - 1, 1, Minigame.InputAction_BasicPress, together ? JumpTogether : Jump, Miss, Empty);
        }

        public void Jump(PlayerActionEvent caller, float state)
        {
            if (state >= 1f || state <= -1f)
            {
                return;
            }
            SoundByte.PlayOneShotGame("penguisionists/penguinjump");
        }

        public void JumpTogether(PlayerActionEvent caller, float state)
        {
            if (state >= 1f || state <= -1f)
            {
                return;
            }
            if (_playTogether) SoundByte.PlayOneShotGame("penguisionists/penguinsjump");
        }

        public void Miss(PlayerActionEvent caller) { }

        public void Empty(PlayerActionEvent caller) { }
    }
}

