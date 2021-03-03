using UnityEngine;

namespace SK.Examples
{
    public sealed class Hand : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _speed = 10;

        private float _flexTarget;
        private float _pinchTarget;
        private float _flexCurrent;
        private float _pinchCurrent;

        private int _animatorFlexHash;
        private int _animatorPinchHash;

        private void Awake()
        {
            _animatorFlexHash  = Animator.StringToHash("Flex");
            _animatorPinchHash = Animator.StringToHash("Pinch");
        }

        private void Update() => AnimateHand();

        public void SetFlex(float value) => _flexTarget = value;

        public void SetPinch(float value) => _pinchTarget = value;

        private void AnimateHand()
        {
            if (_flexCurrent != _flexTarget)
            {
                _flexCurrent = Mathf.MoveTowards(_flexCurrent, _flexTarget, Time.deltaTime * _speed);
                _animator.SetFloat(_animatorFlexHash, _flexCurrent);
            }

            if (_pinchCurrent != _pinchTarget)
            {
                _pinchCurrent = Mathf.MoveTowards(_pinchCurrent, _pinchTarget, Time.deltaTime * _speed);
                _animator.SetFloat(_animatorPinchHash, _pinchCurrent);
            }
        }
    }
}
