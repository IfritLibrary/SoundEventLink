using UnityEngine.UIElements;
namespace GraphProcessor
{
	public class ExposedParameterPropertyView : VisualElement
	{
		protected BaseGraphView baseGraphView;

		public ExposedParameterPropertyView(BaseGraphView graphView, ExposedParameter param)
		{
			baseGraphView = graphView;
			parameter     = param;

			var field = graphView.exposedParameterFactory.GetParameterSettingsField(param, newValue => {
				param.settings = newValue as ExposedParameter.Settings;
			});

			Add(field);
		}

		public ExposedParameter parameter { get; }

		public Toggle hideInInspector { get; private set; }
	}
}