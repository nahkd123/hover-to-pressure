using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;
namespace nahkd123.HoverToPressure
{
    [PluginName("Hover to pressure")]
    public class HoverToPressureFilter : IPositionedPipelineElement<IDeviceReport>
    {
        [Property("Minimum hover distance"), DefaultPropertyValue(0u), ToolTip("Check this value in Tablet Debugger")]
        public uint MinimumHoverDistance { get; set; } = 0u;
        [Property("Maximum hover distance"), DefaultPropertyValue(63u), ToolTip("Check this value in Tablet Debugger")]
        public uint MaximumHoverDistance { get; set; } = 63u;
        [Property("Activation mode"), DefaultPropertyValue(nameof(ActivationMode.Always)), ToolTip("How the hover to pressure should be activated"), PropertyValidated(nameof(ValidActivationModes))]
        public string ActivationModeStr { get; set; } = nameof(ActivationMode.Always);
        public ActivationMode ActivationMode => Enum.TryParse(ActivationModeStr, out ActivationMode result) ? result : ActivationMode.Always;
        public static IEnumerable<string> ValidActivationModes => validActivationModes ??= from mode in Enum.GetValues<ActivationMode>() select Enum.GetName(mode)!;
        private static IEnumerable<string>? validActivationModes = null;
        [Property("Activation button index"), DefaultPropertyValue(1u), ToolTip("The index of button to activate hover to pressure, starting from 1")]
        public uint ActivationButtonIndex { get; set; } = 1u;
        [Property("Smoothing weight"), DefaultPropertyValue(0f), ToolTip("The weight of previous hover distance value")]
        public float SmoothingWeight { get; set; } = 0f;

        public PipelinePosition Position => PipelinePosition.Raw;
        public event Action<IDeviceReport>? Emit;
        public bool Activated => ActivationMode == ActivationMode.Always || activate;
        private bool activate = false;
        private float lastDistance = 0f;
        private bool lastActivated = false;

        [TabletReference]
        public TabletReference? Tablet { get; set; } = null;

        public void Consume(IDeviceReport value)
        {
            #region Activation
            if (value is IAuxReport auxReport && ActivationMode == ActivationMode.AuxButton)
            {
                activate = ActivationButtonIndex >= 1 && ActivationButtonIndex <= auxReport.AuxButtons.Length && auxReport.AuxButtons[ActivationButtonIndex - 1];
            }
            if (value is ITabletReport tabletReport && ActivationMode == ActivationMode.PenButton)
            {
                activate = ActivationButtonIndex >= 1 && ActivationButtonIndex <= tabletReport.PenButtons.Length && tabletReport.PenButtons[ActivationButtonIndex - 1];
            }
            #endregion

            if (Tablet != null && Activated && value is ITabletReport tabletReport1 && value is IProximityReport proximityReport && (proximityReport.HoverDistance != 0 || proximityReport.NearProximity))
            {
                uint maxPressure = Tablet.Properties.Specifications.Pen.MaxPressure;
                float hoverProgress = MathF.Max(1f - (proximityReport.HoverDistance - MinimumHoverDistance) / (float)(MaximumHoverDistance - MinimumHoverDistance), 0f);
                hoverProgress = lastDistance = (lastDistance * SmoothingWeight + hoverProgress) / (SmoothingWeight + 1f);
                uint newPressure = Math.Clamp((uint)(hoverProgress * maxPressure), 0u, maxPressure);
                tabletReport1.Pressure = newPressure;
                lastActivated = true;
            }

            Emit?.Invoke(value);
        }
    }
}
