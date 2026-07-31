/// <summary>
/// Enthält die Aufzählung aller verfügbaren QuBrick-Typen (BeamSplitter, Mirror45, Mirror90, Periskope usw.).
/// </summary>
using System;

namespace QuantenKoffer.Bricks
{
    /// <summary>
    /// All QuBrickType as enum
    /// </summary> 
    [Serializable]
    public enum QuBrickTypeEnum
    {
        BeamSplitter,
        Mirror90,
        Mirror45,
        Periscope,
        GlassWedge,
        Polarizer,
        TunnelEffect,
        Camera,
        DoubleSlit,
        FibreCoupler,
        Waveplate,
        ArrayDetector,
        BucketDetector,
        Pump,
        SPDC
    }
}