namespace Faoem.OpcUaConnector.Definitions;

public enum QualityCode
{
    /// <summary>
    /// The value is not useful for reasons indicated by the sub-status.
    /// </summary>
    Bad = 0,

    /// <summary>
    /// The quality of the value is less than normal, but the value may still be useful. The reason is indicated by the sub-status.
    /// </summary>
    Uncertain = 1,

    /// <summary>
    /// The quality of the value is less than normal, but the value may still be useful. The reason is indicated by the sub-status.
    /// </summary>
    GoodNonCascade = 2,

    /// <summary>
    /// The quality of the value is good and may be used in control.
    /// </summary>
    GoodCascade = 3
}