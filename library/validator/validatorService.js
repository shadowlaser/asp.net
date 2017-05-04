define('validatorService', ['app', 'angular', 'linq'], function (app, angular) {
    app.provider('validatorService', function () {

        this.$get = [
            'ValidateHelper', 'linq', function (validateHelper, linq) {
                var service = {};

                service.builderinValidators = {

                    RequiredValidator: validateHelper.getValidator('Required'),

                    RegularExpressionValidator: validateHelper.getValidator('RegularExpression'),

                    FileRequiredValidator: function (message) {
                        this.validateData = function (value, name, validationContext) {
                            var isValide;
                            if (!value) {
                                isValide = false;
                            } else {
                                isValide = linq.from(value).any(function (file) { return !file.isDeleted; });
                            }
                            if (!isValide) {
                                validateHelper.updateValidationContext(validationContext, name, message || '附件不能我空');
                            }
                            return isValide;
                        };
                    },

                    MaxLengthValidator: function (maxLen, message) {
                        this.validateData = function (value, name, validationContext) {
                            if (value && value.length > maxLen) {
                                validateHelper.updateValidationContext(validationContext, name, message || '长度不能超过' + maxLen);
                                return false;
                            }
                            return true;
                        };
                    }
                };

                var lowerFirstLetter = function (str) {
                    return str.substr(0, 1).toLowerCase() + str.substr(1, str.length);
                };


                var servetToClient = function (serverValidator, fieldValidator) {
                    var fullName = serverValidator.typeId.substr(0, serverValidator.typeId.indexOf(','));

                    var clientValidator = null;
                    switch (fullName) {
                        case 'System.ComponentModel.DataAnnotations.StringLengthAttribute':
                            clientValidator = new service.builderinValidators
                                .MaxLengthValidator(serverValidator.maximumLength, serverValidator.errorMessage);
                            break;
                        case 'System.ComponentModel.DataAnnotations.MaxLengthAttribute'://?服务器端有这东西吗?
                            clientValidator = new service.builderinValidators
                                .MaxLengthValidator(serverValidator.maximumLength, serverValidator.errorMessage);
                            break;
                        case 'System.ComponentModel.DataAnnotations.RequiredAttribute':
                            if (fieldValidator.typeFullName === 'Seagull2.Owin.File.Models.ClientFileInformation') {
                                clientValidator = new service.builderinValidators
                                    .FileRequiredValidator(serverValidator.errorMessage);
                            } else {
                                clientValidator = new service.builderinValidators
                                    .RequiredValidator(serverValidator.errorMessage);
                            }
                            break;
                        case 'System.ComponentModel.DataAnnotations.RegularExpressionAttribute':
                            clientValidator = new service.builderinValidators
                                    .RegularExpressionValidator(new RegExp(serverValidator.pattern), serverValidator.errorMessage);
                            break;
                        default:
                            break;
                    }
                    return {
                        key: fieldValidator.description,
                        attributeName: lowerFirstLetter(fieldValidator.name),
                        validator: clientValidator
                    };
                };

                service.parseValidators = function (serverTypeValidator, customClientValidators) {
                    var clientValidators = [];
                    for (var i = 0; i < serverTypeValidator.fieldValidator.length; i++) {
                        var fieldValidator = serverTypeValidator.fieldValidator[i];
                        for (var j = 0; j < fieldValidator.validators.length; j++) {
                            var servierValidator = fieldValidator.validators[j];
                            var clientValidator = servetToClient(servierValidator, fieldValidator);
                            clientValidators.push(clientValidator);
                        }
                    }

                    if (customClientValidators && customClientValidators.length) {
                        clientValidators = linq.from(clientValidators).concat(customClientValidators).toArray();
                    }

                    return clientValidators;
                }

                service.validateData = function (data, serverTypeValidator, customClientValidators) {
                    var clientValidators = service.parseValidators(serverTypeValidator, customClientValidators);
                    return validateHelper.validateData(data, clientValidators);
                };

                return service;
            }
        ];
    });

    app.constant('validatorCustomList', {
        'FileReady': function (ready, message) {
            this.validateData = function (value, name, validationContext) {
                if (!ready) {
                    validateHelper.updateValidationContext(validationContext, name, message);
                    return false;
                }
                return true;
            };
        },
        'ExistsValidator': function (maxLen, message) {
            this.validateData = function (value, name, validationContext) {
                return true;
            };
        }
    });
});

//demo , 对照后台接口写天气凉
var Seagull2_PerformancePlan_WebApi_Validator_TypeValidator_Demo =
 {
     "typeName": "ScoringStandardTrans",
     "typeFullName": "SinoOcean.Seagull2.TransactionData.PerformancePlan.ScoringStandardTrans",
     "fieldValidator": [{
         "name": "ScoringStandardFormCode",
         "description": "表单编码",
         "typeName": "String",
         "typeFullName": "System.String",
         "validators": [{
             "allowEmptyStrings": false,
             "requiresValidationContext": false,
             "errorMessage": "表单编码不能为空",
             "errorMessageResourceName": null,
             "errorMessageResourceType": null,
             "typeId": "System.ComponentModel.DataAnnotations.RequiredAttribute, System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
         }
         ]
     }, {
         "name": "CnName",
         "description": "评分标准名称",
         "typeName": "String",
         "typeFullName": "System.String",
         "validators": [{
             "allowEmptyStrings": false,
             "requiresValidationContext": false,
             "errorMessage": "评分标准名称不能为空",
             "errorMessageResourceName": null,
             "errorMessageResourceType": null,
             "typeId": "System.ComponentModel.DataAnnotations.RequiredAttribute, System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
         }, {
             "maximumLength": 50,
             "minimumLength": 0,
             "requiresValidationContext": false,
             "errorMessage": "评分标准名称不能超过64个字符",
             "errorMessageResourceName": null,
             "errorMessageResourceType": null,
             "typeId": "System.ComponentModel.DataAnnotations.StringLengthAttribute, System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
         }
         ]
     }, {
         "name": "ScoreCalculateDesp",
         "description": "分值计算方法",
         "typeName": "String",
         "typeFullName": "System.String",
         "validators": [{
             "allowEmptyStrings": false,
             "requiresValidationContext": false,
             "errorMessage": "分值计算方法不能为空",
             "errorMessageResourceName": null,
             "errorMessageResourceType": null,
             "typeId": "System.ComponentModel.DataAnnotations.RequiredAttribute, System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
         }
         ]
     }, {
         "name": "MaintainReason",
         "description": "维护原因",
         "typeName": "String",
         "typeFullName": "System.String",
         "validators": [{
             "maximumLength": 50,
             "minimumLength": 0,
             "requiresValidationContext": false,
             "errorMessage": "维护原因不能超过255个字符",
             "errorMessageResourceName": null,
             "errorMessageResourceType": null,
             "typeId": "System.ComponentModel.DataAnnotations.StringLengthAttribute, System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
         }
         ]
     }, {
         "name": "SortNo",
         "description": "排序号",
         "typeName": "Int32",
         "typeFullName": "System.Int32",
         "validators": [{
             "pattern": "^-?\\d*$",
             "matchTimeoutInMilliseconds": 0,
             "requiresValidationContext": false,
             "errorMessage": "排序号应为整数",
             "errorMessageResourceName": null,
             "errorMessageResourceType": null,
             "typeId": "System.ComponentModel.DataAnnotations.RegularExpressionAttribute, System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
         }
         ]
     }
     ]
 }