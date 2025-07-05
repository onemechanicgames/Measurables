using System.Collections.Generic;
using System.Linq;
using OMG.Extensions.VisualElementExtensions.Runtime;
using OMG.Measurables.Core.Runtime;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;
using Button = Unity.AppUI.UI.Button;

namespace OMG.Measurables.Executor.Runtime
{
    //Uncomment to create another MeasurableUI asset
    //[CreateAssetMenu(fileName = "MeasurableUI", menuName = "MeasureExecutor/Create/MeasurableUI", order = 1)]
    public class MeasurableUI : ScriptableObject
    {
        [SerializeField] private VisualTreeAsset _uiAsset;
        [SerializeField] private MeasurableItemUI _measurableItemUi;

        private ListView _listView;
        private VisualElement _controlsContainer;
        private VisualElement _iterableContainer;
        private Button _addIterableButton;
        private Button _runButton;
        private Text _statusLabel;

        private List<IMeasurableGroup> _allMeasurableGroups;
        private IMeasurableGroup _selectedMeasurableGroup;
        private MeasureExecutor _measureExecutor;

        private List<IntField> _displayedIntegerFields = new();


        ~MeasurableUI() {
            if (_runButton != null) _runButton.clicked -= Measure;
            if (_addIterableButton != null) _addIterableButton.clicked -= AddIterable;
            if (_listView != null) _listView.selectedIndicesChanged -= OnSelectedIndicesChanged;
        }


        public MeasurableUI Initialize(MeasureExecutor measureExecutor, VisualElement container) {
            _measureExecutor = measureExecutor;

            var templateContainer = _uiAsset.Instantiate();
            templateContainer.pickingMode = PickingMode.Ignore;
            container.Add(templateContainer);

            var uiInstance = templateContainer.Q<VisualElement>("measurable-ui");

            _listView = uiInstance.Q<ListView>("measurables-list");
            _listView.selectionType = SelectionType.Single;
            _listView.selectedIndicesChanged += OnSelectedIndicesChanged;

            _controlsContainer = uiInstance.Q<VisualElement>("controls-container");
            _controlsContainer.DisplayNot();

            _iterableContainer = _controlsContainer.Q<VisualElement>("iterable-container");
            _addIterableButton = _iterableContainer.Q<Button>("add-iteration-button");
            _addIterableButton.clicked += AddIterable;

            _runButton = _controlsContainer.Q<Button>("run-button");
            _runButton.clicked += Measure;

            _statusLabel = uiInstance.Q<Text>("status-label");
            _statusLabel.DisplayNot();

            return this;
        }


        private void OnSelectedIndicesChanged(IEnumerable<int> selectedIndices) {
            foreach (var integerField in _displayedIntegerFields)
                _iterableContainer.Remove(integerField);

            _displayedIntegerFields.Clear();

            var selectedIndexArray = selectedIndices as int[] ?? selectedIndices.ToArray();
            if (!selectedIndexArray.Any()) {
                _controlsContainer.DisplayNot();
                return;
            }

            //Since only 1 list item can be selected, we will only have 1 item in the selected indices list
            var selectedIndex = selectedIndexArray.First();
            _selectedMeasurableGroup = _allMeasurableGroups[selectedIndex];

            if (_measureExecutor.IsIterable(_selectedMeasurableGroup)) {
                var defaults = _measureExecutor.GetDefaults(_selectedMeasurableGroup);
                var defaultValues = defaults?.DefaultValues ?? new int[] { 0 };
                for (var i = 0; i < defaultValues.Length; i++) {
                    var defaultValue = defaultValues[i];
                    var integerField = new IntField();
                    integerField.value = defaultValue;
                    _iterableContainer.Add(integerField);
                    _displayedIntegerFields.Add(integerField);
                }
            }

            SelectForExecution();
        }


        private void SelectForExecution() {
            _controlsContainer.Display();
            var isIterable = _measureExecutor.IsIterable(_selectedMeasurableGroup);
            _iterableContainer.Display(isIterable);
        }


        private void AddIterable() {
            var integerField = new IntField();
            integerField.value = 0;
            _iterableContainer.Add(integerField);
            _displayedIntegerFields.Add(integerField);
        }


        private void Measure() {
            if (_measureExecutor.IsIterable(_selectedMeasurableGroup)) {
                var iterations = new List<int>();

                foreach (var integerField in _displayedIntegerFields)
                    if (integerField.value > 0)
                        iterations.Add(integerField.value);

                _ = _measureExecutor.Execute(_selectedMeasurableGroup, iterations.ToArray());
            }
            else {
                _ = _measureExecutor.Execute(_selectedMeasurableGroup);
            }
        }


        internal void Load(List<IMeasurableGroup> allMeasurableGroups) {
            _allMeasurableGroups = allMeasurableGroups;

            _listView.itemsSource = _allMeasurableGroups;
            _listView.makeItem = MakeListItem;
            _listView.bindItem = BindItem;
        }


        private VisualElement MakeListItem() {
            var itemUI = Instantiate<MeasurableItemUI>(_measurableItemUi);
            itemUI.Initialize(_measureExecutor);
            itemUI.Content.userData = itemUI;
            return itemUI.Content;
        }


        private void BindItem(VisualElement visualElement, int index) {
            var itemUi = (MeasurableItemUI)visualElement.userData;
            itemUi.Load(_allMeasurableGroups[index]);
        }


        internal void Notify(string message) {
            _statusLabel.Display();
            _statusLabel.text = message;
        }


        internal void ShowContent(bool show) {
            _listView.Display(show);
            _controlsContainer.Display(show);
        }
    }
}