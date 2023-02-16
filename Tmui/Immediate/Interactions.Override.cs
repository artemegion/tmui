using Tmui.Core;

namespace Tmui.Immediate;

public partial class Interactions
{
    /// <summary>
    /// Override interaction state over a provided area, optionally allowing certain controls to ignore the override.
    /// </summary>
    public readonly struct Override
    {
        public Override(Interaction interaction, Rect? area = null, int[]? exceptionControlId = null)
        {
            Interaction = interaction;
            Area = area;
            ExceptionControlId = exceptionControlId;
        }

        /// <summary>
        /// The interaction state to be reported to controls instead of actual interaction state.
        /// </summary>
        public readonly Interaction Interaction;

        /// <summary>
        /// The area over which this override takes place. If null, the override is used for the whole window.
        /// </summary>
        public readonly Rect? Area;

        /// <summary>
        /// Ids of controls that can ignore the override.
        /// </summary>
        public readonly int[]? ExceptionControlId;

        /// <summary>
        /// Tests if a control with id of <paramref name="controlId"/> can ignore the override.
        /// </summary>
        /// <param name="controlId">Id of control to test.</param>
        /// <returns>True if the control can ignore the override, false otherwise.</returns>
        public bool CanIgnoreOverride(int controlId)
        {
            if (ExceptionControlId == null) return false;

            for (int i = 0, len = ExceptionControlId.Length; i < len; i++)
                if (ExceptionControlId[i] == controlId) return true;

            return false;
        }
    }
}
