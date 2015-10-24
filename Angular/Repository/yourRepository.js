(function () {
    'use strict';
    angular.module('DataService')
            .factory('your_factory_name', ['$resource', functionNameBelow]);

    function functionNameBelow($resource) {
        //return $resource('/api/YourRESTEndPoint/:Id', { Id: '@Id' }); //NOTE: use this one if use custom action web api
        return $resource('/api/YourRESTEndPoint/:Id', null, {
            'update': {method : 'PUT'}
        });
    }

}());