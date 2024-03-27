using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VirtualBlockGrid))]
public class VirtualBlockGridEditor : Editor
{
    private VirtualBlockGrid _virtualBlockGrid;
    private SerializedProperty _blocks;
    private SerializedProperty _blocksOffGrid;

    private int _childCountSaved = 0;
    private bool _isBloksCheked = false;

    private void OnEnable()
    {
        _virtualBlockGrid = target as VirtualBlockGrid;

        _blocks = serializedObject.FindProperty(nameof(_blocks));
        _blocksOffGrid = serializedObject.FindProperty(nameof(_blocksOffGrid));
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        int childCount = _virtualBlockGrid.transform.childCount;

        if (_childCountSaved != childCount)
        {
            _virtualBlockGrid.SetBlocksFromChild();
            _childCountSaved = childCount;
            _isBloksCheked = false;
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Check Blocks Positions", GUILayout.Width(160)))
        {
            _virtualBlockGrid.SetBlocksOffGrid();
            _isBloksCheked = true;
        }

        if (_isBloksCheked)
        {
            if (GUILayout.Button("Fix All Blocks Positions", GUILayout.Width(160)))
            {
                _virtualBlockGrid.FixAllBlocksOffGrid();
                _virtualBlockGrid.SetBlocksOffGrid();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.TextArea(GetBlocksCountString(), GUILayout.Width(160));
    }

    private string GetBlocksCountString()
    {
        int numberOfCorrectBlocks = _blocks.arraySize - _blocksOffGrid.arraySize;
        return _isBloksCheked ? $"Correct Blocks: {numberOfCorrectBlocks}/{_blocks.arraySize}" : "Unknown";
    }
}