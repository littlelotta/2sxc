﻿// By default, eav-controls assume that all their parameters (appId, etc.) are instantiated by the bootstrapper
// but the "root" component must get it from the url
// Since different objects could be the root object (this depends on the initial loader), the root-one must have
// a connection to the Url, but only when it is the root
// So the trick is to just include this file - which will provide values for the important attribute
//
// As of now, it only supplies
// * dialog
//
// note that the following params are already taken care of by the InitParametersFromUrl of the EAV:
// - appId
// - zoneId
// - entityId
// - contentTypeName
// - pipelineId
// - $modalInstance (the dummy object, if needed)

//(function () {
    angular.module("InitSxcParametersFromUrl", ["2sxc4ng"])
        .factory("dialog", function($2sxc) {
            return $2sxc.urlParams.get("dialog");
        })
        .factory("tabId", function($2sxc) {
            return $2sxc.urlParams.get("tid");
        })
        .factory("items", function ($2sxc) {
                var found = $2sxc.urlParams.get("items");
                if (found)
                    return (found) ? JSON.parse(found) : null;
            })
        .factory("prefill", function ($2sxc) {
                var found = $2sxc.urlParams.get("prefill");
                if (found)
                    return (found) ? JSON.parse(found) : null;
        })
        //.factory("groupGuid", function ($2sxc) {
        //    return $2sxc.urlParams.get("groupguid");
        //})
        //.factory("groupSet", function ($2sxc) {
        //    return $2sxc.urlParams.get("groupset");
        //})
        //.factory("groupIndex", function ($2sxc) {
        //    return $2sxc.urlParams.get("groupindex");
        //})
    
    ;



// }());