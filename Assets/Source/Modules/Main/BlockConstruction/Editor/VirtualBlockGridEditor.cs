using UnityEditor;
using UnityEngine;
using static PlasticPipe.PlasticProtocol.Messages.Serialization.ItemHandlerMessagesSerialization;

namespace IJunior.ArrowBlocks.Main.Editor
{
    [CustomEditor(typeof(VirtualBlockGrid))]
    public class VirtualBlockGridEditor : UnityEditor.Editor
    {
        private const int IndentationBeforeButtons = 10;

        private readonly GUILayoutOption WidthSetting = GUILayout.Width(160);

        private VirtualBlockGrid _virtualBlockGrid;

        private SerializedProperty _blocks;
        private SerializedProperty _cellSize;
        private SerializedProperty _blocksOffGrid;

        private void OnEnable()
        {
            _virtualBlockGrid = target as VirtualBlockGrid;

            _blocks = serializedObject.FindProperty(nameof(_blocks));
            _cellSize = serializedObject.FindProperty(nameof(_cellSize));
            _blocksOffGrid = serializedObject.FindProperty(nameof(_blocksOffGrid));

            if (Application.isPlaying)
                return;

            _virtualBlockGrid.SetBlocksFromChildElements();
            CheckBlocksPositions();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUI.enabled = Application.isPlaying == false;

            EditorGUILayout.PropertyField(_cellSize);

            if (Application.isPlaying)
                return;

            GUILayout.Space(IndentationBeforeButtons);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Check Blocks Positions", WidthSetting))
                CheckBlocksPositions();

            if (_blocksOffGrid.arraySize > 0)
            {
                if (GUILayout.Button("Fix All Blocks Positions", WidthSetting))
                    _virtualBlockGrid.FixAllBlocksOffGrid();
            }

            GUILayout.EndHorizontal();

            GUILayout.TextArea(GetBlocksInfoString(), WidthSetting);
        }

        private string GetBlocksInfoString()
        {
            int numberOfCorrectBlocks = _blocks.arraySize - _blocksOffGrid.arraySize;
            return $"Correct Blocks: {numberOfCorrectBlocks}/{_blocks.arraySize}";
        }

        private void CheckBlocksPositions()
        {
            _virtualBlockGrid.SetBlocksOffGrid();
        }
    }
}