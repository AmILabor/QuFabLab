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