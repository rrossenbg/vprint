/* ****************************************************************************
*  MvcControlToolkit.JsQueryable-2.0.0.js
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
(function () {
    mvcct.$$ = {
        and: 'and',
        or: 'or',
        not: 'not',
        eq: 'eq',
        ne: 'ne',
        gt: 'gt',
        ge: 'ge',
        lt: 'lt',
        le: 'le',
        endswith: 'endswith',
        startswith: 'startswith',
        substringof: 'substringof',
        substringofInv: 'substringofInv',
        notSupported: 'ns',
        encodeCondition: function (code) {
            if (code == 'Equal' || code == "1") {
                return mvcct.$$.eq;
            }
            else if (code == 'NotEqual' || code == "2") {
                return mvcct.$$.ne;
            }
            else if (code == 'LessThan' || code == "4") {
                return mvcct.$$.lt;
            }
            else if (code == 'LessThanOrEqual' || code == "8") {
                return mvcct.$$.le;
            }
            else if (code == 'GreaterThan' || code == "16") {
                return mvcct.$$.gt;
            }
            else if (code == 'GreaterThanOrEqual' || code == "32") {
                return mvcct.$$.ge;
            }
            else if (code == 'StartsWith' || code == "64") {
                return mvcct.$$.startswith;
            }
            else if (code == 'EndsWith' || code == "128") {
                return mvcct.$$.endswith;
            }
            else if (code == 'Contains' || code == "256") {
                return mvcct.$$.substringofInv;
            }
            else if (code == 'IsContainedIn' || code == "512") {
                return mvcct.$$.substringof;
            }
            else {
                return mvcct.$$.notSupported
            }
        }
    };

    mvcct.Queryable = function (fop, negate) {
        var _filterOperator = fop || this.$$.and;

        return {
            filterOperator: function () { return _filterOperator },
            get: function () {
                return null;
            },
            execute: function (callBack) {

            },
            getState: function () {
                return null;
            },
            setState: function (state) {

            },
            resetFilter: function () {
                return this;
            },
            resetSorting: function () {
                return this;
            },
            resetPaging: function () {
                return this;
            },
            setSorting: function (sortString) {
                return this;
            },
            setFilter: function (filterString) {
                return this;
            },
            setPaging: function (page, pageSize) {
                return this;
            },
            importSorting: function (sortString) {
                if (sortString == null || sortString == '') return this;
                this.resetSorting();
                var allConditions = sortString.split(';');
                for (var i = 0; i < allConditions.length; i++) {
                    var pair = allConditions[i].split('#');
                    if (pair.length < 2) continue;
                    this.addSort(MvcControlsToolkit_Trim(pair[0]), pair[1].indexOf('-') >= 0);
                }
                return this;
            },
            importSortingControl: function (sortingControlId) {
                return this.importSorting($('#' + sortingControlId + '___SortInfoAsString').val());
            },
            importPager: function (pagerId, pageSize) {
                var pager = $('#' + pagerId);
                this.setPaging(parseInt(pager.val()), pageSize);
                return this;
            },
            addSort: function (field, desc, enabled) {
                return this;
            },
            addCondition: function (operator, value1, value2, enabled, currType) {
                return this;
            },
            addConditionAsString: function (operator, field, currSearch, currType, enabled) {
                if (enabled === false) return this;
                currSearch = MvcControlsToolkit_Parse(currSearch, currType);
                if (currType == 0 || (currType == 4 && currSearch) || (currType > 0 && currType < 4 && !isNaN(currSearch))) {
                    this.addCondition(operator, field, currSearch, currType, enabled);
                }
            },
            open: function (logicalOperator, enabled, negate) {
                return res;
            },
            close: function (enabled) {
                if ((this['father'] || null) == null) return this;
                return this.father;
            },
            importClauses: function (filterID) {
                var index = 0;
                var finished = false;
                var inner = this;
                if (this.filterOperator() != mvcct.$$.and) inner = this.open(mvcct.$$.and, true);
                while (!finished) {
                    var ph = $('#' + filterID + '___' + index);
                    var base = filterID + '___' + index + '___';

                    if (ph.length != 0) {
                        var selector = $('#' + base + 'Selected');
                        if (selector.length > 0 && (selector.val() == 'True' || (selector.prop("checked") || false))) {

                            var currCondition = mvcct.$$.encodeCondition($('#' + base + 'Condition').val());
                            if (currCondition != mvcct.$$.notSupported) {
                                var field = $('#' + filterID + "___" + index + "_f_ields").val().split(',')[0];
                                var currSearchDom = $('#' + base + 'Search');
                                var currSearch = null;
                                var currType = null;
                                var control = $('#' + base + 'Search[data-element-type], #' + base + 'Search_hidden[data-element-type], #' + base + 'Search___Hidden[data-element-type]');
                                if (control.length > 0) {
                                    currType = parseInt(control.attr('data-client-type') || "0");
                                    var element = control[0];
                                    currSearch = eval("MvcControlsToolkit_" + control.attr('data-element-type') + "_Get(element, currType)");
                                    if (currType == 0 || (currType == 4 && currSearch) || (currType > 0 && currType < 4 && !isNaN(currSearch))) {
                                        inner.addCondition(currCondition, field, currSearch, currType);
                                    }
                                }
                                else {
                                    currSearch = currSearchDom.val();
                                    currType = parseInt(currSearchDom.attr('data-client-type') || "0");
                                    inner.addConditionAsString(currCondition, field, currSearch, currType);
                                }

                            }
                        }
                    }
                    else
                        finished = true;
                    index++;
                }
                if (this.filterOperator() != mvcct.$$.and) inner.close(true);
                return this;
            }

        };
    };
    mvcct.oDataQueryable = function (link, fop, options, negate) {
        var filter = '';
        var sorting = '';
        var paging = '';
        var ancestor = this.Queryable(fop, negate);
        options = $.extend({}, mvcct.oDataQueryable.DefaulOptions, options);
        return $.extend({}, ancestor,
     {
         get: function () {
             var res = '';
             if (options.includeTotalcount) {
                 if (res != '') res = res + '&';
                 res = res + '$inlinecount=allpages';
             }
             if (filter != '') {
                 if (res != '') res = res + '&';
                 if (negate === true) res = res + mvcct.$$.not + ' (' + filter + ')';
                 res = res + filter;
             }
             if (sorting != '') {
                 if (res != '') res = res + '&';
                 res = res + sorting;
             }
             if (paging != '') {
                 if (res != '') res = res + '&';
                 res = res + paging;
             }
             if (res != '') res = link + options.connector + res;
             else res = link;
             return res;
         },
         execute: function (callBack, errorCallback) {
             $.ajax({
                 url: this.get(),
                 contentType: "application/json",
                 dataType: "text",
                 success: function (data, textStatus, jqXHR) {
                     data = $.parseJSON(data);
                     callBack(data, textStatus, jqXHR);
                 },
                 error: errorCallback
             });
         },
         setState: function (state) {
             filter = state.f;
             sorting = state.s;
             paging = state.p;
         },
         getState: function () {
             var res =
                    {
                        f: filter,
                        s: sorting,
                        p: paging
                    };
             return res;
         },
         resetFilter: function () {
             filter = '';
             return this;
         },
         resetSorting: function () {
             sorting = '';
             return this;
         },
         resetPaging: function () {
             paging = '';
             return this;
         },
         setSorting: function (sortString) {
             sorting = sortString;
             return this;
         },
         setFilter: function (filterString) {
             filter = filterString;
             return this;
         },
         setPaging: function (page, pageSize) {
             if (pageSize == null || pageSize == '') pageSize = 1;
             var skip = (page - 1) * pageSize;
             paging = options.skip + skip + "&" + options.top + pageSize;
             return this;
         },
         addSort: function (field, desc, enabled) {
             if (enabled === false) return this;
             field = field.replace(".", "/");
             if (sorting != '') sorting = sorting + ",";
             else sorting = options.orderby;
             sorting = sorting + field + ' ' + (desc ? options.desc : options.asc);
             return this;
         },
         addStringCondition: function (condition, enabled) {
             if (enabled === false || condition == '') return this;
             if (filter != '') filter = filter + ' ' + this.filterOperator() + ' ';
             else filter = options.filter;
             filter = filter + condition;
             return this;
         },
         addCondition: function (operator, value1, value2, currType, enabled) {
             if (enabled === false) return this;
             value1 = value1.replace(".", "/");
             if (!currType) {
                 if (mvcct.utils.isDate(value2)) currType = 4;
                 else if (mvcct.utils.isString(value2)) currType = 0;
                 else currType = 3;
             }
             if (value2 == null) value2 = 'null';
             else if (currType == 4) {
                 value2 = "datetime'" + mvcct.utils.ISODate(new Date(value2.getTime() - value2.getTimezoneOffset() * 60000), true) + "'";
             }
             else {
                 value2 = value2 + '';
                 if (currType <= 0) {
                     if (mvcct.utils.isGuid(value2)) value2 = "guid'" + value2 + "'";
                     else value2 = "'" + encodeURIComponent(value2) + "'";
                 }
             }
             if (operator == mvcct.$$.substringof || operator == mvcct.$$.startswith || operator == mvcct.$$.endswith) {
                 this.addStringCondition(operator + '(' + value1 + ',' + value2 + ') eq true');
             }
             else if (operator == mvcct.$$.substringofInv) {
                 this.addStringCondition(mvcct.$$.substringof + '(' + value2 + ',' + value1 + ') eq true');
             }
             else {
                 this.addStringCondition(value1 + ' ' + operator + ' ' + value2);
             }
             return this;
         },
         open: function (logicalOperator, enabled, negate) {
             var newOption = $.extend({}, options, { connector: '', filter: '' });
             var res = MvcControlsToolkit_SQueryable('', logicalOperator, newOption, negate);
             res['father'] = this;
             return res;
         },
         close: function (enabled) {
             if ((this['father'] || null) == null) return this;
             var res = this.get();
             if (res != '') {
                 res = '(' + res + ')';
                 this.father.addStringCondition(res, enabled);
             }
             return this.father;
         }
     });
    };
    mvcct.oDataQueryable.DefaulOptions = {
        includeTotalcount: true,
        connector: '?',
        skip: '$skip=',
        top: '$top=',
        orderby: '$orderby=',
        filter: '$filter=',
        desc: 'desc',
        asc: 'asc'
    };

    mvcct.upshotQueryable = function (dataSource, fop, options, negate) {
        var filter = [];
        var sorting = [];
        var paging = null;
        var ancestor = this.Queryable(fop, negate);
        options = $.extend({}, mvcct.upshotQueryable.DefaulOptions, options);
        function getOperator(operator) {
            switch (operator) {
                case mvcct.$$.lt: return "<";
                case mvcct.$$.le: return "<=";
                case mvcct.$$.eq: return "==";
                case mvcct.$$.ne: return "!=";
                case mvcct.$$.ge: return ">=";
                case mvcct.$$.gt: return ">";
                case mvcct.$$.startswith: return "StartsWith";
                case mvcct.$$.endswith: return "EndsWith";
                case mvcct.$$.substringofInv: return "Contains";
                default: throw "The operator '" + operator + "' is not supported.";
            }
        }
        return $.extend({}, ancestor,
     {
         get: function () {
             var res = '';
             if (filter.length > 0) dataSource.setFilter(filter);
             else dataSource.setFilter(null);
             if (sorting.length > 0) dataSource.setSort(sorting);
             else dataSource.setSort(null);
             dataSource.setPaging(paging);
             return dataSource;
         },
         execute: function (callBack) {
             this.get();
             dataSource.refresh();
         },
         setState: function (state) {
             filter = state.f;
             sorting = state.s;
             paging = state.p;
         },
         getState: function () {
             var res =
                    {
                        f: filter,
                        s: sorting,
                        p: paging
                    };
             return res;
         },
         resetFilter: function () {
             filter = [];
             return this;
         },
         resetSorting: function () {
             sorting = [];
             return this;
         },
         resetPaging: function () {
             paging = null;
             return this;
         },
         setSorting: function (sortArray) {
             sorting = sortArray;
             return this;
         },
         setFilter: function (filterArray) {
             filter = filterArray;
             return this;
         },
         setPaging: function (page, pageSize) {
             if (pageSize == null || pageSize == '') pageSize = 1;
             var skip = (page - 1) * pageSize;
             paging = { skip: skip, take: pageSize, includeTotalCount: options.includeTotalcount };
             return this;
         },
         addSort: function (field, desc, enabled) {
             if (enabled === false) return this;
             sorting.push({
                 property: field,
                 descending: desc
             });
             return this;
         },
         addCondition: function (operator, value1, currSearch, currType, enabled) {
             if (enabled === false) return this;
             filter.push({
                 property: value1,
                 value: currSearch,
                 operator: getOperator(operator)
             });
             return this;
         },
         open: function (logicalOperator, enabled, negate) {
             throw "The method open is not supported.";
             return res;
         },
         close: function (enabled) {
             throw "The method close is not supported.";
             return this.father;
         }
     });
    };
    mvcct.upshotQueryable.DefaulOptions = {
        includeTotalcount: true
    };
})();





(function () {
    property=mvcct.utils.property;
    propertySet = mvcct.utils.propertySet;
    function conditionFarm(condition, field, value){
        if (condition == mvcct.$$.eq) return function (item){
            return property(item, field) == value;
        }
        else if (condition == mvcct.$$.gt) return function (item){
            return property(item, field) > value;
        }
        else if (condition == mvcct.$$.ge) return function (item){
            return property(item, field) >= value;
        }
        else if (condition == mvcct.$$.lt) return function (item){
            return property(item, field) < value;
        }
        else if (condition == mvcct.$$.le) return function (item){
            return property(item, field) <= value;
        }
        else if (condition == mvcct.$$.ne) return function (item){
            return property(item, field) != value;
        }
        else if (condition == mvcct.$$.startswith) return function (item){
            var pValue = property(item, field);
            if (pValue == null || value==null) return false;
            return  pValue.indexOf(value) == 0;
        }
        else if (condition == mvcct.$$.endswith) return function (item){
            var pValue = property(item, field);
            if (pValue == null || value==null) return false;
            return  pValue.indexOf(value) == pValue.length-value.length;
        }
        else if (condition == mvcct.$$.substringof) return function (item){
            var pValue = property(item, field);
            if (pValue == null || value==null) return false;
            return  value.indexOf(pValue) >= 0;
        }
        else if (condition == mvcct.$$.substringofInv) return function (item){
            var pValue = property(item, field);
            if (pValue == null || value==null) return false;
            return  pValue.indexOf(value) >=0;
        }
    }
    mvcct.localQueryable = function (array, fop, negate){
       var ancestor = mvcct.Queryable(fop, negate);
       var filter = [];
       var sorting = [];
       var pagingSkip = 0;
       var pagingTop = 5;
       var cachedArray=null;
       function doPage(){
            var res;
            var skip = Math.min(pagingSkip, cachedArray.length);
            var top = Math.min(pagingTop, cachedArray.length-skip);
            res = [];for(var i=0; i<cachedArray.length; i++) res.push(cachedArray[i]);
            if (skip > 0) res.splice(0, skip);
            if (top == 0) res = [];
            else if (top < res.length) res.splice(top, res.length-top); 
            return res;
       }       
       return $.extend({}, ancestor, {
         get: function () {
             var f = this.getFilter(); 
             var s=this.getSorting();
             return function (source){
                var filtered = [];               
                if (f != null){
                    for(var i=0; i<source.length; i++){
                        if (f(source[i])) filtered.push(source[i]);
                    }
                }
                else
                    filtered=source;
                if (s==null) cachedArray=filtered;
                else cachedArray=filtered.sort(s);
                return doPage();              
             }
         },
         getFilter: function(){
            if (filter.length == 0) return null;
            if (this.filterOperator() == mvcct.$$.and){
                if (negate){
                    return function(item){
                        var res=true;
                        for (var i=0; i<filter.length; i++){
                            res=res && filter[i](item);
                        }
                        return !res;
                    };
                }
                else{
                    return function(item){
                        var res=true;
                        for (var i=0; i<filter.length; i++){
                            res=res && filter[i](item);
                        }
                        return res;
                    };
                }
            }
            else{
                if (negate){
                    return function(item){
                        var res=true;
                        for (var i=0; i<filter.length; i++){
                            res=res || filter[i](item);
                        }
                        return !res;
                    };
                }
                else{
                    return function(item){
                        var res=true;
                        for (var i=0; i<filter.length; i++){
                            res=res || filter[i](item);
                        }
                        return res;
                    };
                }
            }

         },
         getSorting: function(){
            if (sorting.length == 0) return null; 
            return function(item1, item2){
                for (var i=0; i<sorting.length; i++){
                    var res=sorting[i](item1, item2);
                    if (res != 0) return res;
                }
                return 0;
            };
         },
         execute: function (callback) {
             if (cachedArray != null) callback(doPage());
             else callback(this.get()(array));
         },
         setState: function (state) {
            cachedArray=null;
            filter= state.f;
            sorting= state.s;
            pagingSkip = state.ps;
            pagingTop = state.pt;
         },
         getState: function () {
                var res=
                    {
                        f: filter,
                        s: sorting,
                        ps: pagingSkip,
                        pt: pagingTop
                    };
                return res;
          }, 
         resetFilter: function () {
             filter = [];
             cachedArray=null;
             return this;
         },
         resetSorting: function () {
             sorting = [];
             cachedArray=null;
             return this;
         },
         resetPaging: function () {
             pagingSkip = 0;
             return this;
         },
         setSorting: function (sortArray) {
             cachedArray=null;
             sorting = sortArray;
             return this;
         },
         setFilter: function (filterArray) {
             cachedArray=null;
             filter = filterArray;
             return this;
         },
         setPaging: function (page, pageSize) {
             if (pageSize == null || pageSize == '') pageSize = 1;
             pagingSkip = (page - 1) * pageSize;
             pagingTop = pageSize;
             return this;
         },
         addSort: function (field, desc, enabled) {
             if (enabled === false) return this;
             cachedArray=null;
             if (desc){
                 sorting.push(
                    function(x, y){
                        val1 = property(x, field);
                        val2 = property(y, field);
                        if (val1<val2) return 1;
                        else if (val2<val1) return -1;
                        else return 0;
                    }
                 );
             }
             else{
                sorting.push(
                    function(x, y){
                        val1 = property(x, field);
                        val2 = property(y, field);
                        if (val1<val2) return -1;
                        else if (val2<val1) return 1;
                        else return 0;
                    }
                 );
             }
             return this;
         },
         addArrayCondition: function (condition, enabled) {
             if (enabled === false || condition == null) return this;
             cachedArray =null;
             filter = filter.concat(condition);
             return this;
         },
         addCondition: function (operator, value1, value2, enabled, currType) {
             if (enabled === false) return this;
             var func = conditionFarm(operator, value1, value2);
             if (func != null) {
                filter.push(func);
                cachedArray =null;
             }
             return this;
         },
         open: function (logicalOperator, enabled, negate) {           
             var res = MvcControlsToolkit_CQueryable('', logicalOperator, negate);
             res['father'] = this;
             return res;
         },
         close: function (enabled) {
             if ((this['father'] || null) == null) return this;
             var res = this.getFilter();
             if (res != null) {
                 this.father.addCondition(res, enabled);
             }
             return this.father;
         }
       });
    };
    
    mvcct.updatesManager=function(
        updateUrl,
        sourceViewModel,
        sourceExpression,
        keyExpression,
        destinationViewModel,
        destinationExpression,
        options
        ){
        
        if (!sourceViewModel) throw "sourceViewModel is not optional";
        options=options||{};
        var _waiting=null;
        var _lastErrors=null;
        
        function prepareData(){
            var data = mvcct.utils.property(sourceViewModel, sourceExpression);
            if (! mvcct.utils.isArray(data)) return;
            var insertions=[];
            var deletions=[];
            var updates=[];
            var iIndex=[];
            var uIndex=[];
            var originalInserted=[];
            var aChange=false;
            for (var i=0; i<data.length; i++){
                var curr = ko.utils.unwrapObservable(data[i]);
                var attempt = ko.utils.unwrapObservable(curr['_modified']);
                if (attempt){
                    if(mvcct.utils.changed(curr)){
                        updates.push(mvcct.utils.updateCopy(curr));
                        uIndex.push(curr._tag);
                        aChange=true;
                    }
                    else{
                        curr._modified(false);
                    }
                    continue;
                }
                attempt = ko.utils.unwrapObservable(curr['_inserted']);
                if (attempt){
                    insertions.push(mvcct.utils.updateCopy(curr));
                    originalInserted.push(curr);
                    iIndex.push(curr._tag);
                    aChange=true;
                    continue;
                }
                attempt = curr['_destroy'];
                if (attempt){
                    deletions.push( mvcct.utils.property(curr, keyExpression));
                    aChange=true;
                    continue;
                }
            }
            var justCreated=false;
            var result={};
            result[options.updater.i]=insertions;
            result[options.updater.u]=updates;
            result[options.updater.d]=deletions
            if (!destinationExpression) {
                if (!destinationViewModel) justCreated=true;
                destinationViewModel = result;
                 
            }
            else if (!destinationViewModel) {
                 destinationViewModel={};
                 justCreated=true;
                 mvcct.utils.propertySet(destinationViewModel, destinationExpression, result);
            } 
            else mvcct.utils.propertySet(destinationViewModel, destinationExpression, result);
            return {changes: aChange, i: iIndex, u: uIndex, _justCreated_: justCreated, inserted: originalInserted};
        };
        function adjustErrors (errors, indices){
            if (!errors) return;
            iPrefix=destinationExpression ? destinationExpression+"."+options.updater.i : options.updater.i;
            uPrefix=destinationExpression ? destinationExpression+"."+options.updater.u : options.updater.u;
            for(var i=0; i< errors.length; i++){
                if (errors[i]["_pocessed"]) continue;
                var attempt = mvcct.utils.changeIndex(iPrefix, sourceExpression, errors[i].name, 
                    function(index){
                        return indices.i[index];
                    });
                if (!attempt){
                    attempt = mvcct.utils.changeIndex(uPrefix, sourceExpression, errors[i].name, 
                    function(index){
                        return indices.u[index];
                    });
                } 
                if (attempt){
                    errors[i]._pocessed=true;
                    errors[i].name = attempt;
                    errors[i].id = mvcct.utils.idFromName(attempt);
                }
            }
        };
        function postData(self, isDependent, callback){
            if (isDependent) {
                _waiting=callback;
                return;
            }
            if (!updateUrl) throw "updateUrl is not optional";
            $.ajax({
                url: updateUrl,
                contentType: "application/json",
                data: mvcct.utils.stringify(ko.mapping.toJS(destinationViewModel), options['isoDate'], options['noTimelineCorrection']),
                dataType: "text",
                type: "POST",
                success: function (data, textStatus, jqXHR){
                    data=$.parseJSON(data);
                    if (!data) data = {errors: null} ;
                    else if (data['errors'] && mvcct.utils.isArray(data.errors) && data.errors.length == 0) data.errors = null;
                    else if (! data['errors']) data.errors=null;
                    callback(data, self, jqXHR.status);
                },
                error: function (jqXHR, statusText, errorText){ 
                    var text=jqXHR['responseText']||null;
                    data= text ? $.parseJSON(text) : {};
                    if (mvcct.utils.isArray(data)) data={errors:data};
                    if (! data['errors']) data.errors=null;
                    if (mvcct.utils.isArray(data.errors) && data.errors.length == 0) data.errors = null;
                    callback(data, self, jqXHR.status);
                }
            });
        };
        function updateCollection(self, result, indices){
            var data = mvcct.utils.property(sourceViewModel, sourceExpression);
            if (! mvcct.utils.isArray(data)) return;
            var res = [];
            for (var i = 0; i< data.length; i++) {
                if (!data[i]['_destroy']) {
                    self.accepted(data[i]);
                    res.push(data[i]);
                }
            };
            if (result && result['insertedKeys']){
                for(var i=0; i<result.insertedKeys.length; i++){
                    var currI=result.insertedKeys[i];
                    if (!destinationExpression || destinationExpression == currI['destinationExpression']){
                        var inserted = currI['keys'];
                        if (inserted && inserted['length']){
                            inserted = ko.utils.unwrapObservable(ko.mapping.fromJS(inserted));
                            var originalInserted=indices.inserted;
                            for(var j=0; j<inserted.length; j++){
                                propertySet(originalInserted[i], keyExpression, inserted[j]);  
                            }
                        }
                        break;
                    }
                }
            }
            mvcct.utils.propertySet(sourceViewModel, sourceExpression, res);
        };
        options = $.extend({}, 
            {
                updater: {u: "Modified", i: "Inserted", d: "Deleted"},
                updateCallback: function(e, result, status){},
                updatingCallback: function (changes, modelToPost, expr){return changes;}
            }, options);
        return {
            prepare: function(entities, track, visitRelation, cloneArray){
                if (!entities) return;
                entities=ko.utils.unwrapObservable(entities);
                if (!mvcct.utils.isArray(entities)) entities=[entities];
                for(var i=0; i<entities.length; i++){
                    var data = entities[i];
                    if (!data['_inserted']) data._inserted = ko.observable(false);
                    if (!data['_modified']) data._modified = ko.observable(false);
                    if (track) mvcct.utils.Track(data, visitRelation, cloneArray);
                }
            },
            refreshErrors: function(jForm, errorState){
                if (!errorState) errorState=_lastErrors;
                var errors = [];
                    if(errorState && errorState.errors) errors = errorState.errors;
                    jForm.find('.input-validation-error').removeClass('input-validation-error');
                    jForm.find('.field-validation-error').removeClass('field-validation-error').addClass('field-validation-valid');
                    var container = jForm.find("[data-valmsg-summary=true]");
                    list = container.find("ul");
                    list.empty();
                    if (errors.length > 0) {      
                        $.each(errors, function (i, ival) {
                            $.each(ival.errors, function(j, jval){
                                 $("<li />").html(jval).appendTo(list);
                            });

                         });
                         container.addClass("validation-summary-errors").removeClass("validation-summary-valid");
                         MvcControlsToolkit_ServerErrors(errors);
                         setTimeout(function () { jForm.find('span.input-validation-error[data-element-type]').removeClass('input-validation-error') }, 0);
                    }
                    else {
                         container.addClass("validation-summary-valid").removeClass("validation-summary-errors");
                    }  
            },
            clearErrors: function(jForm){
                this.refreshErrors(jForm, {errors:null});
            },
            modified: function (item, track, immediateTrack, visitRelation, cloneArray){
                var x = ko.utils.unwrapObservable(item); 
                if ((!ko.utils.unwrapObservable(x['_inserted'])) && (!x['_destroy'])){  
                     if (immediateTrack){
                        x._modified(mvcct.utils.changed(item));
                     }
                     else if (track) {
                        x._modified(true);
                        mvcct.utils.Track(item, visitRelation, cloneArray);
                     }
                     else x._modified(true);
                }
            },
            inserted: function (array, item){
                var x = ko.utils.unwrapObservable(item); 
                array.push(item);
                x._inserted(true); 
            },
            deleted: function (array, item){
                var x= ko.utils.unwrapObservable(item);
                if (ko.utils.unwrapObservable(x['_inserted'])) array.remove(x);
                else {
                    array.destroy(x);
                    x._modified(false);
                }
            },
            accepted: function (item){
                var x= ko.utils.unwrapObservable(item);
                x._destroy=false;
                x._inserted(false);
                x._modified(false);
                mvcct.utils.accept(item);
            },
            newResult: function(result, status){
                if (_waiting) _waiting(result, this, status);
                _waiting=null;
            },
            reset: function(item){
                if (ko.utils.unwrapObservable(item['_modified'])){
                    mvcct.utils.undo(item);
                    item['_modified'](false);
                 }
                 else if (ko.utils.unwrapObservable(item['_inserted'])){
                    var data=mvcct.utils.property(sourceViewModel, sourceExpression, true);
                    data.remove(item);
                 }
            },
            addRelated: function(collectionExpression, entities, entitiesExternalExpression, inverseCollectionExpression, overrideKeyExpression){
                var data= mvcct.utils.property(sourceViewModel, sourceExpression);
                if (!data) return;
                overrideKeyExpression=overrideKeyExpression || keyExpression;
                entities=ko.utils.unwrapObservable(entities);
                if (!mvcct.utils.isArray(entities)) entities=[entities];
                var lookup = {};
                for(var i=0; i< entities.length; i++){
                    var name = mvcct.utils.property(entities[i], entitiesExternalExpression);
                    var prop=lookup[name];
                    if (prop) prop.push(entities[i]);
                    else lookup[name]=[entities[i]];
                }
                for (var i=0; i<data.length; i++){
                    var curr=lookup[mvcct.utils.property(data[i], overrideKeyExpression)];
                    var collection = mvcct.utils.property(data[i], collectionExpression, true);
                    if (curr){
                        if (ko.isObservable(collection)) {
                            var innerData = ko.utils.unwrapObservable(collection);
                            innerData.push.apply(innerData, curr);
                            collection(innerData);
                        }
                        else if (collection && mvcct.utils.isArray(collection)) collection.push.apply(collection, curr);
                        else  mvcct.utils.propertySet(data[i], collectionExpression, ko.observableArray(curr));
                        if (inverseCollectionExpression){
                            for(var j=0; j<curr.length; j++){
                                var inverseCollection =  mvcct.utils.property(curr[j], inverseCollectionExpression, true);
                                if (ko.isObservable(inverseCollection)) {
                                    inverseCollection.push(data[i]);
                                }
                                else if (inverseCollection && mvcct.utils.isArray(inverseCollection)) inverseCollection.push(data[i]);
                                else  mvcct.utils.propertySet(curr[j], inverseCollectionExpression, ko.observableArray([data[i]]));
                            }
                        }
                    }
                    else if (!collection) mvcct.utils.propertySet(data[i], collectionExpression, ko.observableArray([]));
                }

            }, 
            resetAll: function(jErrorForm){
                var data = mvcct.utils.property(sourceViewModel, sourceExpression);
                var res=[];
                for (var i = 0; i< data.length; i++){
                    var curr = data[i];
                    if (ko.utils.unwrapObservable(curr['_inserted'])) continue;
                    else if (curr['_destroy']) curr['_destroy'] = false;
                    else if (ko.utils.unwrapObservable(curr['_modified'])){ 
                        mvcct.utils.undo(curr);
                        curr['_modified'](false);
                    }
                    res.push(curr);
                }
                _lastErrors=null;
                mvcct.utils.propertySet(sourceViewModel, sourceExpression, res);
                this.refreshErrors(jErrorForm);
            }, 
            submit: function(jForm, isDependent){
                 var indices=prepareData();
                 if (! options.updatingCallback(indices.changes, destinationViewModel, destinationExpression)) return;
                 if (!isDependent) 
                 {
                    if ((!jForm) || jForm.length == 0 ){
                        jForm=$('#_DynamicJSonFormtoSubmit_');
                        if(jForm.length == 0){
                            if (!updateUrl) throw "updateUrl is not optional";
                            $('body').first().prepend("<form id='_DynamicJSonFormtoSubmit_' action='"+updateUrl+"' method='post'><input type='hidden' name='display.$$' value='MVCControlsToolkit.Controls.Bindings.JSONAdapter' /><input type='hidden' name='$.JSonModel' value='' class='JSonModeltoSubmit' /></form>");
                            jForm=$('#_DynamicJSonFormtoSubmit_');
                            jForm.find('.JSonModeltoSubmit').val( mvcct.utils.stringify(ko.mapping.toJS(destinationViewModel), options['isoDate']));
                        }
                    } 
                    jForm.submit();
                 }
            },
            update:function(jErrorForm, isDependent, dependentRequests){
                var indices=prepareData();
                if (! options.updatingCallback(indices.changes, destinationViewModel, destinationExpression)) return;
                postData(this, isDependent, function(result, self, status){
                    if (result.errors) adjustErrors(result.errors, indices);
                    var e = {
                        setErrors: true,
                        model: sourceViewModel,
                        expression: sourceExpression,
                        key: keyExpression,
                        success: !result.errors
                    };
                    options.updateCallback(e, result, status);
                    _lastErrors=result;
                    if (!result.errors) updateCollection(self, result, indices);
                    if (e.setErrors) self.refreshErrors(jErrorForm, result);
                    
                    if (dependentRequests){
                        if (!mvcct.utils.isArray(dependentRequests)) dependentRequests=[dependentRequests];
                        for (var i=0; i<dependentRequests.length; i++) dependentRequests[i].newResult(result, status);  
                    }
                });
            }
        };
    };

})();





  