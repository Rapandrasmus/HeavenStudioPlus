using HeavenStudio.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio.Games.Scripts_Penguisionists
{
    public class Penguin : MonoBehaviour
    {
        [SerializeField] private bool _playTogether = false;
        [SerializeField][Range(-1, 1)] private float panning = 0;
        [SerializeField][Range(0.1f, 5)] private float volume = 1;

        [SerializeField] private Animator _anim;

        private Penguisionists _game;
        private Queue<Penguisionists.PenguinSound> _queuedSoundToPlay = new();

        public void Init(Penguisionists game)
        {
            _game = game;
        }

        public void Walk(bool right)
        {
            _anim.DoScaledAnimationAsync(right ? "Right" : "Left", 0.75f);
        }

        public void QueueJump(double beat, double startBeat, Penguisionists.PenguinSound sound, bool together = false)
        {
            if (beat >= startBeat)
            {
                if (!together) _queuedSoundToPlay.Enqueue(sound);
                _ = _game.ScheduleInput(beat - 1, 1, Minigame.InputAction_BasicPress, together ? JumpTogether : Jump, Miss, Empty);
            }
        }

        public void Jump(PlayerActionEvent caller, float state)
        {
            Penguisionists.PenguinSound sound = _queuedSoundToPlay.Dequeue();
            if (state >= 1f || state <= -1f)
            {
                return;
            }

            string soundString = sound switch
            {
                Penguisionists.PenguinSound.One => "CrashWithPenguin",
                Penguisionists.PenguinSound.Two => "CrashWithPenguin2",
                Penguisionists.PenguinSound.Three => "CrashWithPenguin3",
                _ => throw new System.NotImplementedException(),
            };

            SoundByte.PlayOneShotGame("penguisionists/" + soundString, pan: panning, volume: volume);
        }

        public void JumpTogether(PlayerActionEvent caller, float state)
        {
            if (state >= 1f || state <= -1f)
            {
                return;
            }
            if (_playTogether) SoundByte.PlayOneShotGame("penguisionists/penguinsjump");
        }

        public void Miss(PlayerActionEvent caller) { _ = _queuedSoundToPlay.Dequeue(); }

        public void Empty(PlayerActionEvent caller) { }
    }
}

