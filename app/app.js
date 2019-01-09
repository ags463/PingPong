angular.module('app', ['ngRoute'])
.config([
    '$routeProvider', '$httpProvider',
function (
     $routeProvider, $httpProvider
) {
    $routeProvider.
        when('/home', {
            templateUrl: 'app/Players/playersList.html',
            controller: 'playersListController'
        }).
        when('/edit/:id', {
            templateUrl: 'app/Players/playersEdit.html',
            controller: 'playersEditController'
        }).
        otherwise({
            redirectTo: '/home'
        });
}])
.run(['$rootScope',
function ($rootScope) {
    $rootScope.loading = false;
}]);