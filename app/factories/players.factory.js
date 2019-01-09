angular.module('app')
.factory('playersFactory', ['$http', function ($http) {
    var serviceRoot = 'api/players/';
    var factory = {
        list: list,
        read: read,
        save: save,
        remove: remove
    };

    return factory;

    //////////

    function list() {
        return $http({
            method: 'GET',
            url: serviceRoot + 'List'
        });
    }

    function read(playerID) {
        return $http({
            method: 'GET',
            url: serviceRoot + 'Read',
            params: { 'PlayerID': playerID }
        });
    }

    function save(player) {
        return $http.put(serviceRoot + 'Save', player);
    }

    function remove(playerID) {
        return $http({
            method: 'GET',
            url: serviceRoot + 'Remove',
            params: { 'playerID': playerID }
        });
    }
}]);