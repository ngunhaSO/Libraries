(function () {

    'use strict';

    var app = angular.module('Your_App_Module', ['DataService', 'ui.router', 'ui.mask', 'ui.bootstrap']);

    app.config(['$stateProvider', '$urlRouterProvider',
        function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/');

            $stateProvider.state('home', {
                url: '/',
                templateUrl : 'path_to_your_angular_view'
            })
            .state('state_name', {
                url: '/RESTpath',
                templateUrl: 'path_to_your_angular_view'
                controller: 'YourController as vm'
            })
            .state('state_name', {
                url: '/RESTpath/:Id',
                templateUrl: 'path_to_your_angular_view',
                controller: 'YourController as vm',
                resolve: {
                    repo: "name_of_repo", //assign services here

                    object_to_resolve: function (repo, $stateParams) { 
                        var Id = $stateParams.Id;
                        return repo.get({ Id: Id }).$promise; 
                    }
                }
            })
            .state('state_name', { //===== Parent state
                abstract : true,
                url: '/RESTPath/edit/:Id',
                templateUrl: 'path_to_your_angular_view',
                controller: 'YourController as vm',
                resolve: {
                    repo: 'name_of_repo',
                    object_to_resolve: function (repo, $stateParams) {
                        var Id = $stateParams.Id;
                        return repo.get({ Id: Id }).$promise;
                    }
                }
            })
            .state('state_name.nested_state_name', { //===== Child state
                url : '/first_nested_path',
                templateUrl: 'path_to_your_angular_view'
            })
            .state('state_name.nested_state_name2', { //==== Child state
                url: '/second_nested_path',
                templateUrl: 'path_to_your_angular_view'
            })
            .state('state_name.third_nested_state_name3', { //====Child state
                url: '/third_nested_path',
                templateUrl: 'path_to_your_angular_view'
            })
        }
    ]);

}());