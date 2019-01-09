//This handles retrieving data and is used by controllers. 3 options (server, factory, provider) with 
//each doing the same thing just structuring the functions/data differently.
angular.module('app')
.service('loadingInterceptor', ['$q', '$rootScope', '$log',
function ($q, $rootScope, $log) {
    'use strict';

    var numLoading = 0;

    return {
        request: function (config) {
            numLoading++;
            $rootScope.loading = true;
            return config || $q.when(config);
        },
        requestError: function (rejection) {
            if (!(--numLoading))
                $rootScope.loading = false;
            $log.error('Request error:', rejection);
            return $q.reject(rejection);
        },
        response: function (response) {
            if ((--numLoading) === 0)
                $rootScope.loading = false;
            return response || $q.when(response);
        },
        responseError: function (rejection) {
            if (!(--numLoading))
                $rootScope.loading = false;
            $log.error('Response error:', rejection);
            return $q.reject(rejection);
        }
    };
}]);