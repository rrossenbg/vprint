/* ****************************************************************************
*
* Copyright (c) Francesco Abbruzzese. All rights reserved.
* francesco@dotnet-programming.com
* http://www.dotnet-programming.com/
* 
* This software is subject to the the license at http://mvccontrolstoolkit.codeplex.com/license  
* and included in the license.txt file of this distribution.
* 
* You must not remove this notice, or any other, from this software.
*
* ***************************************************************************/

// DUAL SELECT

var DualSelect_Separator = ";;;";
var DualSelect_SelectAvial = "_AvialSelect";
var DualSelect_SelectSelected = "_SelSelect";
var DualSelect_HiddenSelectedItemsVal = "";



var DualSelect_TempObjSource, DualSelect_TempObjDestination;



function DualSelect_SetObjects(dualSelectId, bDoSelected) {
    if (bDoSelected) {
        DualSelect_TempObjSource =
			document.getElementById(dualSelectId + DualSelect_SelectAvial);
        DualSelect_TempObjDestination =
			document.getElementById(dualSelectId + DualSelect_SelectSelected);
    }
    else {
        DualSelect_TempObjSource =
			document.getElementById(dualSelectId + DualSelect_SelectSelected);
        DualSelect_TempObjDestination =
			document.getElementById(dualSelectId + DualSelect_SelectAvial);
    }
}



function DualSelect_GetIndexForInsert(oSelect, oNode) {
    if (oSelect.autosort == "false")
        return oSelect.length + 1;

    if (oSelect.length == 0) return 0;

    for (var i = 0; i < oSelect.length; i++)
        if (oSelect[i].text > oNode.text)
            return i;
    return oSelect.length;
}



function DualSelect_MoveElement(dualSelectId, bDoSelected) {
    DualSelect_SetObjects(dualSelectId, bDoSelected);

    if (DualSelect_TempObjSource.length == 0) return;

    iLast = 0;

    for (var i = 0; i < DualSelect_TempObjSource.length; i++) {
        if (DualSelect_TempObjSource[i].selected) {
            iLast = i;
            var oNode = document.createElement("Option");
            oNode.text = DualSelect_TempObjSource[i].text;
            oNode.value = DualSelect_TempObjSource[i].value;
            DualSelect_TempObjSource.remove(i);
            nPos = (DualSelect_TempObjDestination.length + 1);
            DualSelect_TempObjDestination.options.add(oNode,
				DualSelect_GetIndexForInsert(DualSelect_TempObjDestination, oNode));

            i--;
        }
    }

    DualSelect_SaveSelection(dualSelectId);

    if (DualSelect_TempObjSource.length > 0 && iLast == 0)
        DualSelect_TempObjSource.selectedIndex = 0;
    else if (DualSelect_TempObjSource.length - 1 >= iLast)
        DualSelect_TempObjSource.selectedIndex = iLast;
    else if (DualSelect_TempObjSource.length >= 1)
        DualSelect_TempObjSource.selectedIndex = iLast - 1;

    DualSelect_ClearSelection(DualSelect_TempObjSource);
    DualSelect_TempObjSource.focus;
}

function DualSelect_Move_Up(dualSelectId, bDoSelected) {
    DualSelect_SetObjects(dualSelectId, bDoSelected);

    if (DualSelect_TempObjSource.length == 0) return;
    if (DualSelect_TempObjSource[0].selected) return;
    for (var i = 1; i < DualSelect_TempObjSource.length; i++) {
        if (DualSelect_TempObjSource[i].selected) {

            var tempValue = DualSelect_TempObjSource[i].value;
            var tempText = DualSelect_TempObjSource[i].text;
            var tempSel = DualSelect_TempObjSource[i].selected;
            DualSelect_TempObjSource[i].value = DualSelect_TempObjSource[i - 1].value;
            DualSelect_TempObjSource[i].text = DualSelect_TempObjSource[i - 1].text;
            DualSelect_TempObjSource[i].selected = DualSelect_TempObjSource[i - 1].selected;
            DualSelect_TempObjSource[i - 1].value = tempValue;
            DualSelect_TempObjSource[i - 1].text = tempText;
            DualSelect_TempObjSource[i - 1].selected = tempSel;
            i--;
        }
    }

    DualSelect_SaveSelection(dualSelectId);
}

function DualSelect_Move_Down(dualSelectId, bDoSelected) {
    DualSelect_SetObjects(dualSelectId, bDoSelected);

    if (DualSelect_TempObjSource.length == 0) return;
    if (DualSelect_TempObjSource[DualSelect_TempObjSource.length - 1].selected) return;
    for (var i = DualSelect_TempObjSource.length-2; i > -1 ; i--) {
        if (DualSelect_TempObjSource[i].selected) {
            var tempValue = DualSelect_TempObjSource[i].value;
            var tempText = DualSelect_TempObjSource[i].text;
            var tempSel = DualSelect_TempObjSource[i].selected;
            DualSelect_TempObjSource[i].value = DualSelect_TempObjSource[i + 1].value;
            DualSelect_TempObjSource[i].text = DualSelect_TempObjSource[i + 1].text;
            DualSelect_TempObjSource[i].selected = DualSelect_TempObjSource[i + 1].selected;
            DualSelect_TempObjSource[i + 1].value = tempValue;
            DualSelect_TempObjSource[i + 1].text = tempText;
            DualSelect_TempObjSource[i + 1].selected = tempSel;
            i++;
        }
    }

    DualSelect_SaveSelection(dualSelectId);
}

function DualSelect_MoveAll(dualSelectId, bDoSelected) {
    DualSelect_SetObjects(dualSelectId, bDoSelected);

    while (DualSelect_TempObjSource.length > 0) {
        oNode = document.createElement("Option");
        oNode.text = DualSelect_TempObjSource[0].text;
        oNode.value = DualSelect_TempObjSource[0].value;

        DualSelect_TempObjSource.remove(DualSelect_TempObjSource[0]);
        DualSelect_TempObjDestination.options.add(oNode,
			DualSelect_GetIndexForInsert(DualSelect_TempObjDestination, oNode));
    }

    DualSelect_SaveSelection(dualSelectId);
}



function DualSelect_ClearSelection(oSelect) {
    for (var i = 0; i < oSelect.length; i++)
        oSelect[i].selected = false;
}



function DualSelect_SaveSelection(dualSelectId) {
    var oSelect = document.getElementById(
		dualSelectId + DualSelect_SelectSelected);
    var sValues = "";
    var sTexts = "";

    for (var i = 0; i < oSelect.length; i++) {
        sValues += oSelect[i].value + DualSelect_Separator;
        
    }

    document.getElementsByName(
		dualSelectId + DualSelect_HiddenSelectedItemsVal)[0].value = sValues;

}

// DATE AND TIME FUNCTIONS

var defaultYear = 1970 + 0;
var defaultMonth = 0+0;
var defaultDay = 1+0;
var defaultHour = 0 + 0;
var defaultMinute = 0 + 0;
var defaultSecond = 0 + 0;


function DateInput_Initialize(id) 
{
    if (document.getElementById(id + "_Year") != null) 
    {
        document.getElementById(id + "_Year").onkeypress = DateInputYearKeyVerify;
        document.getElementById(id + "_Year").onpaste = DateInputYearHandlePaste;
        document.getElementById(id + "_Year").ondrop = DateInputYearHandlePaste;
        document.getElementById(id + "_Year").onchange = DateInputChanged;
    }
    if (document.getElementById(id + "_Month") != null)
        document.getElementById(id + "_Month").onchange = DateInputChanged;

    if (document.getElementById(id + "_Day") != null)
        document.getElementById(id + "_Day").onchange = DateInputChanged;

    if (document.getElementById(id + "_Hours") != null)
        document.getElementById(id + "_Hours").onchange = DateInputChanged;

    if (document.getElementById(id + "_Minutes") != null)
        document.getElementById(id + "_Minutes").onchange = DateInputChanged;

    if (document.getElementById(id + "_Seconds") != null)
        document.getElementById(id + "_Seconds").onchange = DateInputChanged;

    

}

function DateInputGetNumDays(M, curYear) {
    M = M + 1;
    if (curYear % 4 == 0) {
        return (M == 9 || M == 4 || M == 6 || M == 11) ? 30 : (M == 2) ? 29 : 31;
    } else {
        return (M == 9 || M == 4 || M == 6 || M == 11) ? 30 : (M == 2) ? 28 : 31;
    }
}

function DateTimeAdjustYears(cmbInput, min, max) {
    if (cmbInput == null) return;
    var j = 0;
    if (min == cmbInput.options[0].value && max == cmbInput.options[cmbInput.options.length - 1].value) return;
    cmbInput.options.length = 0;
    for (i = min; i <= max; i++) {
        if (i < 10)
            cmbInput.options[j] = new Option("   " + i, i);
        else if (i < 100)
            cmbInput.options[j] = new Option("  " + i, i);
        else if (i < 1000)
            cmbInput.options[j] = new Option(" " + i, i);
        else
            cmbInput.options[j] = new Option("" + i, i);
        j++;
    }
}

function DateTimeAdjustMonthes(cmbInput, min, max) {
    if (cmbInput == null) return;
    var j = 0;
    if (min == cmbInput.options[0].value && max == cmbInput.options[cmbInput.options.length - 1].value) return;
    cmbInput.options.length = 0;
    for (i = min; i <= max; i++) {
        cmbInput.options[j] = new Option(DateTimeMonthes[i], i + 1);
        j++;
    }
}

function DateTimeAdjustDays(cmbInput, min, max) {
    if (cmbInput == null) return;
    var j = 0;
    if (min == cmbInput.options[0].value && max == cmbInput.options[cmbInput.options.length - 1].value) return;
    cmbInput.options.length = 0;
    for (i = min; i <= max; i++) {
        if (i < 10)
            cmbInput.options[j] = new Option(" " + i, i);
        else
            cmbInput.options[j] = new Option("" + i, i);
        j++;
    }
}
function DateTimeAdjustTimeElement(cmbInput, min, max) {
    if (cmbInput == null) return;
    var j = 0;
    if (min == cmbInput.options[0].value && max == cmbInput.options[cmbInput.options.length - 1].value) return;
    cmbInput.options.length = 0;
    for (i = min; i <= max; i++) {
        if (i < 10)
            cmbInput.options[j] = new Option("0" + i, i);
        else
            cmbInput.options[j] = new Option("" + i, i);
        j++;
    }
}

function DateInputYearHandlePaste(evt) {

    evt = (evt) ? (evt) : ((window.event) ? (window.event) : null);
    if (evt == null) return true;

    var sorg = (evt.target) ? (evt.target) : ((event.srcElement) ? (event.srcElement) : null);
    if (sorg == null) return true;

    var val;
    if (evt.type == "paste")
        val = window.clipboardData.getData("Text");
    else if (evt.type == "drop")
        val = evt.dataTransfer.getData("Text");
    else
        return true;


    for (i = 0; i < val.length; i++) {
        keyCode = val.charCodeAt(i);

        if (keyCode == 13 || keyCode == 8)
            continue;

        if ((keyCode >= 48) && (keyCode <= 57))
            continue;
        else
            return false;

    }
    sorg.value = val;
    return false;
}

function DateInputYearKeyVerify(evt) {
    evt = (evt) ? (evt) : ((window.event) ? (window.event) : null);
    if (evt == null) return true;

    var sorg = (evt.target) ? (evt.target) : ((event.srcElement) ? (event.srcElement) : null);
    if (sorg == null) return true;

    var keyCode = ((evt.charCode || evt.initEvent) ? evt.charCode : ((evt.which) ? evt.which : evt.keyCode));


    if (keyCode == 0 || keyCode == 13 || keyCode == 8)
        return true;
    if ((keyCode >= 48) && (keyCode <= 57))
        return true;
    return false;
    var val = sorg.value;
}

function DateInputChanged(evt, cid, update) {

    var clientID;
    if (cid == null) {


        evt = (evt) ? (evt) : ((window.event) ? (window.event) : null);
        if (evt == null) {

            return false;
        }

        var sorg = (evt.target) ? (evt.target) : ((event.srcElement) ? (event.srcElement) : null);
        if (sorg == null) {

            return false;
        }
        clientID = sorg.id.substring(0, sorg.id.lastIndexOf("_"));
    }
    else {
        clientID = cid;
    }
    if (eval(clientID + "Recursive") == true) return;
    eval(clientID + "Recursive = true;");


    var Nanno;
    var Nmese;
    var Ngiorno;
    var NHours;
    var NMinutes;
    var NSeconds;
    var CurrDate = eval(clientID + "_Curr");
    var CurrDay = CurrDate.getDate();
    var CurrMonth = CurrDate.getMonth();
    var CurrYear = CurrDate.getFullYear();
    var CurrHours = CurrDate.getHours();
    var CurrMinutes = CurrDate.getMinutes();
    var CurrSeconds = CurrDate.getSeconds();

    var currMin = eval(clientID + "_MinDate");
    var currMax = eval(clientID + "_MaxDate");

    var dynamicMin = null;
    if (eval("(typeof " + clientID + "_ClientDynamicMin !== 'undefined') && (" + clientID + "_ClientDynamicMin != null)") == true) dynamicMin = eval(clientID + "_ClientDynamicMin()");

    var dynamicMax = null;
    if (eval("(typeof " + clientID + "_ClientDynamicMax !== 'undefined') && (" + clientID + "_ClientDynamicMax != null)") == true) dynamicMax = eval(clientID + "_ClientDynamicMax()");

    if (dynamicMin != null && (currMin == null || dynamicMin > currMin)) {
        if (dynamicMin > currMax)
            currMin = currMax;
        else
            currMin = dynamicMin;
    }
    if (dynamicMax != null && (currMax == null || dynamicMax < currMax)) {
        if (dynamicMax < currMin)
            currMax = currMin;
        else
            currMax = dynamicMax;
    }

    if (document.getElementById(clientID + "_Year") != null) {
        Nanno = document.getElementById(clientID + "_Year").value;
    }
    else {
        Nanno = CurrYear;
    }

    if (document.getElementById(clientID + "_Month") != null)
        Nmese = document.getElementById(clientID + "_Month").value;
    else
        Nmese = CurrMonth;

    if (document.getElementById(clientID + "_Day") != null)
        Ngiorno = document.getElementById(clientID + "_Day").value;
    else
        Ngiorno = CurrDay;

    if (document.getElementById(clientID + "_Hours") != null)
        NHours = document.getElementById(clientID + "_Hours").value;
    else
        NHours = CurrHours;

    if (document.getElementById(clientID + "_Minutes") != null)
        NMinutes = document.getElementById(clientID + "_Minutes").value;
    else
        NMinutes = CurrMinutes;

    if (document.getElementById(clientID + "_Seconds") != null)
        NSeconds = document.getElementById(clientID + "_Seconds").value;
    else
        NSeconds = CurrSeconds;

    var TempNewDate = new Date(Nanno, Nmese-1, Ngiorno, NHours, NMinutes, NSeconds);

    if (currMax != null && currMax < TempNewDate) TempNewDate = currMax;
    if (currMin != null && currMin > TempNewDate) TempNewDate = currMin;

    Nanno = TempNewDate.getFullYear()+"";
    Nmese = (TempNewDate.getMonth()+1)+"";
    Ngiorno = TempNewDate.getDate()+"";
    NHours = TempNewDate.getHours()+"";
    NMinutes = TempNewDate.getMinutes()+"";
    NSeconds = TempNewDate.getSeconds()+"";

    var NewYear;
    var NewMonth;
    var NewDay;
    var NewHours;
    var NewMinutes;
    var NewSeconds;
    var MaxYear;
    var MinYear;
    var MaxMonth;
    var MinMonth;
    var MinDay;
    var MaxDay;
    var MinHours;
    var MaxHours;
    var MinMinutes;
    var MaxMinutes;
    var MinSeconds;
    var MaxSeconds;
    eval(clientID + "_Valid = true");

    
    //year processing
    NewYear = parseInt(Nanno);
    if (!isNaN(NewYear)) {
        if  (currMax == null) {
            MaxYear = null;
        }
        else {
            MaxYear = currMax.getFullYear();
        }
        if (currMin == null) {
            MinYear = null;
        }
        else {
            MinYear = currMin.getFullYear();
        }
        if (MaxYear != null && MaxYear < NewYear) NewYear = MaxYear;
        if (MinYear != null && MinYear > NewYear) NewYear = MinYear;
        if (document.getElementById(clientID + "_Year") != null && !eval(clientID + "_DateHidden") && eval(clientID + "_YearCombo"))
            DateTimeAdjustYears(document.getElementById(clientID + "_Year"), MinYear, MaxYear);
        
        if ((MaxYear == null || MaxYear >= NewYear) && (MinYear == null || MinYear <= NewYear)) {

            //Month Processing
            MaxMonth = 11;
            MinMonth = 0;
            if (MaxYear == NewYear) {
                MaxMonth = currMax.getMonth();
            }
            if (MinYear == NewYear) {
                MinMonth = currMin.getMonth();
            }
            NewMonth = parseInt(Nmese);
            if (!isNaN(NewMonth)) {
                NewMonth = NewMonth - 1;
                if (MinMonth > NewMonth) {
                    NewMonth = MinMonth;
                }
                if (MaxMonth < NewMonth) {
                    NewMonth = MaxMonth;
                }
                if (CurrYear == MinYear || CurrYear == MaxYear || NewYear == MinYear || NewYear == MaxYear)
                    if (document.getElementById(clientID + "_Month") != null && !eval(clientID + "_DateHidden"))
                        DateTimeAdjustMonthes(document.getElementById(clientID + "_Month"), MinMonth, MaxMonth);
                //day processing
                MinDay = 1;
                MaxDay = DateInputGetNumDays(NewMonth, NewYear);
                if (MaxYear == NewYear && MaxMonth == NewMonth) {
                    MaxDay = currMax.getDate();

                }
                if (MinYear == NewYear && MinMonth == NewMonth) {
                    MinDay = currMin.getDate();
                }
                NewDay = parseInt(Ngiorno);
                if (!isNaN(NewDay)) {
                    if (MinDay > NewDay) {
                        NewDay = MinDay;
                    }
                    if (MaxDay < NewDay) {
                        NewDay = MaxDay;
                    }
                    if (document.getElementById(clientID + "_Day") != null && !eval(clientID + "_DateHidden"))
                        DateTimeAdjustDays(document.getElementById(clientID + "_Day"), MinDay, MaxDay);
                    // Hours Processing
                    MinHours = 0;
                    MaxHours = 23;
                    if (MaxYear == NewYear && MaxMonth == NewMonth && NewDay == MaxDay) {
                        MaxHours = currMax.getHours();
                    }
                    if (MinYear == NewYear && MinMonth == NewMonth && NewDay == MinDay) {
                        MinHours = currMin.getHours();
                    }
                    NewHours = parseInt(NHours);
                    if (!isNaN(NewHours)) {
                        if (MaxHours < NewHours) NewHours = MaxHours;
                        if (NewHours < MinHours) NewHours = MinHours;
                        if (document.getElementById(clientID + "_Hours") != null)
                            DateTimeAdjustTimeElement(document.getElementById(clientID + "_Hours"), MinHours, MaxHours);
                        // Minutes Processing
                        MinMinutes = 0;
                        MaxMinutes = 59;
                        if (MaxYear == NewYear && MaxMonth == NewMonth && NewDay == MaxDay && MaxHours == NewHours)
                            MaxMinutes = currMax.getMinutes();
                        if (MinYear == NewYear && MinMonth == NewMonth && NewDay == MinDay && MinHours == NewHours)
                            MinMinutes = currMin.getMinutes();
                        NewMinutes = parseInt(NMinutes);
                        if (!isNaN(NewMinutes)) {
                            if (MaxMinutes < NewMinutes) NewMinutes = MaxMinutes;
                            if (MinMinutes > NewMinutes) NewMinutes = MinMinutes;
                            if (document.getElementById(clientID + "_Minutes") != null)
                                DateTimeAdjustTimeElement(document.getElementById(clientID + "_Minutes"), MinMinutes, MaxMinutes);
                            // Seconds Processing
                            MinSeconds = 0;
                            MaxSeconds = 59;
                            if (MaxYear == NewYear && MaxMonth == NewMonth && NewDay == MaxDay && MaxHours == NewHours && MaxMinutes == NewMinutes)
                                MaxSeconds = currMax.getSeconds();
                            if (MinYear == NewYear && MinMonth == NewMonth && NewDay == MinDay && MinHours == NewHours && MinMinutes == NewMinutes)
                                MinSeconds = currMin.getSeconds();
                            NewSeconds = parseInt(NSeconds);
                            if (!isNaN(NewSeconds)) {
                                if (MaxSeconds < NewSeconds) NewSeconds = MaxSeconds;
                                if (NewSeconds < MinSeconds) NewSeconds = MinSeconds;
                                if (document.getElementById(clientID + "_Seconds") != null)
                                    DateTimeAdjustTimeElement(document.getElementById(clientID + "_Seconds"), MinSeconds, MaxSeconds);
                            }
                            else {
                                eval(clientID + "_Valid = false");
                            }
                        }

                        else {
                            eval(clientID + "_Valid = false");
                        }
                    }
                    else {
                        eval(clientID + "_Valid = false");
                    }
                }
                else {
                    eval(clientID + "_Valid = false");
                }
            }
            else {
                eval(clientID + "_Valid = false");
            }
        }
    }
    else {
        eval(clientID + "_Valid = false");
    }
    var AChange = false;
    if (eval(clientID + "_Valid")) {
        if (update == true || (cid == null  && 
            (CurrYear != NewYear || CurrMonth != NewMonth || CurrDay != NewDay ||
             CurrHours != NewHours || CurrMinutes != NewMinutes || CurrSeconds != NewSeconds))) 
               AChange = true;
        CurrYear = NewYear;
        CurrMonth = NewMonth;
        CurrDay = NewDay;
        CurrHours = NewHours;
        CurrMinutes = NewMinutes;
        CurrSeconds = NewSeconds;
    }
    if (!AChange) {
        eval(clientID + "Recursive = false;")
        return true;
    }

    eval(clientID + "_Curr = new Date(CurrYear, CurrMonth, CurrDay, CurrHours, CurrMinutes, CurrSeconds)");

    if (document.getElementById(clientID + "_Year") != null) {
        document.getElementById(clientID + "_Year").value = CurrYear;
    }
    if (document.getElementById(clientID + "_Month") != null) {
        document.getElementById(clientID + "_Month").value = CurrMonth + 1;
    }
    if (document.getElementById(clientID + "_Day") != null) {
        document.getElementById(clientID + "_Day").value = CurrDay;
    }
    if (document.getElementById(clientID + "_Hours") != null) {
        document.getElementById(clientID + "_Hours").value = CurrHours;
    }
    if (document.getElementById(clientID + "_Minutes") != null) {
        document.getElementById(clientID + "_Minutes").value = CurrMinutes;
    }
    if (document.getElementById(clientID + "_Seconds") != null) {
        document.getElementById(clientID + "_Seconds").value = CurrSeconds;
    }
    
    var currDate = eval(clientID + "_Curr");
    RefreshDependencies(clientID);
    eval(clientID + "_ClientDateChanged(" + currDate.getTime() + ")");
    
    eval(clientID + "Recursive = false;");
    return true;
}

function SetDateInput(id, value, cType) {
    if (eval("typeof " + id + "_Curr === 'undefined'") == true) return;
    var currDate = eval(id + "_Curr");
    if (currDate == null) return;
    var currDateInMilliseconds = currDate.getTime();

    if (cType == 1 && value >= currDateInMilliseconds) 
    {
        return;
    }
    if (cType == 2 && value <= currDateInMilliseconds) {
       return;
    }
    var DateInFormat = new Date(value);
    if (document.getElementById(id + "_Hours") != null) {
        if (document.getElementById(id + "_Year" != null)) {
            document.getElementById(id + "_Year").value = DateInFormat.getFullYear();
            DateInputChanged(null, id, false);
        }
        if (document.getElementById(id + "_Month") != null) {
            document.getElementById(id + "_Month").value = (DateInFormat.getMonth() + 1);
            DateInputChanged(null, id, false);
        }
        if (document.getElementById(id + "_Day") != null) {
            document.getElementById(id + "_Day").value = DateInFormat.getDate();
            DateInputChanged(null, id, false);
        }
        if (document.getElementById(id + "_Hours") != null) {
            document.getElementById(id + "_Hours").value = DateInFormat.getHours();
            DateInputChanged(null, id, false);
        }
        if (document.getElementById(id + "_Minutes") != null) {
            document.getElementById(id + "_Minutes").value = DateInFormat.getMinutes();
            DateInputChanged(null, id, false);
        }
        if (document.getElementById(id + "_Seconds") != null) {
            document.getElementById(id + "_Seconds").value = DateInFormat.getSeconds();
            DateInputChanged(null, id, true);
        }
        else {
            if (document.getElementById(id + "_Year" != null)) {
                document.getElementById(id + "_Year").value = DateInFormat.getFullYear();
                DateInputChanged(null, id, false);
            }
            if (document.getElementById(id + "_Month") != null) {
                document.getElementById(id + "_Month").value = (DateInFormat.getMonth() + 1);
                DateInputChanged(null, id, false);
            }
            if (document.getElementById(id + "_Day") != null) {
                document.getElementById(id + "_Day").value = DateInFormat.getDate();
                DateInputChanged(null, id, true);
            }
        }
    }
}

function GetDateInput(id) {
    return eval(id + "_Curr");
}

function AddToUpdateList(id, toAdd) 
{
    if (id == null || toAdd == null) return;
    
    var currIndex = eval(id+"_UpdateListIndex");
    eval(id + "_UpdateList[" + currIndex + "] = '" + toAdd + "';");
    currIndex++;
    eval(id + "_UpdateListIndex = "+currIndex+";");
}

function RefreshDependencies(id) 
{
    if (eval("typeof " + id + "_UpdateListIndex === 'undefined'") == true) return;
    var length = eval(id + "_UpdateListIndex");
    if (length == null) return;
    for (var i = 0; i < length; i++) {
        DateInputChanged(null, eval(id + "_UpdateList[" + i + "]"), true);
    }
}


//////////////////////// DATAGRID /////////////////////////////

var DataButtonCancel = "DataButtonCancel";
var DataButtonDelete = "DataButtonDelete";
var DataButtonEdit = "DataButtonEdit";
var DataButtonInsert = "DataButtonInsert";
var DisplayPostfix ="_Display";
var EditPostfix = "_Edit";
var SavePostFix = "_Save";
var SavePostFixD = "_SaveD";
var ContainerPostFix = "_Container";
var VarPostfix = "_Var";

function DataButton_Click(itemRoot, itemChanged, dataButtonType) {
    if (dataButtonType == DataButtonDelete ) {
        $('#' + itemRoot + DisplayPostfix + ContainerPostFix).remove();
        eval(itemRoot + SavePostFix + " = null;");
        $('#' + itemChanged).val('True');
    }
    else if (dataButtonType == DataButtonEdit || dataButtonType == DataButtonInsert) {
        var temp = eval(itemRoot + SavePostFix);
        $('#' + itemRoot + DisplayPostfix + ContainerPostFix).before(temp.clone(true));

        $('#' + itemRoot + DisplayPostfix + ContainerPostFix).remove();
        
        
        $('#' + itemChanged).val('True');

    }
    else if (dataButtonType == DataButtonCancel) {
        var temp = eval(itemRoot + SavePostFixD);
        $('#' + itemRoot + EditPostfix + ContainerPostFix).before(temp.clone(true));
        $('#' + itemRoot + EditPostfix + ContainerPostFix).remove();
        
        if (eval(itemChanged + VarPostfix)) 
            $('#' + itemChanged).val('False');
    }

}

function DataGrid_Prepare_Item(itemRoot, itemChanged) 
{
    var temp = $('#' + itemRoot + EditPostfix + ContainerPostFix).clone(true);
    
    eval(itemRoot + SavePostFix + " = temp;");

    temp = $('#' + itemRoot + DisplayPostfix + ContainerPostFix).clone(true);

    eval(itemRoot + SavePostFixD + " = temp;");

    if (eval(itemChanged + VarPostfix)) {
        $('#' + itemRoot + EditPostfix + ContainerPostFix).remove();
    }
    else {
        $('#' + itemRoot + DisplayPostfix + ContainerPostFix).remove();
    }

    
}

///////////////////////////////////MANIPULATION BUTTONS/////////////////////////

var ManipulationButtonRemove = "ManipulationButtonRemove";
var ManipulationButtonHide = "ManipulationButtonHide";
var ManipulationButtonShow = "ManipulationButtonShow";
var ManipulationButtonCustom = "ManipulationButtonCustom";

function ManipulationButton_Click(target, dataButtonType) {
    var end_prefix = target.lastIndexOf("_");
    var deleteName = target.substring(0, end_prefix + 1) + "Deleted";
    var deletedHidden = document.getElementById(deleteName);
    if (dataButtonType == ManipulationButtonRemove) {
        $('#' + target).remove();

    }
    else if (dataButtonType == ManipulationButtonHide) {

        $('#' + target).css('visibility', 'hidden');
        if (deletedHidden != null) deletedHidden.value = "True";

    }
    else if (dataButtonType == ManipulationButtonShow) {

        $('#' + target).css('visibility', 'visible');
        if (deletedHidden != null) deletedHidden.value = "False";
    }
    else {
        eval(target);
    }

}

///////////////////////////////PAGER///////////////////////////////////


function PageButton_Click(pageField, pageValue) {
    var field = document.getElementById(pageField);
    field.value = pageValue;
    $('#' + pageField).parents('form').submit();
}