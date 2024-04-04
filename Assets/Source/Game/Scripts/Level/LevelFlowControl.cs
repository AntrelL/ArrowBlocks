using System.Collections.Generic;
using IJunior.CompositeRoot;
using UnityEngine;

namespace IJunior.ArrowBlocks
{
    internal class LevelFlowControl : MonoBehaviour
    {
        private List<IRootUpdateble> _rootUpdatebleElements;
        private List<IRootFixedUpdateble> _rootFixedUpdatebleElements;

        private bool _isRunning = false;

        private void Update()
        {
            if (_isRunning == false)
                return;

            foreach (var rootUpdatebleElement in _rootUpdatebleElements)
            {
                rootUpdatebleElement.RootUpdate();
            }
        }

        private void FixedUpdate()
        {
            if (_isRunning == false)
                return;

            foreach (var rootFixedUpdatebleElement in _rootFixedUpdatebleElements)
            {
                rootFixedUpdatebleElement.RootFixedUpdate();
            }
        }

        public void Initialize(List<IRootUpdateble> rootUpdatebleElements,
            List<IRootFixedUpdateble> rootFixedUpdatebleElements)
        {
            _rootUpdatebleElements = rootUpdatebleElements;
            _rootFixedUpdatebleElements = rootFixedUpdatebleElements;
        }

        public void Run() => _isRunning = true;
        
        public void Stop() => _isRunning = false;
    }
}
