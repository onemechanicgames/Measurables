using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OMG.Measurables.Core.Runtime;
using OMG.Measurables.MeasurableGroups.Runtime;
using OMG.Utilities.Monobehaviour.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace OMG.Measurables.Executor.Runtime
{
    public class MeasureExecutor : SingletonMonobehaviour<MeasureExecutor>
    {
        private const string kSceneExecutor = "ExecutorScene";

        [SerializeField] private UIDocument _mainUIDocument;
        [SerializeField] private MeasurableUI _ui;

        private List<IMeasurableGroup> _allMeasurableGroups = new();

        private Dictionary<IMeasurableGroup, bool> _measurableIterableMap = new();

        private void Start() {
            Application.targetFrameRate = 60;
            
            var allMeasurableGroupTypes = GetAllMeasurableGroupTypes();

            foreach (var type in allMeasurableGroupTypes) {
                var measurableGroup = (IMeasurableGroup)Activator.CreateInstance(type);
                _allMeasurableGroups.Add(measurableGroup);

                _measurableIterableMap[measurableGroup] = type.GetInterfaces().Contains(typeof(IIterable));
            }

            _ui.Initialize(this, _mainUIDocument.rootVisualElement);
            _ui.Load(_allMeasurableGroups);
        }


        private List<Type> GetAllMeasurableGroupTypes() {
            var type = typeof(IMeasurableGroup);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .ToList();
        }


        public bool IsIterable(IMeasurableGroup measurableGroup) {
            return _measurableIterableMap.GetValueOrDefault(measurableGroup);
        }


        public async Task Execute(IMeasurableGroup measurableGroup, params int[] iterationsList) {
            try {
                _ui.Notify($"Executing {measurableGroup.Title}");
                _ui.ShowContent(false);

                TaskCompletionSource<Task> tcs = new();

                _ = measurableGroup.Execute(tcs, iterationsList);
                await tcs.Task;

                _ui.ShowContent(true);
                _ui.Notify("Done");

                UnityEngine.SceneManagement.SceneManager.LoadScene(kSceneExecutor);
            }
            catch (Exception e) {
                Debug.LogError(e);
                throw;
            }
        }


        public IterableDefaults GetDefaults(IMeasurableGroup selectedMeasurableGroup) {
            if (!IsIterable(selectedMeasurableGroup))
                return null;

            var defaults = selectedMeasurableGroup.GetType().GetCustomAttribute<IterableDefaults>();
            if (defaults == null) return null;

            return defaults;
        }
    }
}