using OMG.Measurables.Core.Runtime;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace OMG.Measurables.Executor.Runtime
{
    [CreateAssetMenu(fileName = "MeasurableItemUI", menuName = "MeasureExecutor/MeasurableItemUI", order = 1)]
    public class MeasurableItemUI : ScriptableObject
    {
        public VisualElement Content => _uiInstance;

        [SerializeField] private VisualTreeAsset _uiAsset;

        private MeasureExecutor _measureExecutor;

        private VisualElement _uiInstance;

        private Text _descriptionLabel;
        private Text _methodsCountLabel;
        private Text _isIterableLabel;

        private bool _isInitialized = false;


        public MeasurableItemUI Initialize(MeasureExecutor measureExecutor) {
            if (_isInitialized)
                return this;

            _measureExecutor = measureExecutor;

            var templateContainer = _uiAsset.Instantiate();

            _uiInstance = templateContainer.Q<VisualElement>("measurable-item-ui");

            _descriptionLabel = _uiInstance.Q<Text>("measurable-title");
            _methodsCountLabel = _uiInstance.Q<Text>("measurable-description");
            _isIterableLabel = _uiInstance.Q<Text>("iterable-value");

            _isInitialized = true;

            return this;
        }

        internal void Load(IMeasurableGroup measurableGroup) {
            _descriptionLabel.text = measurableGroup.Title;
            _methodsCountLabel.text = measurableGroup.Description;
            _isIterableLabel.text = _measureExecutor.IsIterable(measurableGroup).ToString();
        }
    }
}