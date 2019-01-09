angular.module('app')
.controller('playersListController', [
    '$scope', '$rootScope', '$window', '$timeout',
    'modals', 'playersFactory',
function (
    $scope, $rootScope, $window, $timeout, 
    modals, playersFactory
) {
    // Our player list
    $scope.players = [];

    // Command Functions
    $scope.delete = function (player) {
        modals.open(
            "confirm",
            {
                message: "Delete " + player.FirstName + " " + player.LastName+ "?",
                confirmButton: "Yes",
                denyButton: "No, Better Not"
            }
        )
        .then(function (response) {
                // This is the function that is called when the modal is confirmed
                playersFactory.remove(player.PlayerID)
                .then(function () {
                    doRefresh();
                })
        })
        .catch(function () {
            // Do nothing
        });
    };

    $scope.edit = function (player) {
        $window.location.href = "index.html#!/edit/" + player.PlayerID;
    };

    $scope.add = function () {
        $window.location.href = "index.html#!/edit/0";
    };

    // Common Utility Functions
    function startWait() {
        // This is the big loading spinner
        $scope.loading = true;
        // This is the spinning mouse cursor
        $scope.bodyCursor = "wait-cursor";
    }

    function endWait() {
        $scope.loading = false;
        $scope.bodyCursor = "";
    }

    function doRefresh() {
        startWait();
        playersFactory.list()
        .then(function (result) {
            $scope.players = result.data;
            endWait();
        });
    }

    // Set up and start our timer
    $scope.currentDate = new Date();
    $scope.tickInterval = 1000 //ms

    var tick = function () {
        $scope.currentDate = new Date(); // get the current time
        $timeout(tick, $scope.tickInterval); // reset the timer
    };

    $timeout(tick, $scope.tickInterval);

    // Load our data
    doRefresh();
}]);