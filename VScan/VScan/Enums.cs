/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

namespace PremierTaxFree
{
    public enum eRibbonMarkupCommands : uint
    {
        //Groups
        cmdTabHome = 1001,
        cmdGroupMain = 1002,
        cmdTabGroupDesign = 1003,
        cmdTabEdit = 1004,
        cmdGroupInfo = 1005,
        cmdGroupDesign = 1006,
        cmdGroupExport = 1007,
        cmdTabTools = 1008,
        //cmdGroupTools = 1009,

        //Buttons
        cmdButtonHome = 1038,
        cmdButtonSelectDesign = 1039,
        cmdButtonTools = 1037,

        cmdButtonSelect = 1040,
        cmdButtonResize = 1041,
        cmdButtonRotate = 1042,
        cmdButtonSave2 = 1043,
        cmdButtonExport = 1044,
        cmdButtonPrint = 1045,
        cmdButtonScan = 1046,
        cmdButtonScissors = 1047,

        //Context menu
        cmdButtonNew = 1051,
        cmdButtonOpen = 1052,
        cmdButtonSave = 1053,
        cmdButtonExit = 1054,
        cmdContextMap = 1055,
        cmdFontControl = 1056,
        cmdDropDownColorPicker = 1057,
        cmdButtonSaveAs = 1058,


        //Gallery
        cmdGroupSize = 1061,
        cmdGallerySize = 1062,
        cmdCommandSaveSize = 1063,

        cmdGroupBrushes = 1064,
        cmdGalleryBrushes = 1065,

        cmdGroupShapes = 1066,
        cmdGalleryShapes = 1067,
        cmdGroupTools = 1068,
        cmdGalleryTools = 1069,

        cmdButtonInfo = 1080,
    }

    public enum eBrushes : uint
    {
        Brush1 = 2000,
        Brush2 = 2001,
        Brush3 = 2002,
        Brush4 = 2003,
        Brush5 = 2004,
        Brush6 = 2005,
        Brush7 = 2006,
        Brush8 = 2007,
        Brush9 = 2008,
    }

    public enum eTools : uint
    {
        Scissors = 2050,
        ToolMin = 2050,
        Pen = 2051,
        PenPolio = 2052,
        Brush = 2053,
        Buscket = 2054,
        Pippete = 2055,
        Text = 2056,
        Eraser = 2057,
        Rectangle = 2058,
        Circle = 2059,
        ToolMax = 2059,
    }

    public enum eSize : uint
    {
        s1,
        s2,
        s3,
        s4,
        s5,
        s6,
        s7,
        s8,
        s9,
    }
}
