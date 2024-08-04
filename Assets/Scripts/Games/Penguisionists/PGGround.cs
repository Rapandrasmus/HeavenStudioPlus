using HeavenStudio.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio.Games.Scripts_Penguisionists
{
    public class PGGround : MonoBehaviour
    {
        [SerializeField] private float _groundSpeedConstant = 1f;
        [SerializeField] private float _groundDistanceInterpolate = 3.5f;
        [SerializeField] private int _groundAmountExtra = 6;
        [SerializeField] private List<Transform> _firstGrounds = new();
        [SerializeField] private List<Transform> _secondGrounds = new();

        private List<float> _groundDistances = new();
        private List<List<Transform>> _allGrounds = new();

        private void Awake()
        {
            for (int i = 0; i < _firstGrounds.Count; i++)
            {
                _allGrounds.Add(new()
                {
                    _firstGrounds[i],
                    _secondGrounds[i]
                });
                _groundDistances.Add(_allGrounds[i][1].localPosition.x - _allGrounds[i][0].localPosition.x);

                for (int j = 0; j < _groundAmountExtra; j++)
                {
                    Transform spawnedGround = Instantiate(_allGrounds[i][0], transform);
                    spawnedGround.localPosition = new Vector3(spawnedGround.localPosition.x + (_groundDistances[i] * (j + 2)), spawnedGround.position.y, spawnedGround.position.z);
                    _allGrounds[i].Add(spawnedGround);
                }
            }
        }

        public void MoveGroundConstant()
        {
            float moveConstant = Time.deltaTime * _groundSpeedConstant;

            DoForEveryGround(delegate (Transform ground, int x, int y) 
            {
                ground.localPosition -= new Vector3(moveConstant, 0);
            });
            CatchWrapAround();
        }

        public void MoveGroundInterpolate(double startBeat, double endBeat, float distanceMult, EasingFunction.Ease ease)
        {
            _ = StartCoroutine(MoveGroundInterpolateCo(startBeat, endBeat, distanceMult, ease));
        }

        private IEnumerator MoveGroundInterpolateCo(double startBeat, double endBeat, float distanceMult, EasingFunction.Ease ease)
        {
            var func = EasingFunction.GetEasingFunction(ease);
            float prog = 0f;
            List<List<float>> ogXs = new();
            for (int x = 0; x < _allGrounds.Count; x++)
            {
                ogXs.Add(new());
                for (int y = 0; y < _allGrounds[x].Count; y++)
                {
                    var ground = _allGrounds[x][y];
                    ogXs[x].Add(ground.localPosition.x);
                }
            }

            while (prog <= 1f)
            {
                prog = Conductor.instance.GetPositionFromBeat(startBeat, endBeat - startBeat);

                DoForEveryGround(delegate (Transform ground, int x, int y) 
                {
                    float newX = ogXs[x][y] - (_groundDistanceInterpolate * distanceMult);
                    ground.localPosition = new Vector3(func(ogXs[x][y], newX, prog), ground.localPosition.y, ground.localPosition.z);
                    if (ground.localPosition.x < -GetFullDistance(x) / 2)
                    {
                        ground.localPosition += new Vector3(GetFullDistance(x), 0);
                        ogXs[x][y] += GetFullDistance(x);
                    }
                });
                yield return null;
            }
        }

        private delegate void EveryGroundDelegate(Transform ground, int x, int y);

        private void DoForEveryGround(EveryGroundDelegate func)
        {
            for (int x = 0; x < _allGrounds.Count; x++)
            {
                for (int y = 0; y < _allGrounds[x].Count; y++)
                {
                    var ground = _allGrounds[x][y];
                    func(ground, x, y);
                }
            }
        }

        private void CatchWrapAround()
        {
            DoForEveryGround(delegate (Transform ground, int x, int y)
            {
                if (ground.localPosition.x < -GetFullDistance(x) / 2)
                {
                    ground.localPosition += new Vector3(GetFullDistance(x), 0);
                }
            });
        }

        private float GetFullDistance(int i)
        {
            return _groundDistances[i] * (_groundAmountExtra + 2);
        }
    }
}

