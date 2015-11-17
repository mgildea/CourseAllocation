/*
    * Custom Knockout binding to apply the selectpicker widget (bootstrap-select) to a bound dropdown and 
    * refresh it when the contents of the dropdown change.
    *
    * The binding can be used in two ways:
    *
    * 1. If you don't want to bind the dropdown's selected option to an observable, just use data-bind="selectPicker: true"
    * 2. If you wish to bind the dropdown's selected option to an observable, so that when a different option is selected 
    *    in the list it updates the observable, use data-bind="selectPicker: myObservable".
    *    If the options in the dropdown are generated from an observableArray, you can wrap the standard Knockout options binding parameters 
    *    in a selectPickerOptions object 
    *
    *    ie. data-bind="selectPicker: myObservable, selectPickerOptions: { options: myOptionsObservableArray, optionsText: 'arrayTextField', optionsValue: 'arrayValueField', optionsCaption: 'Please select...', disabledCondition: someBooleanExpression, resetOnDisabled: true|false }"
    *
    *    Note the two further parameters in the selectPickerOptions object, disabledCondition and resetOnDisabled. If provided, disabledCondition should be a boolean expression, that when evaluated as true will cause the selectpicker to be disabled.
    *    The resetOnDisabled parameter, if true, will cause the selected index of the dropdown to be reset to zero when it is disabled.
    *
    * Note requires that bootstrap-select.js is included in the view
    */
ko.bindingHandlers.selectPicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        if ($(element).is('select')) {
            if (ko.isObservable(valueAccessor())) {
                if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                    // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                    ko.bindingHandlers.selectedOptions.init(element, valueAccessor, allBindingsAccessor);
                } else {
                    // regular select and observable so call the default value binding
                    ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor);
                }
            }
            $(element).addClass('selectpicker').selectpicker();
        }
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        if ($(element).is('select')) {
            var selectPickerOptions = allBindingsAccessor().selectPickerOptions;
            if (typeof selectPickerOptions !== 'undefined' && selectPickerOptions !== null) {
                var options = selectPickerOptions.optionsArray,
                    optionsText = selectPickerOptions.optionsText,
                    optionsValue = selectPickerOptions.optionsValue,
                    optionsCaption = selectPickerOptions.optionsCaption,
                    isDisabled = selectPickerOptions.disabledCondition || false,
                    resetOnDisabled = selectPickerOptions.resetOnDisabled || false;
                if (ko.utils.unwrapObservable(options).length > 0) {
                    // call the default Knockout options binding
                    ko.bindingHandlers.options.update(element, options, allBindingsAccessor);
                }
                if (isDisabled && resetOnDisabled) {
                    // the dropdown is disabled and we need to reset it to its first option
                    $(element).selectpicker('val', $(element).children('option:first').val());
                }
                $(element).prop('disabled', isDisabled);
            }
            if (ko.isObservable(valueAccessor())) {
                if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                    // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                    ko.bindingHandlers.selectedOptions.update(element, valueAccessor);
                } else {
                    // call the default Knockout value binding
                    ko.bindingHandlers.value.update(element, valueAccessor);
                }
            }

            $(element).selectpicker('refresh');
        }
    }
};



//ko.bindingHandlers.selectPicker = {
//    init: function (element, valueAccessor, allBindingsAccessor) {
//        if ($(element).is('select')) {
//            if (ko.isObservable(valueAccessor())) {
//                if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
//                    // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
//                    ko.bindingHandlers.selectedOptions.init(element, valueAccessor, allBindingsAccessor);
//                }
//                else {
//                    // regular select and observable so call the default value binding
//                    ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor);
//                }
//            }
//            $(element).addClass('selectpicker').selectpicker();
//        }
//    },
//    update: function (element, valueAccessor, allBindingsAccessor) {
//        if ($(element).is('select')) {
//            var selectPickerOptions = allBindingsAccessor().selectPickerOptions;
//            if (typeof selectPickerOptions !== 'undefined' && selectPickerOptions !== null) {
//                var options = selectPickerOptions.options,
//                    optionsText = selectPickerOptions.optionsText,
//                    optionsValue = selectPickerOptions.optionsValue,
//                    optionsCaption = selectPickerOptions.optionsCaption,
//                    isDisabled = selectPickerOptions.disabledCondition || false,
//                    resetOnDisabled = selectPickerOptions.resetOnDisabled || false;
//                if (ko.utils.unwrapObservable(options).length > 0) {
//                    // call the default Knockout options binding
//                    ko.bindingHandlers.options.update(element, options, ko.observable({ optionsText: optionsText, optionsValue: optionsValue, optionsCaption: optionsCaption }));
//                }
//                if (isDisabled && resetOnDisabled) {
//                    // the dropdown is disabled and we need to reset it to its first option
//                    $(element).selectpicker('val', $(element).children('option:first').val());
//                }
//                $(element).prop('disabled', isDisabled);
//            }
//            if (ko.isObservable(valueAccessor())) {
//                if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
//                    // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
//                    ko.bindingHandlers.selectedOptions.update(element, valueAccessor);
//                }
//                else {
//                    // call the default Knockout value binding
//                    ko.bindingHandlers.value.update(element, valueAccessor);
//                }
//            }

//            $(element).selectpicker('refresh');
//        }
//    }
//}